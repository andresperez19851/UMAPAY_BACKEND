using Newtonsoft.Json;
using System.Net;
using System.Security.Authentication;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;

namespace UmaPay.Api.Helpers
{
    public static class HandleException
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    if (contextFeature != null)
                    {
                        context.Response.StatusCode = (int)GetErrorCode(contextFeature.Error);

                        var errorDetails = new
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message,
                            Path = contextFeature.Path
                        };

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(errorDetails));
                    }
                });
            });
        }

        private static HttpStatusCode GetErrorCode(Exception e)
        {
            return e switch
            {
                ValidationException => HttpStatusCode.BadRequest,
                AuthenticationException => HttpStatusCode.Forbidden,
                NotImplementedException => HttpStatusCode.NotImplemented,
                KeyNotFoundException => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError
            };
        }

    }
}