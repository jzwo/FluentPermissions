using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;

namespace FluentPermissions.Tests;

public class DiagnosticsTests
{
    [Fact]
    public void Inconsistent_Generic_Options_Produces_Error_Diagnostic()
    {
        const string sources = """

                               using FluentPermissions.Core.Abstractions;
                               using FluentPermissions.Core.Builder;

                               namespace DemoDiag;

                               public class O1 : PermissionOptionsBase { }
                               public class O2 : PermissionOptionsBase { }

                               public sealed class Reg1 : IPermissionRegistrar<O1>
                               {
                                   public void Register(PermissionBuilder<O1> builder)
                                   {
                                       builder.DefineGroup("A", _ => { });
                                   }
                               }

                               public sealed class Reg2 : IPermissionRegistrar<O2>
                               {
                                   public void Register(PermissionBuilder<O2> builder)
                                   {
                                       builder.DefineGroup("B", _ => { });
                                   }
                               }

                               """;

        var compilation = TestCompilationHelper.CreateCompilation("GeneratorDriver_Diag_Inconsistent_Test", sources);
        var driver = TestCompilationHelper.CreateDriver();
        var result = driver.RunGenerators(compilation).GetRunResult();

        var diag = result.Diagnostics.FirstOrDefault(d => d.Id == "FP0002");
        Assert.NotNull(diag);
        Assert.Equal(DiagnosticSeverity.Error, diag!.Severity);
    }

    [Fact]
    public void Missing_Register_Method_Produces_Warning_Diagnostic()
    {
        const string sources = """

                               using FluentPermissions.Core.Abstractions;

                               namespace DemoWarn;

                               public class O : PermissionOptionsBase { }

                               public sealed class BadRegistrar : IPermissionRegistrar<O>
                               {
                                   // Intentionally missing Register method
                               }

                               """;

        var compilation = TestCompilationHelper.CreateCompilation("GeneratorDriver_Diag_MissingRegister_Test", sources);
        var driver = TestCompilationHelper.CreateDriver();
        var result = driver.RunGenerators(compilation).GetRunResult();

        var diag = result.Diagnostics.FirstOrDefault(d => d.Id == "FP0001");
        Assert.NotNull(diag);
        Assert.Equal(DiagnosticSeverity.Warning, diag!.Severity);
    }
}
