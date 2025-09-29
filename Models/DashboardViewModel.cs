using System.Collections.Generic;

namespace ProyectoWebInventario.Models
{
    public class DashboardViewModel
    {
        public int TotalProductos { get; set; }
        public int TotalUbicaciones { get; set; }
        public int TotalAlertas { get; set; }

        // Una lista para mostrar los productos más recientes en la tabla
        public List<Producto> UltimosProductos { get; set; }
    }
}