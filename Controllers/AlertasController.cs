using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoWebInventario.Models;

namespace ProyectoWebInventario.Controllers
{
    public class AlertasController : Controller
    {
        private BDBodegasEntities db = new BDBodegasEntities();

        // GET: AlertaReposicions
        public ActionResult Index()
        {
            var alertaReposicions = db.AlertaReposicions.Include(a => a.Producto);
            return View(alertaReposicions.ToList());
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
        public ActionResult Create()
        {
            ViewBag.ProductoIdAlertaReposicion = new SelectList(db.Productoes, "IdProducto", "Nombre");
            return View();
        }

        // POST: AlertaReposicions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
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
