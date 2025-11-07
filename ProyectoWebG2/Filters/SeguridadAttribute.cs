using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProyectoWebG2.Filters
{
    public class SeguridadAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;
            var id = http.Session.GetInt32("IdUsuario");
            if (id == null)
            {
                context.Result = new RedirectToActionResult("Login", "Home", null);
            }
            base.OnActionExecuting(context);
        }
    }
}
