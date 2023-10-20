using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator]
public class EndpointInfoGenerator : ISourceGenerator
{
    private const string HAS_ENDPOINT_INTERFACE = "IHasEndpoint";
    private const string HAS_CUSTOM_ENDPOINT_INTERFACE = "IHasCustomEndpoint";

    public void Initialize(GeneratorInitializationContext context)
    {
        // Non sono richieste operazioni di inizializzazione in questo esempio
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var compilation = context.Compilation;
        var syntaxTrees = compilation.SyntaxTrees;

        foreach (var syntaxTree in syntaxTrees)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var root = syntaxTree.GetRoot();
            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (var classSyntax in classes)
            {
                var classSymbol = semanticModel.GetDeclaredSymbol(classSyntax);
                if (classSymbol != null && HasEndpointInterface(classSymbol))
                {
                    var className = classSymbol.Name;
                    var namespaceName = classSymbol.ContainingNamespace.ToString();
                    var attributes = GetAttributes(classSymbol);
                    var actionDomains = GetActionDomains(classSymbol);
                    var properties = GetProperties(classSymbol);

                    var comment = GenerateComment(className, namespaceName, attributes, actionDomains, properties);
                    context.AddSource($"{className}_InfoComment.cs", SourceText.From(comment, Encoding.UTF8));
                }
            }
        }
    }

    private bool HasEndpointInterface(INamedTypeSymbol classSymbol)
    {
        return classSymbol.AllInterfaces.Any(iface => iface.Name == HAS_ENDPOINT_INTERFACE || iface.Name == HAS_CUSTOM_ENDPOINT_INTERFACE);
    }

    private IEnumerable<string> GetAttributes(INamedTypeSymbol classSymbol)
    {
        return classSymbol.GetAttributes().Select(attribute => attribute.AttributeClass.ToDisplayString());
    }

    private IEnumerable<string> GetActionDomains(INamedTypeSymbol classSymbol)
    {
        return classSymbol.AllInterfaces
            .Where(iface => iface.Name == "IDomainAction" && iface.TypeArguments.Length == 2)
            .Select(iface => $"IDomainAction<{iface.TypeArguments[0]}, {iface.TypeArguments[1]}>");
    }

    private IEnumerable<string> GetProperties(INamedTypeSymbol classSymbol)
    {
        return classSymbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Select(property => $"{property.Type} {property.Name} - Attributes: {string.Join(", ", property.GetAttributes().Select(attr => attr.ToString()))}");
    }

    private string GenerateComment(string className, string namespaceName, IEnumerable<string> attributes, IEnumerable<string> actionDomains, IEnumerable<string> properties)
    {
        var comment =
            $@"
// Class: {className}
// Namespace: {namespaceName}
// Attributes: {string.Join(", ", attributes)}
// Action Domains: {string.Join(", ", actionDomains)}

// Properties:
// {string.Join(Environment.NewLine + "// ", properties)}
";
        return comment;
    }
}
