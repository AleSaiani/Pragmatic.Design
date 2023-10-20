namespace Pragmatic.Design.Core.Endpoints.Annotations;

public class HttpPostAttribute : Attribute
{
    public string? Template { get; }

    public HttpPostAttribute(string? template = null)
    {
        Template = template;
    }
}
