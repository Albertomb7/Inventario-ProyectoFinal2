using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using ProyectoWebInventario.Filters;
using ProyectoWebInventario.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ProyectoWebInventario.Controllers
{
    public class AlertasController : BaseController
    {
        private BDBodegasEntities db = new BDBodegasEntities();

        // GET: AlertaReposicions
        [PermisoAttributes("VerAlertas")]
        public ActionResult Index(string busqueda, int page = 1)
        {
            const int pageSize = 3;
            if (page < 1) page = 1;

            var q = db.AlertaReposicions
                .AsNoTracking()
                .Where(a => a.Activo == true)
                .Include(a => a.Producto);

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                q = q.Where(a =>
                    (a.Producto.Nombre ?? "").Contains(busqueda) ||
                    (a.FechaDeGeneracion != null && a.FechaDeGeneracion.ToString().Contains(busqueda))
                );
            }

            q = q.OrderByDescending(a => (DateTime?)a.FechaDeGeneracion ?? DateTime.MinValue)
                 .ThenByDescending(a => a.IdAlertaReposicion);

            var totalItems = q.Count();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
            if (page > totalPages) page = totalPages;

            var alertas = q.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.Busqueda = busqueda;

            return View(alertas);
        }


        // GET: AlertaReposicions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlertaReposicion alertaReposicion = db.AlertaReposicions.Find(id);
            if (alertaReposicion == null)
            {
                return HttpNotFound();
            }
            return View(alertaReposicion);
        }

        // GET: AlertaReposicions/Create
        [PermisoAttributes("CrearAlertas")]
        public ActionResult Create()
        {
            ViewBag.ProductoIdAlertaReposicion = new SelectList(db.Productoes, "IdProducto", "Nombre");
            return View();
        }

        // POST: AlertaReposicions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [PermisoAttributes("CrearAlertas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdAlertaReposicion,ProductoIdAlertaReposicion,FechaDeGeneracion,NivelActual")] AlertaReposicion alertaReposicion)
        {
            if (ModelState.IsValid)
            {
                db.AlertaReposicions.Add(alertaReposicion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductoIdAlertaReposicion = new SelectList(db.Productoes, "IdProducto", "Nombre", alertaReposicion.ProductoIdAlertaReposicion);
            return View(alertaReposicion);
        }

        // GET: AlertaReposicions/Edit/5

        [PermisoAttributes("EditarAlertas")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlertaReposicion alertaReposicion = db.AlertaReposicions.Find(id);
            if (alertaReposicion == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductoIdAlertaReposicion = new SelectList(db.Productoes, "IdProducto", "Nombre", alertaReposicion.ProductoIdAlertaReposicion);
            return View(alertaReposicion);
        }

        // POST: AlertaReposicions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [PermisoAttributes("EditarAlertas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdAlertaReposicion,ProductoIdAlertaReposicion,FechaDeGeneracion,NivelActual")] AlertaReposicion alertaReposicion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(alertaReposicion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductoIdAlertaReposicion = new SelectList(db.Productoes, "IdProducto", "Nombre", alertaReposicion.ProductoIdAlertaReposicion);
            return View(alertaReposicion);
        }

        // GET: AlertaReposicions/Delete/5
        [PermisoAttributes("EliminarAlertas")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlertaReposicion alertaReposicion = db.AlertaReposicions.Find(id);
            if (alertaReposicion == null)
            {
                return HttpNotFound();
            }
            return View(alertaReposicion);
        }

        // POST: AlertaReposicions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AlertaReposicion alertaReposicion = db.AlertaReposicions.Find(id);
            db.AlertaReposicions.Remove(alertaReposicion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
