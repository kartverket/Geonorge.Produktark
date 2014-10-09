using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GeoNorgeAPI;
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

       
        public ActionResult Create(string uuid)
        {
            ProductSheet model = null;
            if (!string.IsNullOrWhiteSpace(uuid))
            {
                model = new ProductSheetService().CreateProductSheetFromMetadata(uuid);
            }
            return View(model);
        }


        // POST: ProductSheets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductSheet productSheet)
        {
            if (ModelState.IsValid)
            {
                db.ProductSheet.Add(productSheet);
                db.SaveChanges();
                return RedirectToAction("CreatePdf", new { id = productSheet.Id });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductSheet productSheet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productSheet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CreatePdf", new { id = productSheet.Id });
            }
            return View(productSheet);
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
            fileStreamResult.FileDownloadName = Server.UrlEncode("Produktark-" + productSheet.Uuid + ".pdf");
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
