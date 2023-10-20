namespace Pragmatic.Design.Core.Endpoints.Annotations;

public class HttpGetAttribute : Attribute
{
    public string? Template { get; }

    public HttpGetAttribute(string? template = null)
    {
        Template = template;
    }
}
