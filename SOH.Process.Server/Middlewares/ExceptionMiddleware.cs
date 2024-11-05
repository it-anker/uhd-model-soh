using System.Net;
using System.Text.Json;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Localization;
using Serilog;
using Serilog.Context;
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
        catch (ValidationException exception)
        {
            await HandleValidationException(context, exception);
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

        var errorResult = new ExceptionResult
        {
            ErrorId = errorId,
            SupportMessage = localizer["Provide the ErrorId to the support team for further analysis"]
        };

#if DEBUG
        errorResult.Source = exception.TargetSite?.DeclaringType?.FullName;
        errorResult.Exception = exception.Message.Trim();
        errorResult.Messages.Add(exception.Message);

        Log.Logger.Error("{ExceptionMessage}", exception.Message);
        Log.Logger.Error("{StackTrace}", exception.StackTrace);
#endif
        var response = context.Response;
        response.ContentType = "application/json";
        if (exception is not CustomException && exception.InnerException != null)
            exception = exception.InnerException;

        GetStatusCodeAndMessage(exception, response, errorResult);

        if (response.StatusCode == (int)HttpStatusCode.InternalServerError)
            errorResult.StackTrace = exception.StackTrace;

        await response.WriteAsync(JsonSerializer.Serialize(errorResult));
    }

    private static async Task HandleValidationException(HttpContext context, ValidationException exception)
    {
        CustomValidationResult validationResult = new CustomValidationResult
        {
            Errors = exception.Errors.Select(validationFailure => 
                    validationFailure.Adapt<CustomValidationFailure>())
                .ToList()
        };

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(validationResult);
    }

    private static void GetStatusCodeAndMessage(Exception exception, HttpResponse response, ExceptionResult errorResult)
    {
        switch (exception)
        {
            case CustomException customException:
                errorResult.StatusCode = customException.StatusCode;
                response.StatusCode = (int)errorResult.StatusCode;

                if (customException.ErrorMessages is not null)
                {
                    errorResult.Messages = customException.ErrorMessages;
                }

                break;

            case KeyNotFoundException:
                errorResult.StatusCode = HttpStatusCode.NotFound;
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            default:
                errorResult.StatusCode = HttpStatusCode.InternalServerError;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }
    }
}