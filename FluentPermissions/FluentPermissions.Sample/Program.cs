using System;
using FluentPermissions.Sample.Security;

namespace FluentPermissions.Sample;

public static class Program
{
    public static void Main()
    {
        Console.WriteLine($"Users.Create = {AppPermissions.System_Users_Create}");
        Console.WriteLine($"Users.Delete = {AppPermissions.System_Users_Delete}");
        Console.WriteLine($"Roles.Assign = {AppPermissions.System_Roles_Assign}");

        Console.WriteLine("All descriptors:");
        foreach (var item in AppPermissions.All)
            Console.WriteLine($"- {item.Code} | {item.Name} | {item.DisplayOrName} | {item.Parent} | Leaf={item.IsLeaf}");

        Console.WriteLine("All codes:");
        foreach (var code in AppPermissions.GetAllCodes())
            Console.WriteLine($"- {code}");
    }
}

public class TestAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
