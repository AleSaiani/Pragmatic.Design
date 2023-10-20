namespace Pragmatic.Design.Core.Endpoints.Annotations;

public class HttpPatchAttribute : Attribute
{
    public string? Template { get; }

    public HttpPatchAttribute(string? template = null)
    {
        Template = template;
    }
}
