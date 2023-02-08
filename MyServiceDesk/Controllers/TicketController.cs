using MyServiceDesk.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyServiceDesk.Enums;
using ClosedXML.Excel;

namespace MyServiceDesk.Controllers
{
    public class TicketController : Controller
    {
        private readonly DA.DATicket gestionTicket;
        private readonly DA.DAUsuario gestionUsuario;
        private readonly DA.DAUtilidades gestUtilidades;

        public TicketController()
        {
            gestionTicket = new DA.DATicket();
            gestionUsuario = new DA.DAUsuario();
            gestUtilidades = new DA.DAUtilidades();
        }

        [Autorizacion(operacion: Enums.Operacion.ListadoTickets)]
        public ActionResult Listado()
        {
            IEnumerable<BE.ViewModels.TicketViewModel> tickets = new List<BE.ViewModels.TicketViewModel>();

            List<int> filtroEstado = Session["filtroEstadoTicketGeneral"] as List<int>;
            List<int> filtroResponsable = Session["filtroResponsableTicket"] as List<int>;

            string condicionEstado = "";
            string condicionResponsable = "";

            if (filtroEstado != null)
            {
                condicionEstado = string.Join(",", filtroEstado);
            }

            if (filtroResponsable != null)
            {
                condicionResponsable = string.Join(",", filtroResponsable);
            }

            if (condicionEstado.Length > 0 || condicionResponsable.Length > 0)
            {
                tickets = gestionTicket.FiltrarTickets(condicionEstado, condicionResponsable);
            }
            else
            {
                tickets = gestionTicket.ListarTickets();
            }

            ViewBag.filtroEstado = (filtroEstado ?? new List<int>());
            ViewBag.filtroResponsable = (filtroResponsable ?? new List<int>());

            ViewBag.responsables = gestionUsuario.ListarUsuariosRol((int)RolesUsuario.Responsable);
            ViewBag.estados = gestUtilidades.ListarEstados();
            /*Nuevos ViewBag
            ViewBag.etapas = gestUtilidades.ListarEtapa();
            ViewBag.tipoDeContenidos = gestUtilidades.ListarTipoDeContenidos();
            ViewBag.prioridades = gestUtilidades.ListarPrioridades();
            */
            ViewBag.listarTipoDeContenido = gestUtilidades.ListarTipoDeContenidoPorID();
            ViewBag.tipoDeContenidos = gestUtilidades.ListarTipoDeContenidos();
            ViewBag.categorias = gestUtilidades.ListarCategorias().Where(c => c.EsActivo).ToList();
            BE.BEUsuario usuario = Session["usuario"] == null ? new BE.BEUsuario() : Session["usuario"] as BE.BEUsuario;
            ViewBag.esAdmin = usuario.RolesUsuario.FindIndex(r => r.NombreRol == "Administrador") >= 0;
            return View(tickets);
        }

        [Autorizacion(operacion: Enums.Operacion.MisTickets)]
        public ActionResult MisTickets()
        {
            // Obtiene al usuario en sesión -> Si es nulo, lo inicializa
            BE.BEUsuario usuario = Session["usuario"] == null ? new BE.BEUsuario() : Session["usuario"] as BE.BEUsuario;

            IEnumerable<BE.ViewModels.TicketViewModel> tickets = new List<BE.ViewModels.TicketViewModel>();

            List<int> filtroEstado = Session["filtroEstadoTicketPersonal"] as List<int>;

            string condicionEstado = "";
            string condicionResponsable = usuario.IdUsuario + "";

            if (filtroEstado != null)
            {
                foreach (var estado in filtroEstado)
                {
                    condicionEstado += (estado + ",");
                }
            }

            if (condicionEstado.Length > 0 || condicionResponsable.Length > 0)
            {
                tickets = gestionTicket.FiltrarMisTickets(condicionEstado, usuario.IdUsuario);
            }
            else
            {
                tickets = gestionTicket.ListarTicketsPorResponsable(usuario.IdUsuario);
            }

            ViewBag.filtroEstado = (filtroEstado ?? new List<int>());
            ViewBag.responsables = gestionUsuario.ListarUsuariosRol((int)RolesUsuario.Responsable);
            ViewBag.estados = gestUtilidades.ListarEstados();
            ViewBag.categorias = gestUtilidades.ListarCategorias().Where(c => c.EsActivo).ToList();
            /*Nuevos
            ViewBag.listarTipoDeContenido = gestUtilidades.ListarTipoDeContenidoPorID();
            ViewBag.prioridades = gestUtilidades.ListarPrioridades();
            ViewBag.etapas = gestUtilidades.ListarEtapa();
            */
            ViewBag.esAdmin = usuario.RolesUsuario.FindIndex(r => r.NombreRol == "Administrador") >= 0;

            return View(tickets);
        }

        public ActionResult AsignarTicket(int idTicket, int idUsuarioResponsable = 0, int idCategoria = 0, int idSubCategoria = 0, int IdEstado = 1)
        {
            string mensaje = gestionTicket.AsignarTicket(idTicket, idUsuarioResponsable, idCategoria, idSubCategoria, IdEstado);

            TempData["mensaje"] = mensaje;

            if (mensaje.StartsWith("Error: ", StringComparison.CurrentCultureIgnoreCase))
                TempData["tipoAlerta"] = "alert-danger";
            else
                TempData["tipoAlerta"] = "alert-success";

            return Redirect(Request.UrlReferrer.ToString());
        }

        [Autorizacion(operacion: Enums.Operacion.AsignarTicket)]
        public ActionResult GenerarFlujo(int idTicket, int idUsuarioResponsable, string tema, int[] tipoDeContenido, string descripcion, DateTime fechaEntrega)
        {
            int numPiezas = gestionTicket.GenerarTipoDeContenidoTicket(idTicket, tipoDeContenido);
            var usuario = Session["usuario"] as BE.BEUsuario;
            gestionTicket.UsuarioResponsableDeAsignación(usuario, idTicket);
            string mensaje = gestionTicket.GenerarFlujo(idTicket, idUsuarioResponsable, tema, tipoDeContenido, descripcion, fechaEntrega);
            //string mensaje = gestionTicket.AsignarTicket(idTicket, idUsuarioResponsable, idSubCategoria);

            TempData["mensaje"] = mensaje;

            //if (mensaje.StartsWith("Error: ", StringComparison.CurrentCultureIgnoreCase))
            //    TempData["tipoAlerta"] = "alert-danger";
            //else
            //    TempData["tipoAlerta"] = "alert-success";

            return Redirect(Request.UrlReferrer.ToString());
        }

        /*
        public ActionResult EnviarRespuestaDeRechazo(int idTicket, string textoRechazo)
        {
            string mensaje = gestionTicket.EliminarPedido(idTicket, textoRechazo);
            string respuesta = "Solicitud eliminada satisfactoriamente";
            TempData["mensaje"] = respuesta;
            string respuesta = gestionTicket.EliminarPedido(idTicket, mensaje);
            string mensaje = gestionTicket.AsignarTicket(idTicket, idUsuarioResponsable, idSubCategoria);

            TempData["mensaje"] = mensaje;

            if (mensaje.StartsWith("Error: ", StringComparison.CurrentCultureIgnoreCase))
                TempData["tipoAlerta"] = "alert-danger";
            else
                TempData["tipoAlerta"] = "alert-success";

            return Redirect(Request.UrlReferrer.ToString());
        }
        */
        public JsonResult ListarDatosAsignacion(int idTicket)
        {
            BE.BETicket ticket = gestionTicket.ListarDatosAsignacion(idTicket);
            return Json(ticket, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EstablecerFiltrosGeneral(List<int> estados, List<int> responsables)
        {

            Session["filtroEstadoTicketGeneral"] = estados == null ? new List<int>() : estados;
            Session["filtroResponsableTicket"] = responsables == null ? new List<int>() : responsables;

            return RedirectToAction("Listado");

        }

        public ActionResult EstablecerFiltrosPersonal(List<int> estados)
        {

            Session["filtroEstadoTicketPersonal"] = estados == null ? new List<int>() : estados;

            return RedirectToAction("MisTickets");

        }

        [Autorizacion(operacion: Enums.Operacion.ActualizarEstadoTicket)]
        public JsonResult ActualizarEstadoTicket(int idTicket, int idEstado, int IdCategoria, int IdSubcategoria)
        {
            string mensaje = gestionTicket.ActualizarEstadoTicket(idTicket, idEstado, IdCategoria, IdSubcategoria);
            string rpt = gestionTicket.EnviarActualizacionDeEstadoTicket(idTicket);

            if (mensaje.StartsWith("Error"))
            {
                TempData["mensaje"] = "no";
                return Json(new { mensaje = mensaje, success = false });
            }
            else
            {
                TempData["mensaje"] = "si";
                return Json(new { mensaje = mensaje, success = true });
            }

        }

        public JsonResult ActualizarFechaTicket(int idTicket, DateTime fechaEntrega)
        {
            string mensaje = gestionTicket.ActualizarFechaTicket(idTicket, fechaEntrega);
            string mensaje2 = gestionTicket.ActualizarPrioridad(idTicket, fechaEntrega);
            string rpt = gestionTicket.EnviarActualizacionDeEstadoTicket(idTicket);

            if (mensaje.StartsWith("Error"))
            {
                TempData["mensaje"] = "Error al actualizar fecha";
                return Json(new { mensaje = mensaje, success = false });
            }
            else
            {
                TempData["mensaje"] = "Fecha actualizada correctamente.";
                return Json(new { mensaje = mensaje, success = true });
            }
        }

        [Autorizacion(operacion: Enums.Operacion.ReporteGeneralTickets)]
        public FileResult GenerarReporteTicketsGeneral()
        {
            DataTable dt = new DataTable("Data");
            dt.Columns.AddRange(new DataColumn[11] {     new DataColumn("Asunto"),
                                                        new DataColumn("Fecha Generación"),
                                                        new DataColumn("Responsable"),
                                                        new DataColumn("Fecha Asignación"),
                                                        new DataColumn("Fecha Entrega"),
                                                        new DataColumn("Fecha Cierre"),
                                                        new DataColumn("Etapa"),
                                                        new DataColumn("Prioridad"),
                                                        new DataColumn("Estado"),
                                                        new DataColumn("Tema"),
                                                        new DataColumn("Tipo de Contenido")
            });

            IEnumerable<BE.ViewModels.TicketViewModel> tickets = new List<BE.ViewModels.TicketViewModel>();

            List<int> filtroEstado = Session["filtroEstadoTicketGeneral"] as List<int>;
            List<int> filtroResponsable = Session["filtroResponsableTicket"] as List<int>;

            string condicionEstado = "";
            string condicionResponsable = "";

            if (filtroEstado != null)
            {
                foreach (var estado in filtroEstado)
                {
                    condicionEstado += (estado + ",");
                }
            }

            if (filtroResponsable != null)
            {
                foreach (var responsable in filtroResponsable)
                {
                    condicionResponsable += (responsable + ",");
                }
            }

            if (condicionEstado.Length > 0 || condicionResponsable.Length > 0)
            {
                tickets = gestionTicket.FiltrarTickets(condicionEstado, condicionResponsable);
            }
            else
            {
                tickets = gestionTicket.ListarTickets();
            }

            List<BE.ViewModels.DetalleTicketViewModel> detallesTicket = new List<BE.ViewModels.DetalleTicketViewModel>();

            foreach (var ticket in tickets)
            {
                BE.ViewModels.DetalleTicketViewModel detalle = gestionTicket.DetallarTicket(ticket.IdTicket);
                detallesTicket.Add(detalle);
            }

            var listarTipoDeContenido = gestUtilidades.ListarTipoDeContenidoPorID();
            List<string> lst = new List<string>();
            string fecha;
            foreach (var detalle in detallesTicket)
            {
                foreach (var tipoContenido in listarTipoDeContenido)
                {
                    if (detalle.IdTicket == tipoContenido.id_ticket)
                    {
                        string contenido = tipoContenido.tipocontenidoDesc;
                        lst.Add(contenido);
                    }

                }
                string combinedString = string.Join(",", lst);
                if (Convert.ToDateTime(detalle.fechaEntrega).ToString("dd-MM-yyyy") == "01-01-0001")
                {
                    fecha = "";
                }
                else
                {
                    fecha = Convert.ToDateTime(detalle.fechaEntrega).ToString("dd-MM-yyyy");
                }
                dt.Rows.Add(detalle.AsuntoTicket, detalle.FechaGeneracion, detalle.UsuarioResponsable, detalle.FechaAsignación, fecha,
                    detalle.FechaCierre, detalle.EstadoTicket, detalle.prioridad, detalle.etapa, detalle.tema, combinedString);
                lst.Clear();
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                var sheet = wb.Worksheets.Add(dt, "Reporte General de Tickets");
                sheet.Columns("A", "J").AdjustToContents();
                using (MemoryStream stream = new MemoryStream()) //using System.IO;  
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_General_Tickets.xlsx");
                }
            }
        }

        [Autorizacion(operacion: Enums.Operacion.ReportePersonalTickets)]
        public FileResult GenerarReporteTicketsPersonal()
        {
            DataTable dt = new DataTable("Data");
            dt.Columns.AddRange(new DataColumn[11] {     new DataColumn("Asunto"),
                                                        new DataColumn("Fecha Generación"),
                                                        new DataColumn("Responsable"),
                                                        new DataColumn("Fecha Asignación"),
                                                        new DataColumn("Fecha Entrega"),
                                                        new DataColumn("Fecha Cierre"),
                                                        new DataColumn("Etapa"),
                                                        new DataColumn("Prioridad"),
                                                        new DataColumn("Estado"),
                                                        new DataColumn("Tema"),
                                                        new DataColumn("Tipo de Contenido")
            });

            // Obtiene al usuario en sesión -> Si es nulo, lo inicializa
            BE.BEUsuario usuario = Session["usuario"] == null ? new BE.BEUsuario() : Session["usuario"] as BE.BEUsuario;

            IEnumerable<BE.ViewModels.TicketViewModel> tickets = new List<BE.ViewModels.TicketViewModel>();

            List<int> filtroEstado = Session["filtroEstadoTicketPersonal"] as List<int>;

            string condicionEstado = "";
            string condicionResponsable = usuario.IdUsuario + "";

            if (filtroEstado != null)
            {
                foreach (var estado in filtroEstado)
                {
                    condicionEstado += (estado + ",");
                }
            }

            if (condicionEstado.Length > 0 || condicionResponsable.Length > 0)
            {
                tickets = gestionTicket.FiltrarMisTickets(condicionEstado, usuario.IdUsuario);
            }
            else
            {
                tickets = gestionTicket.ListarTicketsPorResponsable(usuario.IdUsuario);
            }

            List<BE.ViewModels.DetalleTicketViewModel> detallesTicket = new List<BE.ViewModels.DetalleTicketViewModel>();

            foreach (var ticket in tickets)
            {
                BE.ViewModels.DetalleTicketViewModel detalle = gestionTicket.DetallarTicket(ticket.IdTicket);
                detallesTicket.Add(detalle);
            }

            var listarTipoDeContenido = gestUtilidades.ListarTipoDeContenidoPorID();
            List<string> lst = new List<string>();
            string fecha;
            foreach (var detalle in detallesTicket)
            {
                foreach (var tipoContenido in listarTipoDeContenido)
                {
                    if (detalle.IdTicket == tipoContenido.id_ticket)
                    {
                        string contenido = tipoContenido.tipocontenidoDesc;
                        lst.Add(contenido);
                    }

                }
                string combinedString = string.Join(",", lst);
                if (Convert.ToDateTime(detalle.fechaEntrega).ToString("dd-MM-yyyy") == "01-01-0001")
                {
                    fecha = "-";
                }
                else
                {
                    fecha = Convert.ToDateTime(detalle.fechaEntrega).ToString("dd-MM-yyyy");
                }
                dt.Rows.Add(detalle.AsuntoTicket, detalle.FechaGeneracion, detalle.UsuarioResponsable, detalle.FechaAsignación, fecha,
                    detalle.FechaCierre, detalle.EstadoTicket, detalle.prioridad, detalle.etapa, detalle.tema, combinedString);
                lst.Clear();
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                var sheet = wb.Worksheets.Add(dt, "Reporte Personal de Tickets");
                sheet.Columns("A", "J").AdjustToContents();
                using (MemoryStream stream = new MemoryStream()) //using System.IO;  
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Personal_Tickets.xlsx");
                }
            }
        }
    }
}