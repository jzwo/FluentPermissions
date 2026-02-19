using System;
using FluentPermissions.Core.Abstractions;

namespace FluentPermissions.Core.Builder;

/// <summary>
///     统一权限构建器：同一类型既可作为顶层入口，也可作为组内构建器。
/// </summary>
/// <typeparam name="TOptions"></typeparam>
public sealed class PermissionBuilder<TOptions>
    where TOptions : PermissionOptionsBase, new()
{
    internal PermissionBuilder(string? groupName = null)
    {
        GroupName = groupName;
    }

    /// <summary>
    ///     当前构建上下文对应的组名；顶层构建器为 null。
    /// </summary>
    public string? GroupName { get; }

    /// <summary>
    ///     定义子组，并在提供的 <paramref name="configureGroup" /> lambda 作用域内完成其结构与元数据配置。
    ///     返回当前构建器，便于在同一层级继续定义。
    /// </summary>
    public PermissionBuilder<TOptions> DefineGroup(
        string logicalName,
        Action<PermissionBuilder<TOptions>> configureGroup)
    {
        var child = new PermissionBuilder<TOptions>(logicalName);
        configureGroup(child);
        return this;
    }

    /// <summary>
    ///     定义顶层权限组（带显示名），并在 <paramref name="configureGroup" /> 中配置组内结构与元数据。
    /// </summary>
    public PermissionBuilder<TOptions> DefineGroup(
        string logicalName,
        string displayName,
        Action<PermissionBuilder<TOptions>> configureGroup)
    {
        var child = new PermissionBuilder<TOptions>(logicalName);
        configureGroup(child);
        return this;
    }

    /// <summary>
    ///     定义顶层权限组（带显示名与描述），并在 <paramref name="configureGroup" /> 中配置组内结构与元数据。
    /// </summary>
    public PermissionBuilder<TOptions> DefineGroup(
        string logicalName,
        string displayName,
        string description,
        Action<PermissionBuilder<TOptions>> configureGroup)
    {
        var child = new PermissionBuilder<TOptions>(logicalName);
        configureGroup(child);
        return this;
    }

    /// <summary>
    ///     配置当前权限组的元数据（扩展属性）。
    ///     该方法的主要作用是为源生成器提供一个明确的信号，以采集组级选项的常量赋值。
    /// </summary>
    /// <param name="configureOptions">用于配置 <typeparamref name="TOptions" /> 的委托。</param>
    /// <returns>返回当前构建器以支持链式调用。</returns>
    public PermissionBuilder<TOptions> WithOptions(Action<TOptions> configureOptions)
    {
        // 运行时无需保存任何状态；源生成器会从 lambda 中提取常量赋值。
        var options = new TOptions();
        configureOptions(options);
        // 不做持久化，保持无副作用。
        return this;
    }

    /// <summary>
    ///     定义权限项（最简形式）。
    /// </summary>
    // AddPermission 重载（同 DefineGroup 的模式）
    public PermissionBuilder<TOptions> AddPermission(string logicalName)
    {
        return this;
    }

    /// <summary>
    ///     定义权限项并指定显示名。
    /// </summary>
    public PermissionBuilder<TOptions> AddPermission(string logicalName,
        string displayName)
    {
        return this;
    }

    /// <summary>
    ///     定义权限项并指定显示名与描述。
    /// </summary>
    public PermissionBuilder<TOptions> AddPermission(string logicalName,
        string displayName, string description)
    {
        return this;
    }

    /// <summary>
    ///     定义权限项，仅配置扩展属性。
    /// </summary>
    public PermissionBuilder<TOptions> AddPermission(string logicalName,
        Action<TOptions> configure)
    {
        return this;
    }

    /// <summary>
    ///     定义权限项，指定显示名并配置扩展属性。
    /// </summary>
    public PermissionBuilder<TOptions> AddPermission(string logicalName,
        string displayName, Action<TOptions> configure)
    {
        return this;
    }

    /// <summary>
    ///     定义权限项，最完整重载。
    /// </summary>
    public PermissionBuilder<TOptions> AddPermission(string logicalName,
        string displayName, string description, Action<TOptions> configure)
    {
        return this;
    }
}
