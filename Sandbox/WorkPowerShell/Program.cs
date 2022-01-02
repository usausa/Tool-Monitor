namespace WorkPowerShell;

using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

public static class Program
{
    public static void Main()
    {
        using var rs = RunspaceFactory.CreateRunspace();
        rs.Open();

        using var ps = PowerShell.Create();
        ps.Runspace = rs;
        ps.AddCommand("Get-VM");

        var result = ps.Invoke();
        Debug.WriteLine(result.Count);
        foreach (var entry in result)
        {
            Debug.WriteLine(entry.Properties["Name"].Value + " " + entry.Properties["State"].Value);
        }
    }
}
