using Contacts.Application.DTOs;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Contacts.API.OpenApi;

/// <summary>
/// Ajusta o schema OpenAPI para refletir corretamente obrigatoriedade e nulabilidade dos DTOs.
/// </summary>
public class ContactSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Aplica as regras de schema para os tipos documentados no Swagger.
    /// </summary>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(ContactRequest))
        {
            MarkRequired(schema, "name", "birthDate", "sex");
            MarkNonNullable(schema, "name", "birthDate", "sex");
            return;
        }

        if (context.Type == typeof(ContactResponse))
        {
            MarkRequired(schema, "id", "name", "birthDate", "sex", "age", "isActive", "createdAt", "updatedAt");
            MarkNonNullable(schema, "id", "name", "birthDate", "sex", "age", "isActive", "createdAt", "updatedAt");
        }
    }

    private static void MarkRequired(OpenApiSchema schema, params string[] propertyNames)
    {
        schema.Required ??= new HashSet<string>();

        foreach (var propertyName in propertyNames)
        {
            schema.Required.Add(propertyName);
        }
    }

    private static void MarkNonNullable(OpenApiSchema schema, params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            if (schema.Properties.TryGetValue(propertyName, out var propertySchema))
            {
                propertySchema.Nullable = false;
            }
        }
    }
}
