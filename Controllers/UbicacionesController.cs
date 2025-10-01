using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
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
    public class UbicacionesController : BaseController
    {
        private BDBodegasEntities db = new BDBodegasEntities();

        // GET: Ubicaciones
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

               

                //   fuentes para el diseño
                var tituloFont = FontFactory.GetFont("Arial", 25, Font.BOLD);
                var subtituloFont = FontFactory.GetFont("Arial", 18, Font.ITALIC);
                var textoFont = FontFactory.GetFont("Arial", 14, Font.NORMAL);

                // . Título principal
                var titulo = new Paragraph("Reporte de Ubicaciones del Almacén Cortex ", tituloFont);
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                //
                var nombreAlmacen = new Paragraph("Almacén: Cortex El cerebro de tu Empresa", subtituloFont);
                nombreAlmacen.Alignment = Element.ALIGN_CENTER;
                document.Add(nombreAlmacen);

                //  Fecha y hora de generación 
                var fecha = new Paragraph($"Generado el: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}", subtituloFont);
                fecha.Alignment = Element.ALIGN_CENTER;
                document.Add(fecha);

                //  Párrafo de descripción lo pueden modificar a su gusto
                var descripcion = new Paragraph("Este documento presenta el listado oficial de las ubicaciones registradas en el sistema, incluyendo su código, descripción y estado actual.", textoFont);
                descripcion.Alignment = Element.ALIGN_JUSTIFIED; 
                descripcion.SpacingBefore = 10f; 
                descripcion.SpacingAfter = 10f;  
                document.Add(descripcion);

                

                // Crear la tabla
                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100;

                // Encabezados de la tabla
                table.AddCell("ID");
                table.AddCell("Código");
                table.AddCell("Descripción");
                table.AddCell("Estado");

                // Llenar la tabla con datos
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ubicacions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // POST: Ubicacions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // POST: Ubicacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ubicacion ubicacion = db.Ubicacions.Find(id);
            db.Ubicacions.Remove(ubicacion);
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
