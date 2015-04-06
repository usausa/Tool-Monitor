namespace Tool.Monitor
{
    using Tool.Monitor.Providers;

    /// <summary>
    /// 
    /// </summary>
    public class CollectorInformation
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IValueProvider ValueProvider { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GraphOption GraphOption { get; set; }
    }
}
