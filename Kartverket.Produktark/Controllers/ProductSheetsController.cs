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
using System.Security.Claims;

namespace Kartverket.Produktark.Controllers
{
    [HandleError]
    [Authorize]
    public class ProductSheetsController : Controller
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            if (IsAdmin())
                return View(_dbContext.ProductSheet.ToList());
            else
            return View(_productSheetService.FindProductSheetsForOrganization(ClaimsPrincipal.Current.Organization()));
           
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
            Session["alreadyCreatedId"] = null;
            //Check if Productsheet already exists and redirect to it
            ProductSheet UuidExists = _dbContext.ProductSheet.FirstOrDefault(ps => ps.Uuid == uuid);
            if (UuidExists != null && !string.IsNullOrWhiteSpace(uuid))
                return RedirectToAction("Edit", new { id = UuidExists.Id });

            ProductSheet model = null;
            if (!string.IsNullOrWhiteSpace(uuid))
            {
                model = _productSheetService.CreateProductSheetFromMetadata(uuid);
                model.SetTranslations();
            }
            else
                model = new ProductSheet();

            return View(model);
        }


        // POST: ProductSheets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductSheet productSheet)
        {
            
            if (ModelState.IsValid)
            {

                if (Session["alreadyCreatedId"] != null) 
                {
                    productSheet.Id = (int)Session["alreadyCreatedId"];
                    _dbContext.Entry(productSheet).State = EntityState.Modified;
                }
                else
                {
                    _dbContext.ProductSheet.Add(productSheet);
                }
                
                _dbContext.SaveChanges();
                Session["alreadyCreatedId"] = productSheet.Id;
                return RedirectToAction("CreatePdf", new { id = productSheet.Id });
            }
            return View("Edit", productSheet);
        }


        // GET: ProductSheets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            ProductSheet productSheet = _dbContext.ProductSheet.Find(id);
            if (productSheet == null)
            {
                return HttpNotFound();
            }
            productSheet.SetTranslations();

            return View(productSheet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(Duration = 0)]
        public ActionResult Edit(ProductSheet productSheet)
        {

            if (!string.IsNullOrWhiteSpace(Request["hentNyeMetaData"]))
            {
                ProductSheet model = _dbContext.ProductSheet.Find(productSheet.Id);
                if (model == null)
                {
                    return HttpNotFound();
                }

                model = _productSheetService.UpdateProductSheetFromMetadata(productSheet.Uuid, model);

                model.PrecisionInMeters = productSheet.PrecisionInMeters;
                model.CoverageArea = productSheet.CoverageArea;
                model.Projections = productSheet.Projections;
                model.ServiceDetails = productSheet.ServiceDetails;
                model.ListOfFeatureTypes = productSheet.ListOfFeatureTypes;
                model.ListOfAttributes = productSheet.ListOfAttributes;


                if (ModelState.IsValid)
                {
                    _dbContext.Entry(model).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                }

                return RedirectToAction("Edit", new { id = model.Id });
                
            }

            else {


                if (ModelState.IsValid)
                {
                    _dbContext.Entry(productSheet).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    return RedirectToAction("CreatePdf", new { id = productSheet.Id });
                }
             return View(productSheet);
            }
        }

        public ActionResult CreatePdf(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imagePath = Server.MapPath("~/Images/");
            var imagePathLogo = Server.MapPath("~/Logos/");

            ProductSheet productSheet = _dbContext.ProductSheet.Find(id);
            if (productSheet == null)
            {
                return HttpNotFound();
            }

            string logoPath="";
            //Logo logo =_productSheetService.FindLogoForOrganization(ClaimsPrincipal.Current.Organization());
            string logo = _productSheetService.GetLogoForOrganization(productSheet.ContactOwner.Organization);
            if (logo != null)
                logoPath = logo;

            Stream fileStream = new PdfGenerator(productSheet, imagePath, logoPath).CreatePdf();
            var fileStreamResult = new FileStreamResult(fileStream, "application/pdf");
            fileStreamResult.FileDownloadName = GetSafeFilename(productSheet.Title + ".pdf");

            Log.Info(string.Format("Creating PDF for {0} [{1}]", productSheet.Title, productSheet.Uuid));

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

        public string GetSafeFilename(string filename)
        {
            filename=filename.Replace(" ", "_");
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }


        bool IsAdmin() {

            foreach (var role in ClaimsPrincipal.Current.Roles())
            {
                 if(role == "nd.metadata_admin"){
                     return true;
                 }
            }
            return false;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }

    }
}
