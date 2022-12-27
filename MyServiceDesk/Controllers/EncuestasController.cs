using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyServiceDesk.Controllers
{
    public class EncuestasController : Controller
    {
        private readonly DA.DATicket _gestionTicket;
        private readonly DA.DAEncuestas _gestionEncuestas;

        public EncuestasController()
        {
            _gestionTicket = new DA.DATicket();
            _gestionEncuestas = new DA.DAEncuestas();
        }

        public ActionResult Registrar(string guidTicket)
        {
            BE.ViewModels.DetalleTicketViewModel detalleTicket = _gestionTicket.GetDetalleByGuid(guidTicket);

            if (_gestionEncuestas.ExisteEncuesta(detalleTicket.IdTicket))
            {
                return RedirectToAction("EncuestaExiste");
            }

            return View(detalleTicket);
        }

        [HttpPost]
        public ActionResult Registrar(BE.BEEncuesta encuesta)
        {
            string mensaje = _gestionEncuestas.RegistrarEncuesta(encuesta);
            TempData["mensaje"] = mensaje;

            if (!mensaje.StartsWith("Error"))
            {
                return RedirectToAction("Gracias");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Gracias()
        {
            return View();
        }

        public ActionResult EncuestaExiste()
        {
            return View();
        }

        public JsonResult DetalleEncuesta(int idTicket)
        {
            return Json(_gestionEncuestas.GetDeatlle(idTicket), JsonRequestBehavior.AllowGet);
        }
    }
}