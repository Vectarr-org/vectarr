using Microsoft.AspNetCore.Mvc.Filters;

namespace Vectarr.Palooza.Exceptions
{
    public class ErrorValidationExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => int.MaxValue - 10;
        public void OnActionExecuting(ActionExecutingContext context) { }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is ErrorValidationException httpResponseException)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.ObjectResult(httpResponseException.Message)
                {
                    StatusCode = 400
                };

                context.ExceptionHandled = true;
            }
        }
    }
}
