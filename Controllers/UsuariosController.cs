using ProyectoWebInventario.Filters;
using ProyectoWebInventario.Models;
using ProyectoWebInventario.Recursos;
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
    public class UsuariosController : BaseController
    {
        private BDBodegasEntities db = new BDBodegasEntities();

        // GET: Usuarios
        [PermisoAttributes("VerUsuarios")]
    
        public ActionResult Index(string busqueda, int page = 1)
        {
            const int pageSize = 7; // puedes cambiarlo a 7/8/lo que quieras

            if (page < 1) page = 1;

            IQueryable<Usuario> q = db.Usuarios.AsNoTracking();

            // Filtro de búsqueda (opcional)
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                q = q.Where(u =>
                    (u.NombreUsuario ?? "").Contains(busqueda) ||
                    (u.Rol ?? "").Contains(busqueda));
            }

            // Orden estable antes de paginar (desempate por Id)
            q = q.OrderBy(u => u.NombreUsuario)
                 .ThenBy(u => u.IdUsuario);

            // Totales
            var totalItems = q.Count();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
            if (page > totalPages) page = totalPages;

            // Página actual
            var usuarios = q.Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            // ViewBags para la vista
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.Busqueda = busqueda;

            return View(usuarios);
        }



        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuarios/Create
        [PermisoAttributes("CrearUsuarios")]
        public ActionResult Create()
        {
            var roles = db.Rols.ToList(); // db.Rols = tabla de roles
            ViewBag.Roles = new SelectList(roles, "NombreRol", "NombreRol"); // valor = NombreRol, texto = NombreRol
            return View();
        }

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [PermisoAttributes("CrearUsuarios")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdUsuario,NombreUsuario,HasPassword,Rol,Activo")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.HasPassword = Encript.EncriptarSHA256(usuario.HasPassword);

                db.Usuarios.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        [PermisoAttributes("EditarUsuarios")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [PermisoAttributes("EditarUsuarios")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdUsuario,NombreUsuario,HasPassword,Rol,Activo")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        [PermisoAttributes("EliminarUsuarios")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
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
