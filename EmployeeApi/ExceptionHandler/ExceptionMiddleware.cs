using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace EmployeeApi.ExceptionHandler
{
    public static class ExceptionMiddleware
    {
        public static void configureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddlewareCustom>();
        }
        public static void ConfigureExceptionHandler(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode=(int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType="application/json";
                    var contextFeature=context.Features.Get<ExceptionHandlerFeature>();
                    if (contextFeature != null) {
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            statusCode = context.Response.StatusCode,
                            message = "Internal Server Error",
                        }.ToString());
                    }
                });
            });
        }
    }
}
