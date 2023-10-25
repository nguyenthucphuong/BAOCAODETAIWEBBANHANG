using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SaleApi.Filters
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userName = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;

            if (userName == null)
            {
                context.Result = new UnauthorizedResult();
            }
            else
            {
                context.HttpContext.Items["UserName"] = userName;
            }
        }
    }
}
