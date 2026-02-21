using System.Linq;
using Xunit;

namespace FluentPermissions.Tests;

public class TypedOptionsTests
{
    [Fact]
    public void Descriptor_Model_Is_Generated()
    {
        const string sources = """

                               using FluentPermissions.Core.Abstractions;
                               using FluentPermissions.Core.Builder;

                               namespace DemoAppOpts;

                               public class TestOptions : PermissionOptionsBase
                               {
                                   public int Order { get; set; }
                                   public string? Icon { get; set; }
                                   public bool Critical { get; set; }
                               }

                               public sealed class Registrar : IPermissionRegistrar<TestOptions>
                               {
                                   public void Register(PermissionBuilder<TestOptions> builder)
                                   {
                                       builder
                                           .DefineGroup("System", "系统", g =>
                                           {
                                               g.WithOptions(o => { o.Order = 7; o.Icon = "fa-gear"; });
                                               g.AddPermission("P1", "Perm1", o => { o.Critical = true; });
                                           });
                                   }
                               }

                               """;

        var compilation = TestCompilationHelper.CreateCompilation("GeneratorDriver_TypedOpts_Test", sources);
        var driver = TestCompilationHelper.CreateDriver();
        var result = driver.RunGenerators(compilation).GetRunResult();

        var models = result.GeneratedTrees.Single(t => t.FilePath.EndsWith("FluentPermissions.g.Models.cs"));
        var text = models.GetText().ToString();

        Assert.Contains("public sealed class PermissionDescriptor", text);
        Assert.Contains("public string Code { get; }", text);
        Assert.Contains("public string Parent { get; }", text);
        Assert.Contains("public string DisplayOrName => DisplayName ?? Name;", text);
        Assert.Contains("public bool IsLeaf { get; }", text);

        var app = result.GeneratedTrees.Single(t => t.FilePath.EndsWith("AppPermissions.g.cs"));
        var appText = app.GetText().ToString();
        Assert.Contains("public static readonly global::System.Collections.Generic.IReadOnlyDictionary<string, PermissionDescriptor> ByCode", appText);
        Assert.Contains("public static readonly global::System.Collections.Generic.IReadOnlyList<string> AllCodes", appText);
        Assert.Contains("public static global::System.Collections.Generic.IReadOnlyList<string> GetAllCodes() => AllCodes;", appText);
        Assert.DoesNotContain("PermissionTreeNode", appText);
    }
}
