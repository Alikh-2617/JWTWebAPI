using JWTWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JWTWebAPI.FilterAttribut
{
    public class CheckCookiesFilterAttribut : IActionFilter   // Global filter have to inject from Controller Options Service 
    {

        private readonly IAuthService _service;

        public CheckCookiesFilterAttribut()
        {
            
        }

        public CheckCookiesFilterAttribut(IAuthService authService)
        {

            _service = authService;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // can check Token here too 
            var cookie = context.HttpContext.Request.Cookies["Ref-Cookie"];
            if (!_service.VarifyCookie(cookie!))
            {
                context.Result = new UnauthorizedObjectResult("You can sign in again !");
            }
        }
    }
}
