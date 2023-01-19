using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;

namespace MyServiceDesk.Controllers
{
    public class CorreoController : Controller
    {
        private readonly DA.DACorreo gestionCorreo;
        private readonly DA.DATicket gestionTicket;
        private readonly DA.DAUtilidades gestUtilidades;

        public CorreoController()
        {
            gestionCorreo = new DA.DACorreo();
            gestionTicket = new DA.DATicket();
            gestUtilidades = new DA.DAUtilidades();
        }

        public ActionResult AbrirCorreos(int id = 0)
        {
            IEnumerable<BE.BECorreo> correos = new List<BE.BECorreo>();
            correos = gestionCorreo.ListarCorreosPorTicket(id);
            ViewBag.estados = gestUtilidades.ListarEstados();
            ViewBag.usuarios = gestUtilidades.ListarUsuarios();
            //ViewBag.listarTipoDeContenido = gestUtilidades.ListarTipoDeContenidoPorID();

            foreach (var correo in correos)
            {
                // Lee los archivos adjuntos
                List<BE.BEAdjunto> adjuntos = new List<BE.BEAdjunto>();
                adjuntos = gestUtilidades.ListarAdjuntosCorreo(correo.CodigoCorreo);
                correo.Adjuntos = adjuntos;

                // Genera un nuevo html
                HtmlDocument documentoHtml = new HtmlDocument();

                // Carga el mensaje en un html
                documentoHtml.LoadHtml(correo.MensajeCorreo);

                // Obtiene las etiquetas HTML
                var imagenesHtml = documentoHtml.DocumentNode.SelectNodes("//img");
                var imagenesHtmlAdjuntas = adjuntos.Where(a => a.RutaAdjunto.Contains("image")).ToList();

                if (imagenesHtml != null)
                {
                    if (imagenesHtml.Count > 0 && imagenesHtmlAdjuntas.Count > 0)
                    {
                        foreach (var imagenHtml in imagenesHtml)
                        {
                            foreach (var imagenHtmlAdjunta in imagenesHtmlAdjuntas)
                            {
                                if (imagenHtmlAdjunta.CidAdjunto.Length > 0)
                                {
                                    string cidAdjunto = "";

                                    cidAdjunto = imagenHtmlAdjunta.CidAdjunto.Substring(1, imagenHtmlAdjunta.CidAdjunto.Length - 2);

                                    if (imagenHtml.Attributes["src"].Value.Contains(cidAdjunto))
                                    {
                                        imagenHtml.Attributes.Remove("src");
                                        imagenHtml.Attributes.Add("src", Url.Content(imagenHtmlAdjunta.RutaAdjunto));
                                        imagenHtml.Attributes.Add("class", "img-fluid");
                                    }
                                }
                            }
                        }
                    }
                }

                // Guarda el html renderizado
                correo.Render = documentoHtml;
            }

            ViewBag.detalleTicket = gestionTicket.DetallarTicket(id);

            return View(correos);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult GetDatosRespuesta(string codigoCorreo, int idTicket)
        {
            BE.BECorreo correo = gestionCorreo.BuscarCorreo(codigoCorreo, idTicket);
            return Json(correo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Responder(BE.BECorreo correo, string copias, string para, HttpPostedFileBase[] adjuntos, BE.BETicket ticket)
        {

            string mensaje = gestionTicket.ActualizarPropietario(ticket.IdTicket, ticket.Propietario);

            if (!mensaje.StartsWith("Error")) // Si no hubo error
            {
                mensaje = gestionTicket.ActualizarEstadoTicket(ticket.IdTicket, ticket.IdEstado);

                if (!mensaje.StartsWith("Error"))
                {
                    mensaje = gestionCorreo.Responder(correo, copias, para, adjuntos);

                    if (!mensaje.StartsWith("Error"))
                    {
                        TempData["mensaje"] = mensaje;
                        TempData["alerta"] = "alert alert-success";
                    }
                    else
                    {
                        TempData["mensaje"] = mensaje;
                        TempData["alerta"] = "alert alert-danger";
                    }
                }
                else
                {
                    TempData["mensaje"] = mensaje;
                    TempData["alerta"] = "alert alert-danger";
                }
            }
            else
            {
                TempData["mensaje"] = mensaje;
                TempData["alerta"] = "alert alert-danger";
            }

            return RedirectToAction("AbrirCorreos", "Correo", new { id = correo.IdTicket });
        }

        [HttpPost]
        public ActionResult EditarTicket(BE.BETicket ticket)
        {
            string mensaje = gestionTicket.ActualizarPropietario(ticket.IdTicket, ticket.Propietario);


            if (!mensaje.StartsWith("Error"))
            {
                mensaje = gestionTicket.ActualizarEstadoTicket(ticket.IdTicket, ticket.IdEstado);
                if (!mensaje.StartsWith("Error"))
                {
                    TempData["mensaje"] = "Cambios realizados satisfactoriamente";
                    TempData["alerta"] = "alert alert-success";
                }
                else
                {
                    TempData["mensaje"] = mensaje;
                    TempData["alerta"] = "alert alert-danger";
                }
            }
            else
            {
                TempData["mensaje"] = mensaje;
                TempData["alerta"] = "alert alert-danger";
            }

            return RedirectToAction("AbrirCorreos", "Correo", new { id = ticket.IdTicket });
        }
    }
}