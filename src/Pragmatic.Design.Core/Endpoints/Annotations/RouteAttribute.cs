namespace Pragmatic.Design.Core.Endpoints.Annotations;

public class RouteAttribute : Attribute
{
    public string Template { get; }

    public RouteAttribute(string template)
    {
        Template = template;
    }
}
