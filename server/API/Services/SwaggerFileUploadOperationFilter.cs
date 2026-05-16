using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Server.API.Services;

public sealed class SwaggerFileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var parameters = context.ApiDescription.ParameterDescriptions;
        var hasFile = parameters.Any(p => typeof(IFormFile).IsAssignableFrom(p.Type) || typeof(IFormFileCollection).IsAssignableFrom(p.Type));

        if (!hasFile)
        {
            return;
        }

        var properties = new Dictionary<string, OpenApiSchema>(StringComparer.OrdinalIgnoreCase);
        var required = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var p in parameters)
        {
            var name = string.IsNullOrWhiteSpace(p.Name) ? "file" : p.Name;

            if (typeof(IFormFileCollection).IsAssignableFrom(p.Type))
            {
                properties[name] = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema { Type = "string", Format = "binary" }
                };
                required.Add(name);
                continue;
            }

            if (typeof(IFormFile).IsAssignableFrom(p.Type))
            {
                properties[name] = new OpenApiSchema { Type = "string", Format = "binary" };
                required.Add(name);
                continue;
            }

            var t = p.Type;
            if (t == typeof(string))
            {
                properties[name] = new OpenApiSchema { Type = "string" };
                continue;
            }

            if (t == typeof(int) || t == typeof(int?))
            {
                properties[name] = new OpenApiSchema { Type = "integer", Format = "int32" };
                continue;
            }

            if (t == typeof(long) || t == typeof(long?))
            {
                properties[name] = new OpenApiSchema { Type = "integer", Format = "int64" };
                continue;
            }

            if (t == typeof(bool) || t == typeof(bool?))
            {
                properties[name] = new OpenApiSchema { Type = "boolean" };
                continue;
            }

            properties[name] = new OpenApiSchema { Type = "string" };
        }

        operation.RequestBody = new OpenApiRequestBody
        {
            Required = true,
            Content =
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = properties,
                        Required = required
                    }
                }
            }
        };
    }
}
