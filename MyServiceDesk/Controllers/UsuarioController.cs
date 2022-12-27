using MyServiceDesk.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyServiceDesk.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly DA.DAUsuario gestionUsuario;

        public UsuarioController()
        {
            gestionUsuario = new DA.DAUsuario();
        }

        [Autorizacion(operacion: Enums.Operacion.ListadoUsuarios)]
        public ActionResult Listado()
        {
            IEnumerable<BE.ViewModels.UsuarioViewModel> usuarios = gestionUsuario.ListarUsuarios();
            return View(usuarios);
        }

        [Autorizacion(operacion: Enums.Operacion.ListadoUsuarios)]
        public ActionResult Registrar()
        {
            return View(new BE.BEUsuario());
        }

        [Autorizacion(operacion: Enums.Operacion.ListadoUsuarios)]
        [HttpPost]
        public ActionResult Registrar(BE.BEUsuario usuario, List<int> Roles)
        {
            usuario.RolesUsuario = new List<BE.BERol>();
            foreach (var rol in Roles)
            {
                usuario.RolesUsuario.Add(new BE.BERol() { IdRol = rol });
            }

            string mensaje = gestionUsuario.RegistrarUsuario(usuario);
            TempData["mensaje"] = mensaje;
            if (mensaje.StartsWith("Error: ", StringComparison.CurrentCultureIgnoreCase))
                TempData["tipoAlerta"] = "alert alert-danger";
            else
                TempData["tipoAlerta"] = "alert alert-success";
            return RedirectToAction("Listado");
        }

        [Autorizacion(operacion: Enums.Operacion.ListadoUsuarios)]
        public ActionResult Editar(int id)
        {
            return View(gestionUsuario.BuscarUsuario(id));
        }

        public JsonResult ListarDatosUsuario(int id)
        {
            BE.BEUsuario usuario = gestionUsuario.BuscarUsuario(id);
            return Json(usuario, JsonRequestBehavior.AllowGet);
        }

        [Autorizacion(operacion: Enums.Operacion.ListadoUsuarios)]
        [HttpPost]
        public ActionResult Editar(BE.BEUsuario usuario, List<int> roles)
        {
            usuario.RolesUsuario = new List<BE.BERol>();

            foreach (var rol in roles)
            {
                usuario.RolesUsuario.Add(new BE.BERol() { IdRol = rol });
            }

            string mensaje = gestionUsuario.EditarUsuario(usuario);
            TempData["mensaje"] = mensaje;
            if (mensaje.StartsWith("Error: ", StringComparison.CurrentCultureIgnoreCase))
                TempData["tipoAlerta"] = "alert-danger";
            else
                TempData["tipoAlerta"] = "alert-success";
            return RedirectToAction("Listado");
        }

        [Autorizacion(operacion: Enums.Operacion.ListadoUsuarios)]
        [HttpPost]
        public JsonResult Eliminar(int id = 0)
        {
            string mensaje;

            try
            {
                BE.BEUsuario colaborador = gestionUsuario.BuscarUsuario(id);
                mensaje = gestionUsuario.EliminarUsuario(id);

                if (mensaje.StartsWith("Error") || mensaje.StartsWith("No"))
                {
                    return Json(new { success = false, mensaje = mensaje });
                }
                else
                {
                    return Json(new { success = true, mensaje = mensaje });
                }

            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return Json(new { success = false, mensaje = mensaje });
            }
        }
    }
}