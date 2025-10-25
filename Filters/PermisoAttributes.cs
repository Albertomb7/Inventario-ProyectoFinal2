using ProyectoWebInventario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoWebInventario.Filters
{
    public class PermisoAttributes : AuthorizeAttribute
    {
        private readonly string _nombrePermiso;

        public PermisoAttributes(string nombrePermiso)
        {
            _nombrePermiso = nombrePermiso;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["IdUsuario"] == null)
                return false;

            int idUsuario = Convert.ToInt32(httpContext.Session["IdUsuario"]);

            using (var db = new BDBodegasEntities())
            {
                // Obtener el nombre del rol (campo nvarchar en la tabla Usuario)
                var nombreRol = db.Usuarios
                                  .Where(u => u.IdUsuario == idUsuario)
                                  .Select(u => u.Rol)
                                  .FirstOrDefault();

                if (string.IsNullOrEmpty(nombreRol))
                    return false;

                // Buscar el IdRol en la tabla Rol
                var idRol = db.Rols
                              .Where(r => r.NombreRol == nombreRol)
                              .Select(r => r.IdRol)
                              .FirstOrDefault();

                if (idRol == 0)
                    return false;

                //Buscar el Id del permiso que se requiere
                var idPermiso = db.Permisoes
                          .Where(p => p.NombrePermiso == _nombrePermiso)
                          .Select(p => p.IdPermiso)
                          .FirstOrDefault();

                if (idPermiso == 0)
                    return false;

                // Verificar si el permiso está bloqueado (existe en RolPermiso)
                bool estaBloqueado = db.RolPermisoes
                             .Any(rp => rp.RolId == idRol && rp.PermisoId == idPermiso);


                return !estaBloqueado;
            }
        }

        public static bool Check(HttpContextBase httpContext, string nombrePermiso)
        {
            // Creamos una instancia con el permiso pedido y usamos tu AuthorizeCore
            return new PermisoAttributes(nombrePermiso).AuthorizeCore(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Home/AccesoDenegado");
        }
    }
}