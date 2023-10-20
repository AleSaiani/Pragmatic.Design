using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Pragmatic.Design.Core.Infrastructure;

public class CamelCaseParametersOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        foreach (var operationDescription in context.AllOperationDescriptions)
        {
            foreach (var parameter in operationDescription.Operation.Parameters)
            {
                if (parameter.Kind == OpenApiParameterKind.Query)
                {
                    parameter.Name = $"{char.ToLowerInvariant(parameter.Name[0])}{parameter.Name[1..]}";
                }
            }
        }

        return true;
    }
}
