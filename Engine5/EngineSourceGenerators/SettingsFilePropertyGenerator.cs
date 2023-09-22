using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EngineSourceGenerators; 

[Generator]
public class SettingsFilePropertyGenerator : ISourceGenerator {
	public void Execute( GeneratorExecutionContext context ) {
		var classDefinitions2 = context.Compilation.SyntaxTrees.SelectMany( p => p.GetRoot().DescendantNodes() ).OfType<ClassDeclarationSyntax>()/*.Where(p => p.BaseList?.Types.Any(q => q.KindText == "SettingsFileBase")).Select(p => (p, p.BaseList))*/.ToList();
		var semantic = classDefinitions2.Select( p => context.Compilation.GetSemanticModel( p.SyntaxTree ).GetDeclaredSymbol(p) ).ToList();
		var derived = semantic.Where(p => p.BaseType?.Name == "SettingsFileBase" ).ToList();
		var members = derived.SelectMany( p => p.GetMembers().Where(q => q.Kind == SymbolKind.Property) ).ToList();
	}

	public void Initialize( GeneratorInitializationContext context ) {

	}
}
