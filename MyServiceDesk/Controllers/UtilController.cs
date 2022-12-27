using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyServiceDesk.Controllers
{
    public class UtilController : Controller
    {
        private readonly DA.DAUtilidades gestionUtil = new DA.DAUtilidades();

        public JsonResult ListarSubCategorias(int idCategoriaSuperior)
        {
            List<BE.BECategoria> subCategorias = gestionUtil.ListarSubCategorias(idCategoriaSuperior).Where(sc => sc.EsActivo).ToList();
            return Json(subCategorias, JsonRequestBehavior.AllowGet);
        }
    }
}