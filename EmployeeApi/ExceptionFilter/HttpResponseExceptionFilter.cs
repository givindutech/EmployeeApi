using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeApi.ExceptionFilter
{
    public class HttpResponseExceptionFilter : IActionFilter , IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

        }
        public void OnActionExecuted(ActionExecutedContext filterContext) { 
            if(filterContext.Exception is HttpResponseException exception)
            {
                filterContext.Result = new ObjectResult(exception.Message)
                {
                    StatusCode = exception.Status,
                    Value = exception.Message
                };
                filterContext.ExceptionHandled = true;
            }

        }
    }
}
