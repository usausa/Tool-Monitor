namespace WorkHardware;

using System.Diagnostics;

using LibreHardwareMonitor.Hardware;

public static class Program
{
    private static readonly List<ISensor> Sensors = new();

    public static async Task Main()
    {
        Console.WriteLine($"Start: {DateTime.Now:HH:mm:ss.fff}");

        var computer = new Computer
        {
            IsCpuEnabled = true, // 50
            IsGpuEnabled = true, // 200
            IsMemoryEnabled = true, // 20
            IsMotherboardEnabled = true, // 50
            IsControllerEnabled = true, // 100
            IsNetworkEnabled = true, // 80
            IsStorageEnabled = true // 700
        };

        var updateVisitor = new UpdateVisitor();

        // 1000ms+
        Console.WriteLine($"Open1: {DateTime.Now:HH:mm:ss.fff}");
        computer.Open();
        Console.WriteLine($"Open2: {DateTime.Now:HH:mm:ss.fff}");

        // 100ms
        Console.WriteLine($"Accept1: {DateTime.Now:HH:mm:ss.fff}");
        computer.Accept(updateVisitor);
        Console.WriteLine($"Accept2: {DateTime.Now:HH:mm:ss.fff}");

        computer.HardwareAdded += OnHardwareAdded;
        computer.HardwareRemoved += OnHardwareRemoved;

        foreach (var hardware in computer.Hardware)
        {
            OnHardwareAdded(hardware);
        }

        Console.WriteLine($"Booted: {DateTime.Now:HH:mm:ss.fff}");

        Debug.WriteLine("----------");

        var values = Sensors.Where(x => x.Hardware.HardwareType == HardwareType.Cpu && x.SensorType == SensorType.Load).ToArray();

        var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        while (true)
        {
            Console.WriteLine($"Accept1: {DateTime.Now:HH:mm:ss.fff}");
            computer.Accept(updateVisitor);
            Console.WriteLine($"Accept2: {DateTime.Now:HH:mm:ss.fff}");

            foreach (var entry in values)
            {
                Debug.WriteLine(entry.Hardware.Name + "/" + entry.Name + " " + entry.Value);
            }
            Debug.WriteLine("----------");

            await timer.WaitForNextTickAsync();
        }

        //computer.Close();
    }

    private static void OnHardwareAdded(IHardware hardware)
    {
        Debug.WriteLine($"**** Add Hardware: {hardware.Name}:{hardware.HardwareType}");
        hardware.SensorAdded += SensorAdded;
        hardware.SensorRemoved += OnSensorRemoved;

        foreach (var subHardware in hardware.SubHardware)
        {
            Debug.WriteLine($"  SubHardware: {subHardware.Name}:{subHardware.HardwareType}");
            foreach (var sensor in subHardware.Sensors)
            {
                SensorAdded(sensor);
            }
        }

        foreach (var sensor in hardware.Sensors)
        {
            SensorAdded(sensor);
        }
    }

    private static void OnHardwareRemoved(IHardware hardware)
    {
        Debug.WriteLine($"**** Remove Hardware: {hardware.Name}:{hardware.HardwareType}");
    }

    private static void SensorAdded(ISensor sensor)
    {
        Sensors.Add(sensor);
        Debug.WriteLine($"**** Add Sensor: {sensor.Hardware.Name}:{sensor.Hardware.HardwareType}::{sensor.SensorType}:{SensorId(sensor)} [{sensor.Identifier}] value: {sensor.Value}");
    }

    private static string SensorId(ISensor sensor)
    {
        return sensor.Hardware.Parent is null
            ? sensor.Hardware.Name + "/" + sensor.Name
            : sensor.Hardware.Parent.Name + "/" + sensor.Hardware.Name + "/" + sensor.Name;
    }

    private static void OnSensorRemoved(ISensor sensor)
    {
        Debug.WriteLine($"**** Remove Sensor: {sensor.Name}");
    }
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

    public void VisitSensor(ISensor sensor)
    {
    }

    public void VisitParameter(IParameter parameter)
    {
    }
}
