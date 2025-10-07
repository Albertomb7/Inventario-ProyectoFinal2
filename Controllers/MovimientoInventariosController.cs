using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using ProyectoWebInventario.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ProyectoWebInventario.Controllers
{
    public class MovimientoInventariosController : BaseController
    {
        private BDBodegasEntities db = new BDBodegasEntities();

        public ActionResult Index()
        {
            var movimientoInventarios = db.MovimientoInventarios.Include(m => m.Producto).Include(m => m.Ubicacion).Include(m => m.Ubicacion1).Include(m => m.Usuario).OrderByDescending(m => m.IdMovimientoInventario); ;
            return View(movimientoInventarios.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            MovimientoInventario movimientoInventario = db.MovimientoInventarios.Find(id);
            if (movimientoInventario == null) return HttpNotFound();
            return View(movimientoInventario);
        }

        public ActionResult Create()
        {
            ViewBag.ProductoId = new SelectList(db.Productoes, "IdProducto", "Nombre");
            ViewBag.DesdeUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Descripcion");
            ViewBag.HaciaUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Descripcion");
            ViewBag.UsuarioId = new SelectList(db.Usuarios, "IdUsuario", "NombreUsuario");
            return View();
        }

        // MÉTODo CORREG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdMovimientoInventario,Fecha,ProductoId,DesdeUbicacionId,HaciaUbicacionId,Cantidad,UsuarioId,Observacion, TipoMovimiento")] MovimientoInventario movimientoInventario)
        {
            movimientoInventario.Fecha = DateTime.Today;
            
            if (ModelState.IsValid)
            {
                //   modelo.
                var inventarioOrigen = db.Inventarios.FirstOrDefault(i => i.ProductoIdInventario == movimientoInventario.ProductoId && i.UbicacionId == movimientoInventario.DesdeUbicacionId);
                var inventarioDestino = db.Inventarios.FirstOrDefault(i => i.ProductoIdInventario == movimientoInventario.ProductoId && i.UbicacionId == movimientoInventario.HaciaUbicacionId);

                if (movimientoInventario.TipoMovimiento.Equals("Salida", StringComparison.OrdinalIgnoreCase))
                {
                    //VALIDACION PARA MOSTRAR QUE NO HAY SUFICIENTE STOCK - B
                    if (inventarioOrigen == null || inventarioOrigen.Stock < movimientoInventario.Cantidad)
                    {
                        ModelState.AddModelError("", "No hay suficiente stock en la ubicación de origen para realizar la salida.");
                        ViewBag.ProductoId = new SelectList(db.Productoes, "IdProducto", "Nombre", movimientoInventario.ProductoId);
                        ViewBag.DesdeUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.DesdeUbicacionId);
                        ViewBag.HaciaUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.HaciaUbicacionId);
                        ViewBag.UsuarioId = new SelectList(db.Usuarios, "IdUsuario", "NombreUsuario", movimientoInventario.UsuarioId);
                        return View(movimientoInventario);
                    }
                    inventarioOrigen.Stock -= movimientoInventario.Cantidad;
                }
                else if (movimientoInventario.TipoMovimiento.Equals("Entrada", StringComparison.OrdinalIgnoreCase))
                {
                    if (inventarioDestino == null)
                    {
                        
                        inventarioDestino = new Inventario { ProductoIdInventario = movimientoInventario.ProductoId, UbicacionId = (int)movimientoInventario.HaciaUbicacionId, Stock = movimientoInventario.Cantidad };
                        db.Inventarios.Add(inventarioDestino);
                    }
                    else
                    {
                        inventarioDestino.Stock += movimientoInventario.Cantidad;

                        //aaaaaaaaaaaaaa
                        var producto = db.Productoes.Find(movimientoInventario.ProductoId);
                        decimal stockMinimoNumerico;

                        if (decimal.TryParse(producto.StockMinimo, out stockMinimoNumerico))
                        {

                            var alerta = db.AlertaReposicions.FirstOrDefault(a => a.ProductoIdAlertaReposicion == producto.IdProducto);
                            if (alerta != null)
                            {
                                alerta.Activo = false;
                                db.SaveChanges();
                            }

                        }
                    }
                }

                else if (movimientoInventario.TipoMovimiento.Equals("Transferencia", StringComparison.OrdinalIgnoreCase))
                {
                    if (inventarioOrigen == null || inventarioOrigen.Stock < movimientoInventario.Cantidad)
                    {
                        ModelState.AddModelError("", "No hay suficiente stock en la ubicación de origen para realizar la transferencia.");
                        ViewBag.ProductoId = new SelectList(db.Productoes, "IdProducto", "Nombre", movimientoInventario.ProductoId);
                        ViewBag.DesdeUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.DesdeUbicacionId);
                        ViewBag.HaciaUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.HaciaUbicacionId);
                        ViewBag.UsuarioId = new SelectList(db.Usuarios, "IdUsuario", "NombreUsuario", movimientoInventario.UsuarioId);
                        return View(movimientoInventario);
                    }

                    inventarioOrigen.Stock -= movimientoInventario.Cantidad;
                    if (inventarioDestino == null)
                    {
                        
                        inventarioDestino = new Inventario { ProductoIdInventario = movimientoInventario.ProductoId, UbicacionId = (int)movimientoInventario.HaciaUbicacionId, Stock = movimientoInventario.Cantidad };
                        db.Inventarios.Add(inventarioDestino);
                    }
                    else
                    {
                        inventarioDestino.Stock += movimientoInventario.Cantidad;
                    }
                }

                var reporteBitacora = new Bitacora
                {
                    FechaRegistro = DateTime.Now,
                    UsuarioId = 2, 
                    Accion = "Movimiento de inventario: " + movimientoInventario.TipoMovimiento,
                    TipoAccion = movimientoInventario.TipoMovimiento,
                    TablaAfectada = "Inventario/MovimientoInventario",
                    Comentario = movimientoInventario.Observacion
                };
                db.Bitacoras.Add(reporteBitacora);
                db.MovimientoInventarios.Add(movimientoInventario);
                db.SaveChanges();

                if (!movimientoInventario.TipoMovimiento.Equals("Entrada", StringComparison.OrdinalIgnoreCase))
                {
                    var producto = db.Productoes.Find(movimientoInventario.ProductoId);
                    // CProductoIdInventario' y hacemos el Sum más seguro
                    decimal stockActualTotal = db.Inventarios.Where(i => i.ProductoIdInventario == movimientoInventario.ProductoId).Sum(i => (decimal?)i.Stock) ?? 0;
                    string mensaje = $"Salida/Transferencia registrada: Se retiraron {movimientoInventario.Cantidad} unidades de '{producto.Nombre}'. Stock total restante: {stockActualTotal}.";



                    //MUESTRA LA ALERTA DE STOCK MINIMO - B
                    decimal stockMinimoNumerico;
                    if (decimal.TryParse(producto.StockMinimo, out stockMinimoNumerico))
                    {
                        if (stockActualTotal <= stockMinimoNumerico)
                        {
                            db.AlertaReposicions.Add(new AlertaReposicion
                            {
                                ProductoIdAlertaReposicion = movimientoInventario.ProductoId,
                                FechaDeGeneracion = DateTime.Now,
                                NivelActual = Convert.ToInt32(stockActualTotal),
                                Activo = true
                            }); 
                            db.SaveChanges();

                            mensaje += " ¡Atención! El stock ha alcanzado o está por debajo del nivel mínimo.";
                            TempData["NotificationType"] = "warning";
                        }
                        else
                        {
                            TempData["NotificationType"] = "success";
                        }
                    }
                    else
                    {
                        TempData["NotificationType"] = "success";
                    }
                    TempData["NotificationMessage"] = mensaje;
                }

                // Lógica de redirección inteligente
                if (TempData["NotificationType"] as string == "warning")
                {
                    return RedirectToAction("Index", "MovimientoInventarios");
                }
                return RedirectToAction("Index");
            }

            ViewBag.ProductoId = new SelectList(db.Productoes, "IdProducto", "Nombre", movimientoInventario.ProductoId);
            ViewBag.DesdeUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.DesdeUbicacionId);
            ViewBag.HaciaUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.HaciaUbicacionId);
            ViewBag.UsuarioId = new SelectList(db.Usuarios, "IdUsuario", "NombreUsuario", movimientoInventario.UsuarioId);
            return View(movimientoInventario);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            MovimientoInventario movimientoInventario = db.MovimientoInventarios.Find(id);
            if (movimientoInventario == null) return HttpNotFound();
            ViewBag.ProductoId = new SelectList(db.Productoes, "IdProducto", "Nombre", movimientoInventario.ProductoId);
            ViewBag.DesdeUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.DesdeUbicacionId);
            ViewBag.HaciaUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.HaciaUbicacionId);
            ViewBag.UsuarioId = new SelectList(db.Usuarios, "IdUsuario", "NombreUsuario", movimientoInventario.UsuarioId);
            return View(movimientoInventario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdMovimientoInventario,Fecha,ProductoId,DesdeUbicacionId,HaciaUbicacionId,Cantidad,UsuarioId,Observacion")] MovimientoInventario movimientoInventario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movimientoInventario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductoId = new SelectList(db.Productoes, "IdProducto", "Nombre", movimientoInventario.ProductoId);
            ViewBag.DesdeUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.DesdeUbicacionId);
            ViewBag.HaciaUbicacionId = new SelectList(db.Ubicacions, "IdUbicacion", "Codigo", movimientoInventario.HaciaUbicacionId);
            ViewBag.UsuarioId = new SelectList(db.Usuarios, "IdUsuario", "NombreUsuario", movimientoInventario.UsuarioId);
            return View(movimientoInventario);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            MovimientoInventario movimientoInventario = db.MovimientoInventarios.Find(id);
            if (movimientoInventario == null) return HttpNotFound();
            return View(movimientoInventario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MovimientoInventario movimientoInventario = db.MovimientoInventarios.Find(id);
            db.MovimientoInventarios.Remove(movimientoInventario);
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