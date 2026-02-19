using System;

namespace FluentPermissions.Core.Abstractions;

/// <summary>
/// 权限生成选项特性，用于控制权限生成器在生成权限项时的行为。
/// </summary>
/// <param name="includeGroupAsPermission"></param>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = false)]
public sealed class PermissionGenerationOptionsAttribute(bool includeGroupAsPermission = true) : Attribute
{
    /// <summary>
    /// 是否将权限组也作为权限项进行生成。默认为 true，即权限组本身也会被视为一个权限项。
    /// </summary>
    public bool IncludeGroupAsPermission { get; } = includeGroupAsPermission;
}
