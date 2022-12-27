using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MyServiceDesk.Enums;
using DA;
using BE;

namespace MyServiceDesk.Controllers
{
    public class AccountController : Controller
    {
        public readonly DA.DAAccount gestionAccount;
        public readonly DA.DAUsuario gestionUsuario;
        private readonly DA.DAUtilidades gestUtilidades;

        public AccountController()
        {
            gestionUsuario = new DA.DAUsuario();
            gestionAccount = new DA.DAAccount(); ;
        }

        public async Task<ActionResult> Login(string rutaOrigen = "")
        {
            ViewBag.rutaOrigen = rutaOrigen;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string correo, string clave, string rutaOrigen = "", string token = "")
        {
            //if (gestionAccount.EstaEnActiveDirectory(correo, clave))
            //{
            BE.BEUsuario usuario = gestionAccount.EstaEnBaseDeDatos(correo);
            if (usuario != null)
            {
                usuario.RolesUsuario = gestionUsuario.ListarRolesUsuario(usuario.IdUsuario);
                gestionAccount.IniciarSesion(usuario);

                if (!string.IsNullOrEmpty(rutaOrigen))
                {
                    return Redirect(rutaOrigen);
                }
                else
                {
                    if (usuario.RolesUsuario.Where(r => r.IdRol == (int)RolesUsuario.Administrador).FirstOrDefault() != null)
                    {
                        return RedirectToAction("Listado", "Ticket");
                    }
                    else
                    {
                        return RedirectToAction("MisTickets", "Ticket");
                    }
                }

            }
            else
            {
                TempData["mensaje"] = "Usuario y/o contraseña errónea";
                //return View();
                return RedirectToAction("Login", "Account");
            }
            //}
            //else
            //{
            //    TempData["mensaje"] = "Credenciales Incorrectas";
            //    return RedirectToAction("Login", "Account");
            //}
        }

        public async Task<ActionResult> Logout()
        {
            gestionAccount.CerrarSesion();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public void ExtenderTiempoSesion()
        {
            Session.Timeout += 20;
        }
    }
}
