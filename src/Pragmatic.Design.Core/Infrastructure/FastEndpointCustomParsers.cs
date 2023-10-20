using System.Globalization;
using FastEndpoints;

namespace Pragmatic.Design.Core.Infrastructure;

public static class FastEndpointCustomParsers
{
    public static ParseResult DecimalParser(object? input)
    {
        var success = decimal.TryParse(input?.ToString(), CultureInfo.InvariantCulture, out var result);
        return new ParseResult(success, result);
    }

    public static ParseResult DoubleParser(object? input)
    {
        var success = double.TryParse(input?.ToString(), CultureInfo.InvariantCulture, out var result);
        return new ParseResult(success, result);
    }
}
