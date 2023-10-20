namespace Pragmatic.Design.Core.Endpoints.Annotations;

public class HttpDeleteAttribute : Attribute
{
    public string? Template { get; }

    public HttpDeleteAttribute(string? template = null)
    {
        Template = template;
    }
}
