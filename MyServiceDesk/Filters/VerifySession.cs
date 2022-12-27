using MyServiceDesk.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyServiceDesk.Filters
{
    public class VerifySession: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            BE.BEUsuario usuario = HttpContext.Current.Session["usuario"] as BE.BEUsuario;
            if (usuario == null)
            {
                if (filterContext.Controller is AccountController == false)
                {
                    if (filterContext.Controller is EncuestasController == false)
                    {
                        var urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
                        filterContext.Result = new RedirectResult(urlHelp.Action("Login", "Account", new { rutaOrigen = filterContext.HttpContext.Request.Url.ToString() }));
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}