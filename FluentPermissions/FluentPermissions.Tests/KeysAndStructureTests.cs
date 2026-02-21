using System;
using System.IO;
using System.Linq;
using Xunit;

namespace FluentPermissions.Tests;

public class KeysAndStructureTests
{
    [Fact]
    public void Generate_AppPermissions_From_Generic_Registrar()
    {
        const string sources = """

                               using FluentPermissions.Core.Abstractions;
                               using FluentPermissions.Core.Builder;

                               namespace DemoApp;

                               public class TestOptions : PermissionOptionsBase
                               {
                                   public int Order { get; set; }
                                   public string? Icon { get; set; }
                                   public bool Critical { get; set; }
                               }

                               [PermissionGenerationOptions(false)]
                               public sealed class DemoRegistrar : IPermissionRegistrar<TestOptions>
                               {
                                    public void Register(PermissionBuilder<TestOptions> builder)
                                    {
                                        builder
                                            .DefineGroup("System", "系统", "核心系统设置", system =>
                                            {
                                                system.WithOptions(o => o.Order = 10);
                                               system.DefineGroup("Users", "用户账户管理", users =>
                                               {
                                                   users.AddPermission("Create", "创建用户");
                                               });
                                            })
                                            .DefineGroup("Reports", reports =>
                                            {
                                                reports.WithOptions(o => o.Order = 20);
                                                reports.AddPermission("View", "查看报表");
                                            });
                                    }
                                }

                               """;

        var compilation = TestCompilationHelper.CreateCompilation("GeneratorDriver_Generic_Test", sources);
        var driver = TestCompilationHelper.CreateDriver();

        var runResult = driver.RunGenerators(compilation).GetRunResult();

        Assert.True(runResult.Diagnostics.IsEmpty,
            string.Join(Environment.NewLine, runResult.Diagnostics.Select(d => d.ToString())));

        var files = runResult.GeneratedTrees.Select(t => Path.GetFileName(t.FilePath)).ToArray();
        Assert.Contains("AppPermissions.g.cs", files);
        Assert.Contains("FluentPermissions.g.Models.cs", files);

        var appTree = runResult.GeneratedTrees.First(t => t.FilePath.EndsWith("AppPermissions.g.cs"));
        var appText = appTree.GetText().ToString();

        Assert.Contains("namespace GeneratorDriver_Generic_Test.Security;", appText);
        Assert.Contains("public static partial class AppPermissions", appText);
        Assert.DoesNotContain("public const string System = \"APP:System\";", appText);
        Assert.DoesNotContain("public const string System_Users = \"APP:System:Users\";", appText);
        Assert.Contains("public const string System_Users_Create = \"APP:System:Users:Create\";", appText);
        Assert.DoesNotContain("public const string Reports = \"APP:Reports\";", appText);
        Assert.DoesNotContain("new PermissionDescriptor(\"APP:System\", \"System\"", appText);
        Assert.DoesNotContain("new PermissionDescriptor(\"APP:Reports\", \"Reports\"", appText);
    }

    [Fact]
    public void Generate_From_NonGeneric_Registrar()
    {
        const string sources = """

                               using FluentPermissions.Core.Abstractions;
                               using FluentPermissions.Core.Builder;

                               namespace DemoApp2;

                               public sealed class NonGenericRegistrar : IPermissionRegistrar
                               {
                                   public void Register(PermissionBuilder builder)
                                   {
                                       builder
                                           .DefineGroup("SystemNG", "系统(NG)", system =>
                                           {
                                               system.DefineGroup("Users", "用户管理", users =>
                                               {
                                                   users.AddPermission("Create", "创建用户");
                                               });
                                           })
                                           .DefineGroup("ReportsNG", reports =>
                                           {
                                               reports.AddPermission("View", "查看报表");
                                           });
                                   }
                               }

                               """;

        var compilation = TestCompilationHelper.CreateCompilation("GeneratorDriver_NonGeneric_Test", sources);
        var driver = TestCompilationHelper.CreateDriver();
        var runResult = driver.RunGenerators(compilation).GetRunResult();

        var appSyntax = runResult.GeneratedTrees.Single(t => t.FilePath.EndsWith("AppPermissions.g.cs"));
        var appText = appSyntax.GetText().ToString();

        Assert.Contains("namespace GeneratorDriver_NonGeneric_Test.Security;", appText);
        Assert.Contains("public const string SystemNG = \"APP:SystemNG\";", appText);
        Assert.Contains("public const string ReportsNG = \"APP:ReportsNG\";", appText);
        Assert.Contains("new PermissionDescriptor(\"APP:SystemNG\", \"SystemNG\"", appText);
    }
}
