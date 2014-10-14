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
using Kartverket.Produktark.Logging;
using Kartverket.Produktark.Models;

namespace Kartverket.Produktark.Controllers
{
    public class ProductSheetsController : Controller
    {
        private static readonly ILog Logger = LogProvider.For<ProductSheetsController>();

        private readonly ProductSheetContext _dbContext;
        private IProductSheetService _productSheetService;

        public ProductSheetsController(ProductSheetContext dbContext, IProductSheetService productSheetService)
        {
            _dbContext = dbContext;
            _productSheetService = productSheetService;
        }

        // GET: ProductSheets
        public ActionResult Index()
        {
            return View(_dbContext.ProductSheet.ToList());
        }

        // GET: ProductSheets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSheet productSheet = _dbContext.ProductSheet.Find(id);
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
                model = _productSheetService.CreateProductSheetFromMetadata(uuid);
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
                _dbContext.ProductSheet.Add(productSheet);
                _dbContext.SaveChanges();
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
            ProductSheet productSheet = _dbContext.ProductSheet.Find(id);
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
                _dbContext.Entry(productSheet).State = EntityState.Modified;
                _dbContext.SaveChanges();
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


            ProductSheet productSheet = _dbContext.ProductSheet.Find(id);
            if (productSheet == null)
            {
                return HttpNotFound();
            }

            Stream fileStream = new PdfGenerator(productSheet, imagePath).CreatePdf();
            var fileStreamResult = new FileStreamResult(fileStream, "application/pdf");
            fileStreamResult.FileDownloadName = Server.UrlEncode("Produktark-" + productSheet.Uuid + ".pdf");

            Logger.Info(string.Format("Creating PDF for {0} [{1}]", productSheet.Title, productSheet.Uuid));

            return fileStreamResult;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
