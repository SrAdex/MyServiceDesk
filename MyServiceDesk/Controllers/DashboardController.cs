using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyServiceDesk.Enums;

namespace MyServiceDesk.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DA.DADashboard gestionDashboard;
        private readonly DA.DAUsuario gestionUsuario;
        private readonly DA.DAUtilidades gestUtilidades;

        public DashboardController()
        {
            gestionDashboard = new DA.DADashboard();
            gestionUsuario = new DA.DAUsuario();
            gestUtilidades = new DA.DAUtilidades();
        }

        public ActionResult Dashboard()
        {
            List<BE.BEDashboard> listaDashboard_TicketsXUsuario = new List<BE.BEDashboard>();
            listaDashboard_TicketsXUsuario = gestionDashboard.Dashboard_TicketsXUsuario();
            ViewBag.TicketsXUsuario = listaDashboard_TicketsXUsuario;
            //
            ViewBag.responsables = gestionUsuario.ListarUsuariosRol((int)RolesUsuario.Responsable);
            ViewBag.estados = gestUtilidades.ListarEstados();
            //
            int DashboardTotalDeTickets = gestionDashboard.DashboardTotalDeTickets();
            ViewBag.DashboardTotalDeTickets = DashboardTotalDeTickets;
            int DashboardTotalEncuestas = gestionDashboard.DashboardTotalEncuestas();
            ViewBag.DashboardTotalEncuestas = DashboardTotalEncuestas;
            int DashboardTotalTicketsRecibidos = gestionDashboard.DashboardTotalTicketsRecibidos();
            ViewBag.DashboardTotalTicketsRecibidos = DashboardTotalTicketsRecibidos;
            int DashboardTotalUsuariosActivos = gestionDashboard.DashboardTotalUsuariosActivos();
            ViewBag.DashboardTotalUsuariosActivos = DashboardTotalUsuariosActivos;

            return View();
        }
        [HttpGet]
        public JsonResult Reporte1()
        {
            List<BE.BEDashboard> listaDashboard_TicketsXFechaGen = new List<BE.BEDashboard>();
            listaDashboard_TicketsXFechaGen = gestionDashboard.Dashboard_TicketsXFechaGen();
            return Json(listaDashboard_TicketsXFechaGen, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Reporte2()
        {
            List<BE.BEDashboard> listaDashboard_TicketsXEstado = new List<BE.BEDashboard>();
            listaDashboard_TicketsXEstado = gestionDashboard.Dashboard_TicketsXEstado();
            return Json(listaDashboard_TicketsXEstado, JsonRequestBehavior.AllowGet);
        }
    }
}