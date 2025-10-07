using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoWebInventario.Models;
using ProyectoWebInventario.ViewModels;

namespace ProyectoWebInventario.Controllers
{
    public class RolsController : Controller
    {
        private BDBodegasEntities db = new BDBodegasEntities();


        /// ******************************************************************************************
        /// 


        // GET: Rol/AsignarPermisos/5
        public ActionResult AsignarPermisos(int id)
        {
            var rol = db.Rols.Find(id);
            var permisos = db.Permisoes.ToList();

            // Obtener los permisos actualmente denegados
            var denegados = db.RolPermisoes
                .Where(rp => rp.RolId == id)
                .Select(rp => rp.PermisoId)
                .ToList();

            var modelo = new AsignarPermisosViewModel
            {
                IdRol = rol.IdRol,
                NombreRol = rol.NombreRol,
                Permisos = permisos.Select(p => new PermisoSeleccionado
                {
                    IdPermiso = p.IdPermiso,
                    Nombre = p.NombrePermiso,
                    Autorizado = !denegados.Contains(p.IdPermiso)
                }).ToList()
            };

            return View(modelo);

        }

        [HttpPost]
        public ActionResult AsignarPermisos(int IdRol, int[] permisosAutorizados)
        {
            var todos = db.Permisoes.Select(p => p.IdPermiso).ToList();
            var denegados = todos.Except(permisosAutorizados ?? new int[0]).ToList();

            // Limpiar los permisos anteriores
            var existentes = db.RolPermisoes.Where(rp => rp.RolId == IdRol);
            db.RolPermisoes.RemoveRange(existentes);

            // Agregar los nuevos denegados
            foreach (var idPermiso in denegados)
            {
                db.RolPermisoes.Add(new RolPermiso
                {
                    RolId = IdRol,
                    PermisoId = idPermiso
                });
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }





        /// <summary>
        /// //////////////////////
        /// </summary>
        /// <returns></returns>


        // GET: Rols
        public ActionResult Index()
        {
            return View(db.Rols.ToList());
        }

        // GET: Rols/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rol rol = db.Rols.Find(id);
            if (rol == null)
            {
                return HttpNotFound();
            }
            return View(rol);
        }

        // GET: Rols/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Rols/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdRol,NombreRol,Descripcion")] Rol rol)
        {
            if (ModelState.IsValid)
            {
                db.Rols.Add(rol);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rol);
        }

        // GET: Rols/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rol rol = db.Rols.Find(id);
            if (rol == null)
            {
                return HttpNotFound();
            }
            return View(rol);
        }

        // POST: Rols/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdRol,NombreRol,Descripcion")] Rol rol)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rol).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rol);
        }

        // GET: Rols/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rol rol = db.Rols.Find(id);
            if (rol == null)
            {
                return HttpNotFound();
            }
            return View(rol);
        }

        // POST: Rols/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rol rol = db.Rols.Find(id);
            db.Rols.Remove(rol);
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
