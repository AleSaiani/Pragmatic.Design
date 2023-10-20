using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.Text.Json;

namespace Pragmatic.Design.Core.Endpoints;

/**
 * FastEndpoints support binding of enum arrays from multipart form data only as serialized JSON arrays.
 * This Binder converts the enum arrays to JSON arrays before binding.
 *
 * The static constructor of this class finds all enum array properties and stores their names in a list,
 * the list is then used in the BindAsync method to find the enum array properties and convert them to JSON arrays.
 */
public class MultipartFormRequestBinder<T> : RequestBinder<T>
    where T : notnull
{
    private static readonly IReadOnlyList<string>? _enumPropertyNames = null;

    static MultipartFormRequestBinder()
    {
        List<string>? enumPropertyNames = null;
        typeof(T)
            .GetProperties()
            .ToList()
            .ForEach(x =>
            {
                if (x.PropertyType.IsGenericType && x.PropertyType.IsAssignableTo(typeof(IEnumerable)))
                {
                    x.PropertyType
                        .GetGenericArguments()
                        .ToList()
                        .ForEach(y =>
                        {
                            if (!y.IsEnum)
                                return;
                            enumPropertyNames ??= new List<string>();
                            enumPropertyNames.Add($"{char.ToLowerInvariant(x.Name[0])}{x.Name[1..]}");
                        });
                }
            });
        _enumPropertyNames = enumPropertyNames;
    }

    public override async ValueTask<T> BindAsync(BinderContext ctx, CancellationToken ct)
    {
        if (_enumPropertyNames is not null)
        {
            var form = ctx.HttpContext.Request.Form;
            var newValues = form.Keys.ToDictionary(x => x, x => form[x]);

            foreach (var propName in _enumPropertyNames)
            {
                var types = form[propName];
                var enumValues = types.Select(int.Parse).ToArray();
                var jsonValues = JsonSerializer.Serialize(enumValues);
                newValues[propName] = jsonValues;
            }
            var newForm = new FormCollection(newValues, form.Files);
            ctx.HttpContext.Request.Form = newForm;
        }

        return await base.BindAsync(ctx, ct);
    }
}
