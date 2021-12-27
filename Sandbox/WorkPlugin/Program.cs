using System.Reflection;

using WorkPlugin.Abstractions;

var assembly = Assembly.LoadFrom("WorkPlugin.Plugin1.dll");
var types = assembly.ExportedTypes.Where(x => typeof(IPlugin).IsAssignableFrom(x)).ToArray();

var plugin = (IPlugin)Activator.CreateInstance(types[0])!;
plugin.Execute();
