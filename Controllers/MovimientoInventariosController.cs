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
    public class MovimientoInventariosController : Controller
    {
        private BDBodegasEntities db = new BDBodegasEntities();

        public ActionResult Index()
        {
            var movimientoInventarios = db.MovimientoInventarios.Include(m => m.Producto).Include(m => m.Ubicacion).Include(m => m.Ubicacion1).Include(m => m.Usuario);
            return View(movimientoInventarios.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            MovimientoInventario movimientoInventario = db.MovimientoInventarios.Find(id);
            if (movimientoInventario == null) return HttpNotFound();
            return View(movimientoInventario);
        }

        public ActionResult Create()
        {
            ViewBag.ProductoId = new SelectList(db.Productoes, "IdProducto", "Nombre");
            ViewBag.DesdeUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo");
            ViewBag.HaciaUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo");
            ViewBag.UsuarioId = new SelectList(db.Usuarios, "IdUsuario", "NombreUsuario");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdMovimientoInventario,Fecha,ProductoId,DesdeUbicacionId,HaciaUbicacionId,Cantidad,UsuarioId,Observacion,TipoMovimiento")] MovimientoInventario movimientoInventario)
        {
            if (ModelState.IsValid)
            {
                var inventarioAfectado = db.Inventarios.FirstOrDefault(i => i.ProductoId == movimientoInventario.ProductoId);

                if (inventarioAfectado != null)
                {
                    // stok no encontrado no me aparece el error
                    if ("Entrada".Equals(movimientoInventario.TipoMovimiento, StringComparison.OrdinalIgnoreCase))
                    {
                        inventarioAfectado.Stock += movimientoInventario.Cantidad;
                    }
                    else if ("Salida".Equals(movimientoInventario.TipoMovimiento, StringComparison.OrdinalIgnoreCase))
                    {
                        inventarioAfectado.Stock -= movimientoInventario.Cantidad;
                    }
                    db.Entry(inventarioAfectado).State = EntityState.Modified;
                }

                db.MovimientoInventarios.Add(movimientoInventario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductoId = new SelectList(db.Productoes, "IdProducto", "Nombre", movimientoInventario.ProductoId);
            ViewBag.DesdeUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.DesdeUbicacionId);
            ViewBag.HaciaUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.HaciaUbicacionId);
            ViewBag.UsuarioId = new SelectList(db.Usuarios, "IdUsuario", "NombreUsuario", movimientoInventario.UsuarioId);
            return View(movimientoInventario);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            MovimientoInventario movimientoInventario = db.MovimientoInventarios.Find(id);
            if (movimientoInventario == null) return HttpNotFound();
            ViewBag.ProductoId = new SelectList(db.Productoes, "IdProducto", "Nombre", movimientoInventario.ProductoId);
            ViewBag.DesdeUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.DesdeUbicacionId);
            ViewBag.HaciaUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.HaciaUbicacionId);
            ViewBag.UsuarioId = new SelectList(db.Usuarios, "IdUsuario", "NombreUsuario", movimientoInventario.UsuarioId);
            return View(movimientoInventario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdMovimientoInventario,Fecha,ProductoId,DesdeUbicacionId,HaciaUbicacionId,Cantidad,UsuarioId,Observacion")] MovimientoInventario movimientoInventario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movimientoInventario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductoId = new SelectList(db.Productoes, "IdProducto", "Nombre", movimientoInventario.ProductoId);
            ViewBag.DesdeUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.DesdeUbicacionId);
            ViewBag.HaciaUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.HaciaUbicacionId);
            ViewBag.UsuarioId = new SelectList(db.Usuarios, "IdUsuario", "NombreUsuario", movimientoInventario.UsuarioId);
            return View(movimientoInventario);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            MovimientoInventario movimientoInventario = db.MovimientoInventarios.Find(id);
            if (movimientoInventario == null) return HttpNotFound();
            return View(movimientoInventario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MovimientoInventario movimientoInventario = db.MovimientoInventarios.Find(id);
            db.MovimientoInventarios.Remove(movimientoInventario);
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