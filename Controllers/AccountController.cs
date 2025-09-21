using ProyectoWebInventario.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoWebInventario.Controllers
{
    public class AccountController : Controller
    {
        private BDBodegasEntities db = new BDBodegasEntities();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Usuario model)
        {
            if (ModelState.IsValid)
            {
                string passwordEncriptada = Recursos.Encript.EncriptarSHA256(model.HasPassword);

                var usuario = db.Usuarios.FirstOrDefault(u =>
                    u.NombreUsuario == model.NombreUsuario &&
                    u.HasPassword == passwordEncriptada &&
                    u.Activo == true);

                if (usuario != null)
                {
                    // Guardar el nombre en sesión
                    Session["Usuario"] = usuario.NombreUsuario;
                    Session["Rol"] = usuario.Rol;

                    return RedirectToAction("Index", "Home"); 
                }

                ModelState.AddModelError("k","Usuario o contraseña incorrectos");
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}