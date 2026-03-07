using System.Net;
using Microsoft.AspNetCore.WebUtilities;

namespace EmployeeApi.ExceptionHandler
{
    public class ExceptionMiddlewareCustom
    {
        public readonly RequestDelegate _next;
        public ExceptionMiddlewareCustom(RequestDelegate next) 
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                 await HandleExceptionAsync(httpContext, ex);
            }
        }
        private  async Task HandleExceptionAsync(HttpContext context,Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(new ErrorDetails()
            {
                message = exception.Message,
                statusCode = context.Response.StatusCode
            }.ToString());
        }
    }
}
