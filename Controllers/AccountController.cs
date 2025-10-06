using ProyectoWebInventario.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoWebInventario.Controllers
{
    [AllowAnonymous] // Permite acceder al login aunque uses [Authorize] global
    public class AccountController : Controller
    {
        private readonly BDBodegasEntities db = new BDBodegasEntities();

        [HttpGet]
        public ActionResult Login()
        {
            // Devuelve la vista con un modelo vacío 
            return View(new Usuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Usuario model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Normaliza entradas
            var userName = (model.NombreUsuario ?? string.Empty).Trim();
            var rawPass = model.HasPassword ?? string.Empty;

            // Encripta y busca
            string passwordEncriptada = Recursos.Encript.EncriptarSHA256(rawPass);

            var usuario = db.Usuarios.FirstOrDefault(u =>
                u.NombreUsuario == userName &&
                u.HasPassword == passwordEncriptada &&
                u.Activo == true);

            if (usuario == null)
            {

                ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                return View(model);
            }

            // Guarda en sesión
            Session["Usuario"] = usuario.NombreUsuario;
            Session["Rol"] = usuario.Rol;

            var returnUrl = Request.QueryString["returnUrl"];
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Pantalla principal
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}