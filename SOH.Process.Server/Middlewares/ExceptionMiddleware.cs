using System.Net;
using System.Text.Json;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Serilog.Context;
using ServiceStack;
using SOH.Process.Server.Models.Common;
using SOH.Process.Server.Models.Common.Exceptions;
using SOH.Process.Server.Resources;

namespace SOH.Process.Server.Middlewares;

public class ExceptionMiddleware(IStringLocalizer<SharedResource> localizer) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleDefaultException(context, exception);
        }
    }

    private async Task HandleDefaultException(HttpContext context, Exception exception)
    {
        string errorId = Guid.NewGuid().ToString();

        LogContext.PushProperty("ErrorId", errorId);
        LogContext.PushProperty("StackTrace", exception.StackTrace);

        var errorResult = new ProblemDetails
        {
            Extensions = { { "errorId", errorId } },
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        var response = context.Response;
        response.ContentType = "application/json";
        if (exception is not CustomException && exception.InnerException != null)
            exception = exception.InnerException;

        GetStatusCodeAndMessage(exception, response, errorResult);
        errorResult.Title = GetTitleFromException(exception);
        if (errorResult.Status != null)
            response.StatusCode = errorResult.Status.GetValueOrDefault();

        if (exception is ValidationException validationException)
        {
            var validations = validationException.Errors
                .Select(failure =>
                {
                    var custom = failure.Severity switch
                    {
                        Severity.Error => CustomSeverity.Error,
                        Severity.Warning => CustomSeverity.Warning,
                        Severity.Info => CustomSeverity.Info,
                        _ => CustomSeverity.Error
                    };

                    return new CustomValidationFailure
                    {
                        Severity = custom,
                        ErrorCode = failure.ErrorCode,
                        ErrorMessage = failure.ErrorMessage,
                        PropertyName = failure.PropertyName.ToLowerInvariant()
                    };
                }).ToList();

            var error = validations.First(failure => failure.Severity == CustomSeverity.Error);
            errorResult.Detail = error.ErrorMessage;
            errorResult.Extensions.Add("validation", validations);
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            errorResult.Status = (int?)HttpStatusCode.BadRequest;
        }

        await response.WriteAsJsonAsync(errorResult);
    }

    private string GetTitleFromException(Exception exception)
    {
        return exception switch
        {
            ValidationException => localizer["Invalid request."],
            BadRequestException => localizer["Invalid request."],
            NotFoundException => localizer["Data not found."],
            ConflictException => localizer["Request with resource already created."],
            UnauthorizedException => localizer["You are not authorized to access this resource."],
            ForbiddenException => localizer["Accessing this resource is not allowed."],
            _ => localizer["Something wrong happened."]
        };
    }

    private static void GetStatusCodeAndMessage(Exception exception,
        HttpResponse response, ProblemDetails errorResult)
    {
        switch (exception)
        {
            case CustomException customException:
                errorResult.Status = (int)customException.StatusCode;

                if (customException.ErrorMessages is not null)
                {
                    errorResult.Extensions.Add("errorMessages", customException.ErrorMessages);
                }

                break;

            case KeyNotFoundException:
                errorResult.Status = (int)HttpStatusCode.NotFound;
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            default:
                errorResult.Status = (int)HttpStatusCode.InternalServerError;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }
    }
}