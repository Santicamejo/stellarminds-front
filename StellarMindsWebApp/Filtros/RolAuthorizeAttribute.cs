using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StellarMindsWebApp.Filtros
{
    public class RolAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string[] _rolesPermitidos;

        public RolAuthorizeAttribute(string[] rolesPermitidos)
        {
            _rolesPermitidos = rolesPermitidos;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string? rol = context.HttpContext.Session.GetString("UsuarioLogeadoRol");

            if (string.IsNullOrWhiteSpace(rol))
            {
                context.Result = new RedirectToActionResult("Login", "Usuario", null);
                return;
            }

            if (!_rolesPermitidos.Contains(rol))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}