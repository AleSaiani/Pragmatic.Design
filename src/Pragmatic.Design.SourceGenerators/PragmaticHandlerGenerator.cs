using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator]
public class ImplementingClassesGenerator : ISourceGenerator
{
    private const string DOMAIN_ACTION_INTERFACE = "IDomainAction";
    private const string HAS_ENDPOINT_INTERFACE = "IHasEndpoint";
    private const string HAS_CUSTOM_ENDPOINT_INTERFACE = "IHasCustomEndpoint";

    public void Initialize(GeneratorInitializationContext context)
    {
        // Non sono richieste operazioni di inizializzazione in questo esempio
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var implementingClasses = new List<string>();
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
                if (classSymbol != null && ImplementsInterface(classSymbol, DOMAIN_ACTION_INTERFACE))
                {
                    var interfaceType = classSymbol.AllInterfaces.FirstOrDefault(iface => iface.Name == DOMAIN_ACTION_INTERFACE);
                    if (interfaceType != null && interfaceType.TypeArguments.Length == 2)
                    {
                        var requestType = interfaceType.TypeArguments[0];
                        var responseType = interfaceType.TypeArguments[1];

                        var implementingClassName = classSymbol.Name;
                        var namespaceName = classSymbol.ContainingNamespace.ToString();

                        // Genera il codice per PragmaticHandler
                        var handlerCode = GenerateHandlerCode(namespaceName, implementingClassName, requestType, responseType);
                        context.AddSource($"{implementingClassName}_Handler.cs", SourceText.From(handlerCode, Encoding.UTF8));

                        // Genera il codice per PragmaticEndpoint se ha l'interfaccia IHasEndpoint o IHasCustomEndpoint
                        if (HasEndpointInterface(classSymbol))
                        {
                            var endpointCode = GenerateEndpointCode(namespaceName, implementingClassName, requestType, responseType);
                            context.AddSource($"{implementingClassName}_Endpoint.cs", SourceText.From(endpointCode, Encoding.UTF8));
                        }
                    }
                }
            }
        }

        if (implementingClasses.Any())
        {
            var outputText = string.Join(Environment.NewLine, implementingClasses);
            context.AddSource("prova.txt", outputText);
        }
    }

    private bool ImplementsInterface(INamedTypeSymbol classSymbol, string interfaceName)
    {
        return classSymbol.AllInterfaces.Any(iface => iface.Name == interfaceName);
    }

    private bool HasEndpointInterface(INamedTypeSymbol classSymbol)
    {
        return classSymbol.AllInterfaces.Any(iface => iface.Name == HAS_ENDPOINT_INTERFACE || iface.Name == HAS_CUSTOM_ENDPOINT_INTERFACE);
    }

    private string GenerateHandlerCode(string namespaceName, string className, ITypeSymbol requestType, ITypeSymbol responseType)
    {
        var handlerCode =
            $@"
using System;
using Pragmatic.Design.Core.Abstractions;
using Pragmatic.Design.Core.Endpoints;

namespace {namespaceName}
{{
    public class {className}Handler : MediatorHandler<{requestType}, {responseType}>
    {{
        public {className}Handler(Root root) : base(root)
        {{
        }}
    }}
}}
";
        return handlerCode;
    }

    private string GenerateEndpointCode(string namespaceName, string className, ITypeSymbol requestType, ITypeSymbol responseType)
    {
        var endpointCode =
            $@"
using System;
using Pragmatic.Design.Core.Endpoints;

namespace {namespaceName}
{{
    public class {className}Endpoint : MediatorEndpoint<{requestType}, {responseType}>
    {{
    }}
}}
";
        return endpointCode;
    }
}
