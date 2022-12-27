using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MyServiceDesk.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Autorizacion: AuthorizeAttribute
    {
        private IEnumerable<BE.BEOperacion> operacionesColaborador;
        private readonly MyServiceDesk.Enums.Operacion _operacion;

        public Autorizacion(MyServiceDesk.Enums.Operacion operacion)
        {
            _operacion = operacion;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                if (HttpContext.Current.Session["operaciones"] != null)
                {
                    operacionesColaborador = HttpContext.Current.Session["operaciones"] as IEnumerable<BE.BEOperacion>;
                    if (operacionesColaborador.Where(op => op.IdOperacion == (int)_operacion).FirstOrDefault() == null)
                    {
                        filterContext.Result = new RedirectResult("~/Error/OperacionSinAutorizacion");
                    }
                }
                else
                {
                    operacionesColaborador = new List<BE.BEOperacion>();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                filterContext.Result = new RedirectResult("~/Error/OperacionSinAutorizacion");
            }
        }
    }
}