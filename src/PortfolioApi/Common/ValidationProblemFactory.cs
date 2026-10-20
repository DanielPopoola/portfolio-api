using System.Text.Json;
using Microsoft.AspNetCore.Mvc;


namespace PortfolioApi.Common;

public static class ValidationProblemFactory
{
    public static IActionResult Create(ActionContext context)
    {
        var errors = context.ModelState
            .Where(entry => entry.Value!.Errors.Count > 0)
            .ToDictionary(
                entry => JsonNamingPolicy.CamelCase.ConvertName(entry.Key),
                entry => entry.Value!.Errors
                    .Select(error => error.ErrorMessage)
                    .ToArray()
            );

        return new BadRequestObjectResult(new ValidationProblemDetails(errors));
    }
}
