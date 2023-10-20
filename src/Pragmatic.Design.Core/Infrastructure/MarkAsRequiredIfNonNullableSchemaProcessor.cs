using NJsonSchema;
using NJsonSchema.Generation;

namespace Pragmatic.Design.Core.Infrastructure;

public class MarkAsRequiredIfNonNullableSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        foreach (var (_, prop) in context.Schema.Properties)
            if (!prop.IsNullable(SchemaType.OpenApi3))
                prop.IsRequired = true;
    }
}
