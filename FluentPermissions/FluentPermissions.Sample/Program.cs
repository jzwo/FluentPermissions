using System;
using FluentPermissions.Sample.Security;

namespace FluentPermissions.Sample;

public static class Program
{
    public static void Main()
    {
        Console.WriteLine($"Users.Create = {Permissions.System_Users_Create}");
        Console.WriteLine($"Users.Delete = {Permissions.System_Users_Delete}");
        Console.WriteLine($"Roles.Assign = {Permissions.System_Roles_Assign}");

        Console.WriteLine("All descriptors:");
        foreach (var item in Permissions.All)
            Console.WriteLine($"- {item.Code} | {item.Name} | {item.DisplayOrName} | {item.Parent} | Leaf={item.IsLeaf}");

        Console.WriteLine("All codes:");
        foreach (var code in Permissions.GetAllCodes())
            Console.WriteLine($"- {code}");
    }
}

public class TestAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
