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

      

        // GET: Categoria
        public ActionResult Index()
        {
            return View();
        }
    }
}