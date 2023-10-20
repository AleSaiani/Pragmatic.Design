namespace Pragmatic.Design.Core.Endpoints.Annotations;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SwaggerResponseAttribute : Attribute
{
    public int StatusCode { get; }
    public Type ResponseType { get; }
    public string? Description { get; }
    public string? ContentType { get; set; }

    public SwaggerResponseAttribute(int statusCode, Type? responseType = null, string? description = null, string? contentType = null)
    {
        StatusCode = statusCode;
        if (responseType is not null)
            ResponseType = responseType;
        Description = description;
        ContentType = contentType;
    }
}
