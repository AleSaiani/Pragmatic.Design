using Pragmatic.Design.Core.Abstractions;

namespace Pragmatic.Design.Core.Endpoints;

public class EndpointArgs : EventArgs
{
    public Action PreventDefault { get; }
    public Root Root { get; }

    public EndpointArgs(Root root, Action? preventDefault)
    {
        PreventDefault = preventDefault ?? (() => { });
        Root = root;
    }
}
