using System.Linq;
using Xunit;

namespace FluentPermissions.Tests;

public class DeepNestingDriverTests
{
    [Fact]
    public void Deep_Nesting_Generates_Flat_Keys()
    {
        const string sources = """

                               using FluentPermissions.Core.Abstractions;
                               using FluentPermissions.Core.Builder;

                               namespace DemoDeep;

                               public class O : PermissionOptionsBase { }

                               public sealed class Registrar : IPermissionRegistrar<O>
                               {
                                   public void Register(PermissionBuilder<O> builder)
                                   {
                                       builder
                                           .DefineGroup("A", a =>
                                           {
                                               a.DefineGroup("A1", a1 =>
                                               {
                                                   a1.DefineGroup("A1a", aa =>
                                                   {
                                                       aa.AddPermission("X", "X");
                                                   });
                                               });
                                           })
                                           .DefineGroup("B", b => { b.AddPermission("Y", "Y" ); });
                                   }
                               }

                               """;

        var compilation = TestCompilationHelper.CreateCompilation("GeneratorDriver_Deep_Test", sources);
        var driver = TestCompilationHelper.CreateDriver();
        var result = driver.RunGenerators(compilation).GetRunResult();

        var app = result.GeneratedTrees.Single(t => t.FilePath.EndsWith("Permissions.g.cs"));
        var appText = app.GetText().ToString();

        Assert.Contains("public const string A_A1_A1a_X = \"APP:A:A1:A1a:X\";", appText);
        Assert.Contains("public const string B_Y = \"APP:B:Y\";", appText);
    }
}
