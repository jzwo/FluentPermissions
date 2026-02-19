using System.Linq;
using Xunit;

namespace FluentPermissions.Tests;

public class MultiRegistrarTests
{
    [Fact]
    public void Multiple_Registrars_Combine_Tree()
    {
        const string sources = """

                               using FluentPermissions.Core.Abstractions;
                               using FluentPermissions.Core.Builder;

                               namespace DemoMulti;

                               public class O : PermissionOptionsBase { public int Order { get; set; } public string? Icon { get; set; } public bool Critical { get; set; } }

                               public sealed class SalesReg : IPermissionRegistrar<O>
                               {
                                   public void Register(PermissionBuilder<O> builder)
                                   {
                                       builder.DefineGroup("Sales", s => { s.WithOptions(o => { o.Order = 100; o.Icon = "fa-dollar"; }); s.AddPermission("View", "View"); });
                                   }
                               }

                               public sealed class HrReg : IPermissionRegistrar<O>
                               {
                                   public void Register(PermissionBuilder<O> builder)
                                   {
                                       builder.DefineGroup("HR", h => { h.AddPermission("Edit", "Edit", o => o.Critical = true); });
                                   }
                               }

                               """;

        var compilation = TestCompilationHelper.CreateCompilation("GeneratorDriver_Multi_Test", sources);
        var driver = TestCompilationHelper.CreateDriver();
        var result = driver.RunGenerators(compilation).GetRunResult();

        var app = result.GeneratedTrees.Single(t => t.FilePath.EndsWith("Permissions.g.cs"));
        var appText = app.GetText().ToString();

        Assert.Contains("public const string Sales_View = \"APP:Sales:View\";", appText);
        Assert.Contains("public const string HR_Edit = \"APP:HR:Edit\";", appText);
        Assert.Contains("new PermissionDescriptor(\"APP:Sales:View\"", appText);
    }
}
