# FluentPermissions

[English](./README.md)

FluentPermissions 是一个基于 Roslyn 增量生成器的 .NET 权限建模工具。
你通过 Fluent DSL 定义权限，编译期自动生成常量与只读查询 API。

## 生成内容

- 命名空间：`$(AssemblyName).Security`
- 入口类型：`public static partial class AppPermissions`
- 扁平常量
- 只读描述集合：`AppPermissions.All`
- 常用查询能力：
  - `AppPermissions.ByCode`
  - `AppPermissions.AllCodes`、`AppPermissions.GetAllCodes()`
  - `AppPermissions.Contains(code)`
  - `AppPermissions.TryGet(code, out descriptor)`
  - `AppPermissions.GetByCode(code)`
  - `AppPermissions.GetChildren(parentCode)`
  - `AppPermissions.GetLeaves()`

## 项目结构

- `FluentPermissions.Core`（`netstandard2.0`）：契约与 Fluent Builder DSL
- `FluentPermissions`（`netstandard2.0`）：源码生成器
- `FluentPermissions.Sample`（`net9.0`）：使用示例
- `FluentPermissions.Tests`（`net9.0`）：生成器测试

## 快速开始

1. 添加引用

- 在业务项目引用 `FluentPermissions.Core`
- 以 Analyzer/NuGet 方式引入 `FluentPermissions`

2. 定义单一 Options 类型

```csharp
using FluentPermissions.Core.Abstractions;

public sealed class SampleOptions : PermissionOptionsBase
{
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public bool IsHighRisk { get; set; }
}
```

3. 实现注册器

```csharp
using FluentPermissions.Core.Abstractions;
using FluentPermissions.Core.Builder;

public sealed class AppPermissionDefinition : IPermissionRegistrar<SampleOptions>
{
    public void Register(PermissionBuilder<SampleOptions> builder)
    {
        builder
            .DefineGroup("System", "系统", "核心系统设置", system =>
            {
                system.WithOptions(o =>
                {
                    o.DisplayOrder = 10;
                    o.Icon = "fa-gear";
                });

                system.DefineGroup("Users", "用户管理", users =>
                {
                    users.AddPermission("Create", "创建用户");
                    users.AddPermission("Delete", "删除用户", "高风险操作", o => o.IsHighRisk = true);
                });
            })
            .DefineGroup("Reports", reports =>
            {
                reports.AddPermission("View", "查看报表");
                reports.AddPermission("Export", "导出报表");
            });
    }
}
```

4. 使用生成 API

```csharp
using MyApp.Security;

var code = AppPermissions.System_Users_Create; // APP:System:Users:Create

if (AppPermissions.TryGet(code, out var descriptor))
{
    Console.WriteLine(descriptor!.DisplayOrName);
    Console.WriteLine(descriptor.Parent);
}

foreach (var c in AppPermissions.GetAllCodes())
    Console.WriteLine(c);
```

## 全局生成开关（Attribute）

通过 `PermissionGenerationOptionsAttribute` 控制“组本身是否也生成权限项”。

- 默认：`true`
- 当设为 `false` 时：
  - `AppPermissions.All` 不生成组节点 descriptor
  - 不生成组常量 key
  - 仅保留叶子权限常量与 descriptor

可放在程序集或注册器类上：

```csharp
using FluentPermissions.Core.Abstractions;

[assembly: PermissionGenerationOptions(false)]
// 或在 Registrar 类上标注 [PermissionGenerationOptions(false)]
```

## 生成模型

`PermissionDescriptor` 包含：

- `Code`：完整编码（如 `APP:System:Users:Create`）
- `Name`：逻辑名（如 `Create`）
- `DisplayName`：显示名（可空）
- `Parent`：父级编码（如 `APP:System:Users`）
- `IsLeaf`：是否叶子权限
- `IsGroup => !IsLeaf`
- `DisplayOrName => DisplayName ?? Name`

## 备注

- 生成器会做一致性校验（例如 `FP0002`：所有 registrar 的泛型 options 必须一致）。

## License

MIT
