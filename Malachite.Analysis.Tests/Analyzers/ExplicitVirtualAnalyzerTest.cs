// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using System.Reflection;

using Malachite.Analysis.Analyzers;

using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<Malachite.Analysis.Analyzers.ExplicitVirtualAnalyzer, Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace Malachite.Analysis.Tests.Analyzers;

public sealed class ExplicitVirtualAnalyzerTest {
    [Fact]
    public async Task RecordNoAnnotation() {
        const string test = "record C {}";
        DiagnosticResult[] expected = [
            Verify.Diagnostic().WithSpan(1, 1, 1, 7).WithArguments("C"),
        ];
        await Verify.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ClassNoAnnotation() {
        const string test = "class C {}";
        DiagnosticResult[] expected = {
            Verify.Diagnostic().WithSpan(1, 1, 1, 6).WithArguments("C"),
        };
        await Verify.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ClassWithAnnotation() {
        var ctx = new CSharpAnalyzerTest<ExplicitVirtualAnalyzer, DefaultVerifier>();
        {
            const string file = "Analysis.cs";
            await using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(file)!;
            ctx.TestState.Sources.Add((file, SourceText.From(stream)));
        }

        const string test1 = "sealed class C {}";
        ctx.TestCode = test1;
        await ctx.RunAsync();
        const string test2 = "static class C2 {}";
        ctx.TestCode = test2;
        await ctx.RunAsync();
        const string test3 = "abstract class C3 {}";
        ctx.TestCode = test3;
        await ctx.RunAsync();
        const string test4 = """
                             using Malachite.Core.Annotations;
                             [Virtual] class C4 {}
                             """;
        ctx.TestCode = test4;
        await ctx.RunAsync();
    }

    [Fact]
    public async Task RecordWithAnnotation() {
        var ctx = new CSharpAnalyzerTest<ExplicitVirtualAnalyzer, DefaultVerifier>();
        {
            const string file = "Analysis.cs";
            await using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(file)!;
            ctx.TestState.Sources.Add((file, SourceText.From(stream)));
        }

        const string test1 = "sealed record C {}";
        ctx.TestCode = test1;
        await ctx.RunAsync();
        await ctx.RunAsync();
        const string test2 = "abstract record C2 {}";
        ctx.TestCode = test2;
        await ctx.RunAsync();
        const string test3 = """
                             using Malachite.Core.Annotations;
                             [Virtual] record C3 {}
                             """;
        ctx.TestCode = test3;
        await ctx.RunAsync();
    }

}
