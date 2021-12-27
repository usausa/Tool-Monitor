namespace WorkPlugin.Plugin1;

using WorkPlugin.Abstractions;

public class TestPlugin : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("Test");
    }
}
