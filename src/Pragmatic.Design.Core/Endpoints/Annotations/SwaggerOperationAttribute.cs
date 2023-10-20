namespace Pragmatic.Design.Core.Endpoints.Annotations;

[AttributeUsage(AttributeTargets.Class)]
public class SwaggerOperationAttribute : Attribute
{
    public string? Summary { get; set; }
    public string? Description { get; set; }
    public string OperationId { get; set; }
    public string[] Tags { get; set; }

    public SwaggerOperationAttribute(string operationId, string? summary = null, string? description = null, string[]? tags = null)
    {
        OperationId = operationId;
        Tags = tags ?? Array.Empty<string>();
        Summary = summary;
        Description = description;
    }
}
