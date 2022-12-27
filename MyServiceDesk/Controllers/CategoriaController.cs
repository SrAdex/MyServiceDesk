using MyServiceDesk.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyServiceDesk.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly DA.DAUtilidades gestionUtilidades;
        private readonly DA.DACategoria gestionCategoria;

        public CategoriaController()
        {
            gestionUtilidades= new DA.DAUtilidades();
            gestionCategoria= new DA.DACategoria();
        }

        [Autorizacion(Enums.Operacion.ListadoCategorias)]
        public ActionResult Listado()
        {
            IEnumerable<BE.BECategoria> categorias = gestionUtilidades.ListarCategorias();
            return View(categorias);
        }

        [Autorizacion(Enums.Operacion.ListadoCategorias)]
        public JsonResult ListarSubCategorias(int idCategoria)
        {
            List<BE.BECategoria> subcategorias = new List<BE.BECategoria>();
            subcategorias = gestionUtilidades.ListarSubCategorias(idCategoria);

            return Json(subcategorias, JsonRequestBehavior.AllowGet);
        }

        [Autorizacion(Enums.Operacion.ListadoCategorias)]
        public JsonResult BuscarCategoria(int idCategoria)
        {
            return Json(gestionUtilidades.ListarCategorias().Where(c => c.IdCategoria == idCategoria)
                            .FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }

        [Autorizacion(Enums.Operacion.ListadoCategorias)]
        public JsonResult BuscarSubCategoria(int idSubcategoria, int idCategoriaSuperior)
        {
            return Json(gestionUtilidades.ListarSubCategorias(idCategoriaSuperior).Where(c => c.IdCategoria == idSubcategoria)
                            .FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }

        [Autorizacion(Enums.Operacion.ListadoCategorias)]
        [HttpPost]
        public ActionResult RegistrarCategoria(BE.BECategoria categoria)
        {
            string mensaje = gestionCategoria.RegistrarCategoria(categoria);

            TempData["mensaje"] = mensaje;
            if (mensaje.StartsWith("Error"))
            {
                TempData["tipoAlerta"] = "alert-danger";
            }
            else
            {
                TempData["tipoAlerta"] = "alert-success";
            }

            return RedirectToAction("Listado");
        }

        [Autorizacion(Enums.Operacion.ListadoCategorias)]
        [HttpPost]
        public ActionResult RegistrarSubcategoria(BE.BECategoria categoria)
        {
            string mensaje = gestionCategoria.RegistrarSubCategoria(categoria);

            TempData["mensaje"] = mensaje;
            if (mensaje.StartsWith("Error"))
            {
                TempData["tipoAlerta"] = "alert-danger";
            }
            else
            {
                TempData["tipoAlerta"] = "alert-success";
            }

            return RedirectToAction("Listado");
        }

        [Autorizacion(Enums.Operacion.ListadoCategorias)]
        [HttpPost]
        public ActionResult EditarCategoria(BE.BECategoria categoria)
        {
            string mensaje = gestionCategoria.EditarCategoria(categoria);

            TempData["mensaje"] = mensaje;
            if (mensaje.StartsWith("Error"))
            {
                TempData["tipoAlerta"] = "alert-danger";
            }
            else
            {
                TempData["tipoAlerta"] = "alert-success";
            }

            return RedirectToAction("Listado");
        }

        [Autorizacion(Enums.Operacion.ListadoCategorias)]
        [HttpPost]
        public JsonResult Eliminar(int id = 0)
        {
            string mensaje;
            try
            {
                mensaje = gestionCategoria.EliminarCategoria(id);

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