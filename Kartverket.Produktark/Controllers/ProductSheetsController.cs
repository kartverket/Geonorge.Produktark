using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kartverket.Produktark.Models;

namespace Kartverket.Produktark.Controllers
{
    public class ProductSheetsController : Controller
    {
        private ProductSheetContext db = new ProductSheetContext();

        // GET: ProductSheets
        public ActionResult Index()
        {
            return View(db.ProductSheet.ToList());
        }

        // GET: ProductSheets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSheet productSheet = db.ProductSheet.Find(id);
            if (productSheet == null)
            {
                return HttpNotFound();
            }
            return View(productSheet);
        }

        // GET: ProductSheets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductSheets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Uuid,Title,Description")] ProductSheet productSheet)
        {
            if (ModelState.IsValid)
            {
                db.ProductSheet.Add(productSheet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productSheet);
        }

        // GET: ProductSheets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSheet productSheet = db.ProductSheet.Find(id);
            if (productSheet == null)
            {
                return HttpNotFound();
            }
            return View(productSheet);
        }

        // POST: ProductSheets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Uuid,Title,Description")] ProductSheet productSheet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productSheet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productSheet);
        }

        // GET: ProductSheets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSheet productSheet = db.ProductSheet.Find(id);
            if (productSheet == null)
            {
                return HttpNotFound();
            }
            return View(productSheet);
        }

        // POST: ProductSheets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductSheet productSheet = db.ProductSheet.Find(id);
            db.ProductSheet.Remove(productSheet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult CreatePdf(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imagePath = Server.MapPath("~/Images");


            ProductSheet productSheet = db.ProductSheet.Find(id);
            if (productSheet == null)
            {
                return HttpNotFound();
            }

            Stream fileStream = new PdfGenerator().CreatePdf(productSheet, imagePath);
            var fileStreamResult = new FileStreamResult(fileStream, "application/pdf");
            fileStreamResult.FileDownloadName = Server.UrlEncode(productSheet.Title + ".pdf");
            return fileStreamResult;
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
