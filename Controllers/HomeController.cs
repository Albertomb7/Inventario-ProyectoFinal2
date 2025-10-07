using ProyectoWebInventario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoWebInventario.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            // 1. Crear una instancia 
            var viewModel = new DashboardViewModel();

            // 2. Consultar la base de datos 
            viewModel.TotalProductos = db.Productoes.Count();
            viewModel.TotalUbicaciones = db.Ubicacions.Count();
            viewModel.TotalAlertas = db.AlertaReposicions.Count(a => a.Activo == true);

            // Toma los 5 productos más recientes 
            viewModel.UltimosProductos = db.Productoes.OrderByDescending(p => p.IdProducto).Take(5).ToList();

            // 3. Enviar el ViewModel a la vista
            return View(viewModel);
        }
        private BDBodegasEntities db = new BDBodegasEntities();
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}