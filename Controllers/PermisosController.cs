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
    public class PermisosController : Controller
    {
        private BDBodegasEntities db = new BDBodegasEntities();

        // GET: Permisos
        [PermisoAttributes("VerPermiso")]
        public ActionResult Index()
        {
            return View(db.Permisoes.ToList());
        }

        // GET: Permisos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permiso permiso = db.Permisoes.Find(id);
            if (permiso == null)
            {
                return HttpNotFound();
            }
            return View(permiso);
        }

        // GET: Permisos/Create
        [PermisoAttributes("CrearPermiso")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Permisos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [PermisoAttributes("CrearPermiso")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdPermiso,NombrePermiso,Descripcion")] Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                db.Permisoes.Add(permiso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(permiso);
        }

        // GET: Permisos/Edit/5
        [PermisoAttributes("EditarPermiso")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permiso permiso = db.Permisoes.Find(id);
            if (permiso == null)
            {
                return HttpNotFound();
            }
            return View(permiso);
        }

        // POST: Permisos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [PermisoAttributes("VerPermiso")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdPermiso,NombrePermiso,Descripcion")] Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(permiso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(permiso);
        }

        // GET: Permisos/Delete/5
        [PermisoAttributes("EliminarPermiso")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permiso permiso = db.Permisoes.Find(id);
            if (permiso == null)
            {
                return HttpNotFound();
            }
            return View(permiso);
        }

        // POST: Permisos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Permiso permiso = db.Permisoes.Find(id);
            db.Permisoes.Remove(permiso);
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
