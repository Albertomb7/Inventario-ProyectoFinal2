using iTextSharp.text;
using iTextSharp.text.pdf;
using ProyectoWebInventario.Filters;
using ProyectoWebInventario.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ProyectoWebInventario.Controllers
{
    public class UbicacionesController : BaseController
    {
        private BDBodegasEntities db = new BDBodegasEntities();

        // GET: Ubicaciones
        [PermisoAttributes("VerUbicaciones")]
        public ActionResult Index()
        {
            return View(db.Ubicacions.ToList());
        }

        // GET: Ubicacions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ubicacion ubicacion = db.Ubicacions.Find(id);
            if (ubicacion == null)
            {
                return HttpNotFound();
            }
            return View(ubicacion);
        }
        public ActionResult ExportarPDF()
        {
            List<Ubicacion> listaUbicaciones = db.Ubicacions.ToList();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                var tituloFont = FontFactory.GetFont("Arial", 25, Font.BOLD);
                var subtituloFont = FontFactory.GetFont("Arial", 18, Font.ITALIC);
                var textoFont = FontFactory.GetFont("Arial", 14, Font.NORMAL);

                var titulo = new Paragraph("Reporte de Ubicaciones del Almacén Cortex ", tituloFont);
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                var nombreAlmacen = new Paragraph("Almacén: Cortex El cerebro de tu Empresa", subtituloFont);
                nombreAlmacen.Alignment = Element.ALIGN_CENTER;
                document.Add(nombreAlmacen);

                var fecha = new Paragraph($"Generado el: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}", subtituloFont);
                fecha.Alignment = Element.ALIGN_CENTER;
                document.Add(fecha);

                var descripcion = new Paragraph("Este documento presenta el listado oficial de las ubicaciones registradas en el sistema, incluyendo su código, descripción y estado actual.", textoFont);
                descripcion.Alignment = Element.ALIGN_JUSTIFIED;
                descripcion.SpacingBefore = 10f;
                descripcion.SpacingAfter = 10f;
                document.Add(descripcion);

                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.AddCell("ID");
                table.AddCell("Código");
                table.AddCell("Descripción");
                table.AddCell("Estado");

                foreach (var ubicacion in listaUbicaciones)
                {
                    table.AddCell(ubicacion.IdUbicacion.ToString());
                    table.AddCell(ubicacion.Codigo);
                    table.AddCell(ubicacion.Descripcion);
                    table.AddCell(ubicacion.Activo == true ? "Activo" : "Inactivo");
                }

                document.Add(table);
                document.Close();

                string pdfName = $"ReporteUbicaciones-{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf";
                return File(memoryStream.ToArray(), "application/pdf", pdfName);
            }
        }
        // GET: Ubicacions/Create
        [PermisoAttributes("CrearUbicaciones")]
        public ActionResult Create()
        {
            return View();
        }

        [PermisoAttributes("CrearUbicaciones")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdUbicacion,Codigo,Descripcion,Activo")] Ubicacion ubicacion)
        {
            if (ModelState.IsValid)
            {
                db.Ubicacions.Add(ubicacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ubicacion);
        }

        // GET: Ubicacions/Edit/5
        [PermisoAttributes("EditarUbicaciones")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ubicacion ubicacion = db.Ubicacions.Find(id);
            if (ubicacion == null)
            {
                return HttpNotFound();
            }
            return View(ubicacion);
        }

        [PermisoAttributes("EditarUbicaciones")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdUbicacion,Codigo,Descripcion,Activo")] Ubicacion ubicacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ubicacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ubicacion);
        }

        // GET: Ubicacions/Delete/5
        [PermisoAttributes("EliminarUbicaciones")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ubicacion ubicacion = db.Ubicacions.Find(id);
            if (ubicacion == null)
            {
                return HttpNotFound();
            }
            return View(ubicacion);
        }

        // logica de elimianr ubicacion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Verifico si la ubicación está siendo usada en algún movimiento
            bool estaEnUsoEnMovimientos = db.MovimientoInventarios.Any(m => m.DesdeUbicacionId == id || m.HaciaUbicacionId == id);

            // Verifico  si la ubicación todavía tiene stock registrado
            bool tieneStockRegistrado = db.Inventarios.Any(i => i.UbicacionId == id && i.Stock > 0);

            if (estaEnUsoEnMovimientos || tieneStockRegistrado)
            {
                // si tiene stok o esta en uso no se puede eliminar
                string mensajeError = "Error: No se puede eliminar la ubicación porque ";
                if (estaEnUsoEnMovimientos) mensajeError += "ya ha sido utilizada en el historial de movimientos. ";
                if (tieneStockRegistrado) mensajeError += "aún tiene stock registrado. ";

                TempData["NotificationMessage"] = mensajeError;
                TempData["NotificationType"] = "danger"; // Notificación roja
                return RedirectToAction("Index");
            }

            var inventariosAEliminar = db.Inventarios.Where(i => i.UbicacionId == id).ToList();
            if (inventariosAEliminar.Any())
            {
                db.Inventarios.RemoveRange(inventariosAEliminar);
            }

            // Ahora sí se puede elinar la ubicaion stok en 0 papa
            Ubicacion ubicacion = db.Ubicacions.Find(id);
            if (ubicacion != null)
            {
                db.Ubicacions.Remove(ubicacion);
                db.SaveChanges();

                TempData["NotificationMessage"] = "La ubicación ha sido eliminada correctamente.";
                TempData["NotificationType"] = "success"; // Notificación verde
            }

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