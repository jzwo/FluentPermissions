using System;

namespace FluentPermissions.Core.Builder;

/// <summary>
///     非泛型统一权限构建器（无扩展字段/无 Options）。
/// </summary>
public sealed class PermissionBuilder
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
    ///     定义子组，并在 <paramref name="configureGroup" /> 中配置其子级与权限。
    /// </summary>
    public PermissionBuilder DefineGroup(string logicalName, Action<PermissionBuilder> configureGroup)
    {
        var child = new PermissionBuilder(logicalName);
        configureGroup(child);
        return this;
    }

    /// <summary>
    ///     定义顶层权限组（带显示名），并在 <paramref name="configureGroup" /> 中配置其子级与权限。
    /// </summary>
    public PermissionBuilder DefineGroup(string logicalName, string displayName,
        Action<PermissionBuilder> configureGroup)
    {
        var child = new PermissionBuilder(logicalName);
        configureGroup(child);
        return this;
    }

    /// <summary>
    ///     定义顶层权限组（带显示名与描述），并在 <paramref name="configureGroup" /> 中配置其子级与权限。
    /// </summary>
    public PermissionBuilder DefineGroup(string logicalName, string displayName, string description,
        Action<PermissionBuilder> configureGroup)
    {
        var child = new PermissionBuilder(logicalName);
        configureGroup(child);
        return this;
    }

    /// <summary>
     ///     定义权限项。
     /// </summary>
    public PermissionBuilder AddPermission(string logicalName)
    {
        return this;
    }

    /// <summary>
     ///     定义权限项（带显示名）。
     /// </summary>
    public PermissionBuilder AddPermission(string logicalName, string displayName)
    {
        return this;
    }

    /// <summary>
     ///     定义权限项（带显示名与描述）。
     /// </summary>
    public PermissionBuilder AddPermission(string logicalName, string displayName, string description)
    {
        return this;
    }
}
