using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoWebInventario.ViewModels
{
    public class AsignarPermisosViewModel
    {
        public int IdRol { get; set; }
        public string NombreRol { get; set; }
        public List<PermisoSeleccionado> Permisos { get; set; }
    }

    public class PermisoSeleccionado
    {
        public int IdPermiso { get; set; }
        public string Nombre { get; set; }
        public bool Autorizado { get; set; }
    }
}