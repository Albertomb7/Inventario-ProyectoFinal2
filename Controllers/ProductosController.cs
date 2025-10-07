using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ProyectoWebInventario.Models;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System.IO;
using System;

namespace ProyectoWebInventario.Controllers
{
    public class ProductosController : BaseController
    {
        private BDBodegasEntities db = new BDBodegasEntities();


        // GET: Productos
        public ActionResult Index(string busqueda)
        {
            var productos = db.Productoes.Include(p => p.Inventarios);
            if (!string.IsNullOrEmpty(busqueda))
            {
                productos = productos.Where(p => p.Nombre.Contains(busqueda));
            }
            return View(productos.ToList());
        }

        // GET: Productos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            // Corregido para cargar inventario también en Detalles
            Producto producto = db.Productoes.Include(p => p.Inventarios).FirstOrDefault(p => p.IdProducto == id);
            if (producto == null) return HttpNotFound();
            return View(producto);
        }

        // GET: Productos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Nombre,Categoria,Marca,UnidadMedida,StockMinimo,Activo")] Producto producto, int StockInicial = 0)
        {
            if (ModelState.IsValid)
            {
                db.Productoes.Add(producto);

                var primeraUbicacion = db.Ubicacions.FirstOrDefault();
                if (primeraUbicacion != null)
                {
                    Inventario inventarioInicial = new Inventario
                    {
                        ProductoIdInventario = producto.IdProducto, // Usar el nombre de propiedad correcto
                        UbicacionId = primeraUbicacion.IdUbicacion,
                        Stock = StockInicial
                    };
                    db.Inventarios.Add(inventarioInicial);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(producto);
        }

        // GET: Productos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Producto producto = db.Productoes.Find(id);
            if (producto == null) return HttpNotFound();
            return View(producto);
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id)
        {
            var productoAEditar = db.Productoes.Find(id);
            if (productoAEditar == null) return HttpNotFound();

            string[] camposPermitidos = new string[] { "Nombre", "Categoria", "Marca", "UnidadMedida", "StockMinimo", "Activo" };

            if (TryUpdateModel(productoAEditar, "", camposPermitidos) && ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productoAEditar);
        }

        // GET: Productos/Delete/5 -- MÉTODO CORREGIDO --
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Con este cambio, cargamos el producto Y la información de su inventario asociado.
            Producto producto = db.Productoes.Include(p => p.Inventarios).FirstOrDefault(p => p.IdProducto == id);

            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }


        // MÉTODO DELETECONFIRMED 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Producto productoAEliminar = db.Productoes
                .Include(p => p.Inventarios)
                .Include(p => p.MovimientoInventarios)
                .Include(p => p.AlertaReposicions)
                .FirstOrDefault(p => p.IdProducto == id);

            if (productoAEliminar == null)
            {
                return HttpNotFound();
            }

            db.AlertaReposicions.RemoveRange(productoAEliminar.AlertaReposicions);
            db.MovimientoInventarios.RemoveRange(productoAEliminar.MovimientoInventarios);
            db.Inventarios.RemoveRange(productoAEliminar.Inventarios);

            db.Productoes.Remove(productoAEliminar);

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }

        // Métodos de Exportación 
        public ActionResult ExportarExcel()
        {
            List<Producto> listaProductos = db.Productoes.Include(p => p.Inventarios).ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Productos");

                //  Encabezados de la tabla 
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Nombre";
                worksheet.Cells[1, 3].Value = "Categoría";
                worksheet.Cells[1, 4].Value = "Marca";
                worksheet.Cells[1, 5].Value = "Unidad de Medida";
                worksheet.Cells[1, 6].Value = "Stock Mínimo";
                worksheet.Cells[1, 7].Value = "Stock Actual";
                worksheet.Cells[1, 8].Value = "Estado";

                // Poner encabezados en negrita
                using (var range = worksheet.Cells["A1:H1"])
                {
                    range.Style.Font.Bold = true;
                }

                // Llenar la tabla con datos 
                int row = 2;
                foreach (var producto in listaProductos)
                {
                    worksheet.Cells[row, 1].Value = producto.IdProducto;

                    //  85
                    worksheet.Cells[row, 2].Value = producto.Nombre ?? "";
                    worksheet.Cells[row, 3].Value = producto.Categoria ?? "";
                    worksheet.Cells[row, 4].Value = producto.Marca ?? "";
                    worksheet.Cells[row, 5].Value = producto.UnidadMedida ?? "";

                    // StockMinim0
                    worksheet.Cells[row, 6].Value = producto.StockMinimo != null ? producto.StockMinimo.ToString() : "";

                    //  el Stock Actual
                    decimal stockActual = (producto.Inventarios != null && producto.Inventarios.Any()) ? producto.Inventarios.Sum(i => i.Stock) : 0;
                    worksheet.Cells[row, 7].Value = stockActual;

                    worksheet.Cells[row, 8].Value = producto.Activo == true ? "Activo" : "Inactivo";

                    row++;
                }

                // ancho de las columnas
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"ReporteProductos-{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        public ActionResult ExportarPDF()
        {
            List<Producto> listaProductos = db.Productoes.Include(p => p.Inventarios).ToList();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                //  Encabezado Profesional para que se vea good
                Font fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.BLACK);
                Font fuenteSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.DARK_GRAY);
                Font fuenteFecha = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);
                Paragraph nombreEmpresa = new Paragraph("Cortex - El Cerebro de tu Empresa", fuenteTitulo);
                nombreEmpresa.Alignment = Element.ALIGN_CENTER;
                document.Add(nombreEmpresa);
                Paragraph tituloReporte = new Paragraph("Reporte de Productos", fuenteSubtitulo);
                tituloReporte.Alignment = Element.ALIGN_CENTER;
                document.Add(tituloReporte);
                Paragraph fechaReporte = new Paragraph($"Generado el: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}", fuenteFecha);
                fechaReporte.Alignment = Element.ALIGN_RIGHT;
                document.Add(fechaReporte);
                document.Add(Chunk.NEWLINE);
                // Fin del Encabezado 

                PdfPTable table = new PdfPTable(7);
                table.WidthPercentage = 100;

                // Encabezados
                table.AddCell("ID");
                table.AddCell("Nombre");
                table.AddCell("Categoría");
                table.AddCell("Marca");
                table.AddCell("Stock Mínimo");
                table.AddCell("Stock Actual");
                table.AddCell("Estado");

                foreach (var producto in listaProductos)
                {
                    table.AddCell(producto.IdProducto.ToString());

                    
                    table.AddCell(producto.Nombre ?? "");
                    table.AddCell(producto.Categoria ?? "");
                    table.AddCell(producto.Marca ?? "");

                    
                    table.AddCell(producto.StockMinimo != null ? producto.StockMinimo.ToString() : "");

                    decimal stockActual = (producto.Inventarios != null && producto.Inventarios.Any()) ? producto.Inventarios.Sum(i => i.Stock) : 0;
                    table.AddCell(stockActual.ToString());

                    table.AddCell(producto.Activo == true ? "Activo" : "Inactivo");
                }

                document.Add(table);
                document.Close();

                string pdfName = $"ReporteProductos-{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf";
                return File(memoryStream.ToArray(), "application/pdf", pdfName);
            }
        }
    }
}   