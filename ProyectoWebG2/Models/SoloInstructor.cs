using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProyectoWebG2.Models
{
    public class SoloInstructor : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            var isAuth = session.GetString("IsAuth") == "1";
            var rol = session.GetInt32("Rol");

            if (!isAuth || rol != 3)
            {
                if (context.Controller is Controller controller)
                {
                    controller.TempData["Error"] = "Acceso restringido a instructores.";
                }
                context.Result = new RedirectToActionResult("Login", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
