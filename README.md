# FluentPermissions

[简体中文](./README.zh-CN.md)

FluentPermissions is a Roslyn incremental source generator for permission modeling in .NET.
You define permissions with a fluent DSL, and get compile-time generated constants + read-only query APIs.

## What It Generates

- Namespace: `$(AssemblyName).Security`
- Entry type: `public static partial class Permissions`
- Flat permission code constants
- Read-only descriptors: `Permissions.All`
- Common lookup/query members:
  - `Permissions.ByCode`
  - `Permissions.AllCodes`, `Permissions.GetAllCodes()`
  - `Permissions.Contains(code)`
  - `Permissions.TryGet(code, out descriptor)`
  - `Permissions.GetByCode(code)`
  - `Permissions.GetChildren(parentCode)`
  - `Permissions.GetLeaves()`

## Repository Layout

- `FluentPermissions.Core` (`netstandard2.0`): contracts + fluent builder DSL
- `FluentPermissions` (`netstandard2.0`): source generator
- `FluentPermissions.Sample` (`net9.0`): usage sample
- `FluentPermissions.Tests` (`net9.0`): generator tests

## Quick Start

1. Add references

- Reference `FluentPermissions.Core` in your app project
- Add `FluentPermissions` as source generator (analyzer/NuGet)

2. Define a single options type

```csharp
using FluentPermissions.Core.Abstractions;

public sealed class SampleOptions : PermissionOptionsBase
{
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public bool IsHighRisk { get; set; }
}
```

3. Implement registrar

```csharp
using FluentPermissions.Core.Abstractions;
using FluentPermissions.Core.Builder;

public sealed class AppPermissionDefinition : IPermissionRegistrar<SampleOptions>
{
    public void Register(PermissionBuilder<SampleOptions> builder)
    {
        builder
            .DefineGroup("System", "System", "Core system settings", system =>
            {
                system.WithOptions(o =>
                {
                    o.DisplayOrder = 10;
                    o.Icon = "fa-gear";
                });

                system.DefineGroup("Users", "User management", users =>
                {
                    users.AddPermission("Create", "Create user");
                    users.AddPermission("Delete", "Delete user", "High risk", o => o.IsHighRisk = true);
                });
            })
            .DefineGroup("Reports", reports =>
            {
                reports.AddPermission("View", "View reports");
                reports.AddPermission("Export", "Export reports");
            });
    }
}
```

4. Consume generated API

```csharp
using MyApp.Security;

var code = Permissions.System_Users_Create; // APP:System:Users:Create

if (Permissions.TryGet(code, out var descriptor))
{
    Console.WriteLine(descriptor!.DisplayOrName);
    Console.WriteLine(descriptor.Parent);
}

foreach (var c in Permissions.GetAllCodes())
    Console.WriteLine(c);
```

## Global Generation Option (Attribute)

Use `PermissionGenerationOptionsAttribute` to control whether groups themselves are emitted as permission items.

- Default: `true`
- If `false`:
  - group descriptors are not emitted into `Permissions.All`
  - group constants are not emitted
  - only leaf permission constants/descriptors remain

You can place it on assembly or registrar class.

```csharp
using FluentPermissions.Core.Abstractions;

[assembly: PermissionGenerationOptions(false)]
// or: [PermissionGenerationOptions(false)] on registrar class
```

## Generated Model

`PermissionDescriptor`:

- `Code`: full code (e.g. `APP:System:Users:Create`)
- `Name`: logical name (e.g. `Create`)
- `DisplayName`: optional display name
- `Parent`: parent code (e.g. `APP:System:Users`)
- `IsLeaf`: whether this is a leaf permission
- `IsGroup => !IsLeaf`
- `DisplayOrName => DisplayName ?? Name`

## Notes

- Analyzer diagnostics include consistency checks for generic options across registrars (`FP0002`).

## License

MIT
