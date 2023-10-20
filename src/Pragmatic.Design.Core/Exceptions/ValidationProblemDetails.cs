using Microsoft.AspNetCore.Mvc;

namespace Pragmatic.Design.Core.Exceptions;

public class ValidationProblemDetails : ProblemDetails
{
    public List<string> ValidationErrors { get; set; }

    public ValidationProblemDetails(List<string> validationErrors)
    {
        ValidationErrors = validationErrors;
    }
}
