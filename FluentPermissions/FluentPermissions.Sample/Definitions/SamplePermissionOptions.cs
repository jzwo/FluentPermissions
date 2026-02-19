using FluentPermissions.Core.Abstractions;

namespace FluentPermissions.Sample.Definitions;

public class SampleOptions : PermissionOptionsBase
{
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public bool IsHighRisk { get; set; }
}
