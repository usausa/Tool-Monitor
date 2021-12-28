using System.Diagnostics;

using StackExchange.Redis;

using var connection = ConnectionMultiplexer.Connect("redis-server:6379,allowAdmin=true");

var server = connection.GetServer(connection.GetEndPoints()[0]);

foreach (var group in server.Info())
{
    Debug.WriteLine($"{group.Key}:");
    foreach (var pair in group)
    {
        Debug.WriteLine($"  {pair.Key}:{pair.Value}");
    }
}
