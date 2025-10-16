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
    public class InventarioController : BaseController
    {
        private BDBodegasEntities db = new BDBodegasEntities();

        // GET: Inventario
        [PermisoAttributes("VerInventario")]
        public ActionResult Index()
        {
            var inventarios = db.Inventarios.Include(i => i.Producto).Include(i => i.Ubicacion);
            return View(inventarios.ToList());
        }

        // GET: Inventario/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventario inventario = db.Inventarios.Find(id);
            if (inventario == null)
            {
                return HttpNotFound();
            }
            return View(inventario);
        }

        // GET: Inventario/Create
        [PermisoAttributes("CrearInventario")]
        public ActionResult Create()
        {
            ViewBag.ProductoIdInventario = new SelectList(db.Productoes, "IdProducto", "Nombre");
            ViewBag.UbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo");
            return View();
        }

        // POST: Inventario/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [PermisoAttributes("CrearInventario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdInventario,ProductoIdInventario,UbicacionId,Stock")] Inventario inventario)
        {
            if (ModelState.IsValid)
            {
                db.Inventarios.Add(inventario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductoIdInventario = new SelectList(db.Productoes, "IdProducto", "Nombre", inventario.ProductoIdInventario);
            ViewBag.UbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", inventario.UbicacionId);
            return View(inventario);
        }

        // GET: Inventario/Edit/5
        [PermisoAttributes("EditarInventario")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventario inventario = db.Inventarios.Find(id);
            if (inventario == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductoIdInventario = new SelectList(db.Productoes, "IdProducto", "Nombre", inventario.ProductoIdInventario);
            ViewBag.UbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", inventario.UbicacionId);
            return View(inventario);
        }

        // POST: Inventario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [PermisoAttributes("EditarInventario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdInventario,ProductoIdInventario,UbicacionId,Stock")] Inventario inventario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductoIdInventario = new SelectList(db.Productoes, "IdProducto", "Nombre", inventario.ProductoIdInventario);
            ViewBag.UbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", inventario.UbicacionId);
            return View(inventario);
        }

        // GET: Inventario/Delete/5
        [PermisoAttributes("EliminarInventario")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventario inventario = db.Inventarios.Find(id);
            if (inventario == null)
            {
                return HttpNotFound();
            }
            return View(inventario);
        }

        // POST: Inventario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inventario inventario = db.Inventarios.Find(id);
            db.Inventarios.Remove(inventario);
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
