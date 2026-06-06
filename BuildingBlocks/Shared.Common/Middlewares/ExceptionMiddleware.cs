using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Common.Exceptions;
using Shared.Common.Responses;
using FluentValidation;

namespace Shared.Common.Middlewares;

public class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger,
    IHostEnvironment env)
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            context.Response.ContentType = "application/json";

            var traceId = context.TraceIdentifier;

            var path = context.Request.Path;

            var method = context.Request.Method;

            int statusCode = StatusCodes.Status500InternalServerError;

            object response;

            switch (ex)
            {
                case ValidationException validationException:
                    statusCode = StatusCodes.Status400BadRequest;

                    response = ApiResponse<object>.Failure(
                        "Validation failed",
                        validationException.Errors
                            .GroupBy(x => x.PropertyName)
                            .ToDictionary(
                                x => x.Key,
                                x => x.Select(e => e.ErrorMessage)
                            )
                    );

                    break;
                // =====================================
                // CUSTOM APP EXCEPTION
                // =====================================
                case AppException appException:

                    statusCode = appException.StatusCode;

                    response = new
                    {
                        success = false,
                        message = appException.Message,
                        statusCode,
                        traceId,
                        path,
                        method,
                        timestamp = DateTime.UtcNow
                    };

                    break;

                // =====================================
                // DATABASE EXCEPTION
                // =====================================
                case DbUpdateException dbException:

                    statusCode =
                        StatusCodes.Status500InternalServerError;

                    response = env.IsDevelopment()
                        ? new
                        {
                            success = false,

                            message = "Database update failed",

                            statusCode,

                            traceId,

                            path,

                            method,

                            timestamp = DateTime.UtcNow,

                            exception = dbException.GetType().Name,

                            innerException =
                                dbException.InnerException?.Message,

                            details = dbException.Message
                        }
                        : new
                        {
                            success = false,

                            message = "Database error",

                            statusCode,

                            traceId,

                            timestamp = DateTime.UtcNow
                        };

                    break;

                // =====================================
                // GENERAL EXCEPTION
                // =====================================
                default:

                    statusCode =
                        StatusCodes.Status500InternalServerError;

                    response = env.IsDevelopment()
                        ? new
                        {
                            success = false,

                            message = ex.Message,

                            statusCode,

                            traceId,

                            path,

                            method,

                            timestamp = DateTime.UtcNow,

                            exception = ex.GetType().Name,

                            innerException =
                                ex.InnerException?.Message,

                            source = ex.Source,

                            details = ex.Message
                        }
                        : new
                        {
                            success = false,

                            message = "Internal server error",

                            statusCode,

                            traceId,

                            timestamp = DateTime.UtcNow
                        };

                    break;
            }

            context.Response.StatusCode = statusCode;

            var json = JsonSerializer.Serialize(
                response,
                JsonOptions
            );

            await context.Response.WriteAsync(json);
        }
    }
}