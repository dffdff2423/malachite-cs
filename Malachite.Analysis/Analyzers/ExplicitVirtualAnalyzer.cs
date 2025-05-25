// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Malachite.Analysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExplicitVirtualAnalyzer : DiagnosticAnalyzer {
    private const string VirtualAttr = "Malachite.Core.Annotations.VirtualAttribute";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.ExplicitVirtualRuleId,
        "Types must explicitly opt in to inheritance",
        "Type {0} must be sealed, abstract, static or have the [Virtual] attribute",
        "Maintainability",
        DiagnosticSeverity.Error,
        true,
        "Type must have the [Virtual] attribute or be abstract, static, or sealed.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [Rule];

    public override void Initialize(AnalysisContext ctx) {
        ctx.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        ctx.EnableConcurrentExecution();

        ctx.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
        ctx.RegisterSyntaxNodeAction(AnalyzeRecord, SyntaxKind.RecordDeclaration);
    }

    private static bool HasAttribute(ISymbol? symbol, INamedTypeSymbol attrib) =>
        symbol != null && symbol.GetAttributes().Any(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attrib));

    private static void AnalyzeClass(SyntaxNodeAnalysisContext ctx) {
        var attr = ctx.Compilation.GetTypeByMetadataName(VirtualAttr)!;
        var classDecl = (ClassDeclarationSyntax)ctx.Node;
        if (classDecl.Modifiers.Any(SyntaxKind.SealedKeyword))
            return;
        if (classDecl.Modifiers.Any(SyntaxKind.AbstractKeyword))
            return;
        if (classDecl.Modifiers.Any(SyntaxKind.StaticKeyword))
            return;
        if (HasAttribute(ctx.ContainingSymbol, attr))
            return;

        ctx.ReportDiagnostic(Diagnostic.Create(Rule, classDecl.Keyword.GetLocation(), classDecl.Identifier.ValueText));
    }

    private static void AnalyzeRecord(SyntaxNodeAnalysisContext ctx) {
        var attr = ctx.Compilation.GetTypeByMetadataName(VirtualAttr)!;
        var recordDecl = (RecordDeclarationSyntax)ctx.Node;
        if (recordDecl.Modifiers.Any(SyntaxKind.SealedKeyword))
            return;
        if (recordDecl.Modifiers.Any(SyntaxKind.AbstractKeyword))
            return;
        if (HasAttribute(ctx.ContainingSymbol, attr))
            return;

        ctx.ReportDiagnostic(Diagnostic.Create(Rule, recordDecl.Keyword.GetLocation(), recordDecl.Identifier.ValueText));
    }
}
