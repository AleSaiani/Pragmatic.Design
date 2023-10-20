namespace Pragmatic.Design.Core.Endpoints.Annotations;

public class HttpPutAttribute : Attribute
{
    public string? Template { get; }

    public HttpPutAttribute(string? template = null)
    {
        Template = template;
    }
}
