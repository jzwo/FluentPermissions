using FluentPermissions.Core.Builder;

namespace FluentPermissions.Core.Abstractions;

/// <summary>
///     权限注册器接口。实现此接口的类将被源生成器扫描。
/// </summary>
public interface IPermissionRegistrar<TOptions>
    where TOptions : PermissionOptionsBase, new()
{
    /// <summary>
    ///     register permissions in this method.
    /// </summary>
    /// <param name="builder"></param>
    void Register(PermissionBuilder<TOptions> builder);
}
