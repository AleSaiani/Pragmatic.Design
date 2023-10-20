using Microsoft.AspNetCore.Mvc;

namespace Pragmatic.Design.Core.Abstractions.Domain;

public abstract class DomainException : Exception
{
    public abstract ProblemDetails ToProblemDetails();
}
