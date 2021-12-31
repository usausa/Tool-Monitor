namespace WorkHardwareCollector;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

using LibreHardwareMonitor.Hardware;

public static class Program
{
    public static void Main()
    {
        var json = "{ \"Items\": null }";
        var data = JsonSerializer.Deserialize<Data>(json);

        var computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsControllerEnabled = true,
            IsNetworkEnabled = true,
            IsStorageEnabled = true
        };
        computer.Open();

        var updateVisitor = new UpdateVisitor();
        updateVisitor.VisitComputer(computer);

        var visitor = new LookupVisitor(
            SensorType.Fan,
            null,
            new object[] { "Fan #2", "Fan #7", new FilterEntry { HardwareType = HardwareType.GpuNvidia } },
            null,
            new object[] { new FilterEntry { HardwareType = HardwareType.GpuNvidia } });
        //var visitor = new LookupVisitor(
        //    SensorType.Fan,
        //    null,
        //    null,
        //    new object[] { "Fan #2", "Fan #7" },
        //    null);
        //var visitor = new LookupVisitor(
        //    SensorType.Fan,
        //    null,
        //    null,
        //    null,
        //    new object[] { new FilterEntry { HardwareType = HardwareType.GpuNvidia } });
        visitor.VisitComputer(computer);

        Debug.WriteLine("-----");
        var sensors = visitor.ToArray();
        foreach (var sensor in sensors)
        {
            Debug.WriteLine($"{sensor.Identifier} : {sensor.SensorType} : {sensor.Name}");
        }
    }

    //var values = new object[] { "a", new FilterEntry { Name = "x", Type = "y" }, 1 };
    //var stream = new MemoryStream();
    //JsonSerializer.Serialize(stream, values);
    //Debug.WriteLine(Encoding.UTF8.GetString(stream.ToArray()));

    //var json = "{ \"Items\": [\"a\", {\"Type\":\"y\",\"Name\":\"x\"}] }";
    //var data = JsonSerializer.Deserialize<Data>(json);

    //Console.WriteLine($"Start: {DateTime.Now:HH:mm:ss.fff}");

    //var computer = new Computer
    //{
    //    IsCpuEnabled = true,
    //    IsGpuEnabled = true,
    //    IsMemoryEnabled = true,
    //    IsMotherboardEnabled = true,
    //    IsControllerEnabled = true,
    //    IsNetworkEnabled = true,
    //    IsStorageEnabled = true
    //};

    //var updateVisitor = new UpdateVisitor();

    //computer.Open();

    //computer.Accept(updateVisitor);
}

public class Data
{
    [AllowNull]
    [JsonConverter(typeof(CustomConverter))]
    public object[] Items { get; set; }
}

public class CustomConverter : JsonConverter<object[]>
{
    public override object[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        var objects = new List<object>();

        do
        {
            if (!reader.Read())
            {
                throw new JsonException("Read error.");
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                objects.Add(reader.GetString()!);
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                var entry = new FilterEntry();

                if (!reader.Read())
                {
                    throw new JsonException("Read error.");
                }

                do
                {
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException("Invalid entry.");
                    }

                    var property = reader.GetString()!;

                    if (!reader.Read())
                    {
                        throw new JsonException("Read error.");
                    }

                    if (reader.TokenType != JsonTokenType.String)
                    {
                        throw new JsonException("Invalid property value.");
                    }

                    var value = reader.GetString()!;

                    switch (property)
                    {
                        case "Type":
                            entry.HardwareType = Enum.Parse<HardwareType>(value);
                            break;
                        case "Name":
                            entry.Name = value;
                            break;
                        default:
                            throw new JsonException("Invalid property name.");
                    }

                    if (!reader.Read())
                    {
                        throw new JsonException("Read error.");
                    }
                }
                while (reader.TokenType != JsonTokenType.EndObject);

                objects.Add(entry);
            }
        }
        while (reader.TokenType != JsonTokenType.EndArray);

        return objects.ToArray();
    }

    public override void Write(Utf8JsonWriter writer, object[] value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FilterEntry
{
    public HardwareType? HardwareType { get; set; }

    public string? Name { get; set; }
}

public class UpdateVisitor : IVisitor
{
    public void VisitComputer(IComputer computer)
    {
        computer.Traverse(this);
    }
    public void VisitHardware(IHardware hardware)
    {
        hardware.Update();

        foreach (var subHardware in hardware.SubHardware)
        {
            subHardware.Accept(this);
        }
    }
    public void VisitSensor(ISensor sensor) { }

    public void VisitParameter(IParameter parameter) { }
}


public class LookupVisitor : IVisitor
{
    private readonly SensorType sensorType;

    private readonly HardwareType[]? hardwareTypes;

    private readonly object[]? includes;

    private readonly object[]? excludes;

    private readonly object[]? orders;

    private readonly List<ISensor> sensors = new();

    public LookupVisitor(SensorType sensorType, HardwareType[]? hardwareTypes, object[]? includes, object[]? excludes, object[]? orders)
    {
        this.sensorType = sensorType;
        this.hardwareTypes = hardwareTypes;
        this.includes = includes;
        this.excludes = excludes;
        this.orders = orders;
    }

    public void VisitComputer(IComputer computer)
    {
        computer.Traverse(this);
    }

    public void VisitHardware(IHardware hardware)
    {
        foreach (var subHardware in hardware.SubHardware)
        {
            subHardware.Accept(this);
        }

        foreach (var sensor in hardware.Sensors)
        {
            sensor.Accept(this);
        }
    }

    public void VisitSensor(ISensor sensor)
    {
        Debug.WriteLine($"{sensor.Identifier} : {sensor.SensorType} : {sensor.Name}");

        if (IsTarget(sensor))
        {
            sensors.Add(sensor);
        }
    }

    public void VisitParameter(IParameter parameter)
    {
    }

    private bool IsTarget(ISensor sensor)
    {
        if (sensor.SensorType != sensorType)
        {
            return false;
        }

        if ((hardwareTypes?.Length > 0) && !hardwareTypes.Contains(sensor.Hardware.HardwareType))
        {
            return false;
        }

        if (includes?.Length > 0)
        {
            return IsMatch(sensor, includes);
        }

        return !((excludes?.Length > 0) && IsMatch(sensor, excludes));
    }

    public ISensor[] ToArray()
    {
        if (orders?.Length > 0)
        {
            var list = new List<ISensor>();
            var processed = new HashSet<int>();

            for (var i = 0; i < sensors.Count; i++)
            {
                if (IsMatch(sensors[i], orders))
                {
                    list.Add(sensors[i]);
                    processed.Add(i);
                }
            }

            for (var i = 0; i < sensors.Count; i++)
            {
                if (!processed.Contains(i))
                {
                    list.Add(sensors[i]);
                }
            }

            return list.ToArray();
        }

        return sensors.ToArray();
    }

    private static bool IsMatch(ISensor sensor, object[] filters)
    {
        foreach (var filter in filters)
        {
            if (filter is string text)
            {
                if (sensor.Name == text)
                {
                    return true;
                }
            }
            else if (filter is FilterEntry entry)
            {
                if ((!entry.HardwareType.HasValue || (entry.HardwareType.Value == sensor.Hardware.HardwareType)) &&
                    (String.IsNullOrEmpty(entry.Name) || (entry.Name == sensor.Name)))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
