namespace Tool.Monitor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using Tool.Monitor.Providers;

    /// <summary>
    /// 
    /// </summary>
    public class XmlContextLoader
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(XmlContextLoader));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
        public Config Load(string fileName)
        {
            var doc = XDocument.Load(fileName);

            // Collector
            return new Config
            {
                RRDTool =
                    CreateRRDTool(
                        doc.XPathSelectElement("Settings/RRDtool")),
                Template = CreateTemplateInformation(doc.XPathSelectElement("Settings/Template")),
                Collectors =
                    CreateCollectoInformations(
                        doc.XPathSelectElements("Settings/Collector"),
                        CreateValueProviderFactories(
                            doc.XPathSelectElements("Settings/ValueProviderFactory")))
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static RRDTool CreateRRDTool(XElement element)
        {
            return new RRDTool
            {
                ExePath = element.XPathSelectElement("ExePath").Value,
                DataDir = element.XPathSelectElement("DataDir").Value,
                OutputDir = element.XPathSelectElement("OutputDir").Value,
                Colors = element.XPathSelectElements("Colors/Color").Select(_ => _.Value).ToArray()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static TemplateInformation CreateTemplateInformation(XElement element)
        {
            return new TemplateInformation
            {
                ListTemplate = element.XPathSelectElement("ListTemplate").Value,
                DetailTemplate = element.XPathSelectElement("DetailTemplate").Value,
                ListFileName = element.XPathSelectElement("ListFileName").Value,
                DetailFileName = element.XPathSelectElement("DetailFileName").Value,
                OutputDir = element.XPathSelectElement("OutputDir").Value,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static Dictionary<string, IValueProviderFactory> CreateValueProviderFactories(IEnumerable<XElement> elements)
        {
            var factories = new Dictionary<string, IValueProviderFactory>();
            foreach (var element in elements)
            {
                var factory = element.XPathSelectElement("Factory").Value;
                var type = Type.GetType(factory);
                if (type == null)
                {
                    Log.Warn(String.Format(CultureInfo.InvariantCulture, "Load factory failed. factory=[{0}]", factory));
                    continue;
                }

                var instance = (IValueProviderFactory)Activator.CreateInstance(type);
                instance.Initialize(element.XPathSelectElements("Parameter/*").ToLookup(_ => _.Name.ToString(), _ => _.Value));
                factories[element.XPathSelectElement("Type").Value] = instance;
            }
            return factories;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="factories"></param>
        /// <returns></returns>
        private static CollectorInformation[] CreateCollectoInformations(IEnumerable<XElement> elements, Dictionary<string, IValueProviderFactory> factories)
        {
            var collectors = new List<CollectorInformation>();
            foreach (var element in elements)
            {
                var type = element.XPathSelectElement("ValueProvider/Type").Value;
                IValueProviderFactory factory;
                if (!factories.TryGetValue(type, out factory))
                {
                    Log.Warn(String.Format(CultureInfo.InvariantCulture, "Unknown value provider. type=[{0}]", type));
                    continue;
                }

                collectors.Add(new CollectorInformation
                {
                    Id = element.XPathSelectElement("Id").Value,
                    Category = element.XPathSelectElement("Category").Value,
                    ValueProvider = factory.Create(element.XPathSelectElements("ValueProvider/Parameter/*").ToLookup(_ => _.Name.ToString(), _ => _.Value)),
                    GraphOption = CreateGraphOption(element.XPathSelectElement("GraphOption"))
                });
            }
            return collectors.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static GraphOption CreateGraphOption(XElement element)
        {
            return new GraphOption
            {
                Title = element.XPathSelectElement("Title").Value,
                SourceType = element.XPathSelectElement("SourceType").Value,
                Draw = element.XPathSelectElement("Draw").Value,
                Option = element.XPathSelectElement("Option").Value,
                Labels = element.XPathSelectElements("Labels/Label").Select(_ => _.Value).ToArray(),
                Colors = element.XPathSelectElements("Colors/Color").Select(_ => _.Value).ToArray()
            };
        }
    }
}
