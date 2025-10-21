using System;
using System.Collections.Generic;
using System.Linq;
using GeoNorgeAPI;
using www.opengis.net;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using Kartverket.Geonorge.Utilities.Organization;
using System.Device.Location;

namespace Kartverket.Produktark.Models
{
    public class ProductSheetService : IProductSheetService
    {
        private readonly GeoNorge _geonorge;
        private readonly ProductSheetContext _dbContext;
        private readonly IOrganizationService _organizationService;
        RegisterFetcher register;

        public ProductSheetService(GeoNorge geoNorge, ProductSheetContext dbContext, IOrganizationService organizationService)
        {
            _geonorge = geoNorge;
            _dbContext = dbContext;
            _organizationService = organizationService;
        }

        public ProductSheet UpdateProductSheetFromMetadata(string uuid, ProductSheet productSheet) {
            register = new RegisterFetcher();
            MD_Metadata_Type metadata = _geonorge.GetRecordByUuid(uuid);
            if (metadata == null)
                throw new InvalidOperationException("Metadata not found for uuid: " + uuid);

            var simpleMetadata = new SimpleMetadata(metadata);

                dynamic metedataExtended = register.GetMetadataExtended(simpleMetadata.Uuid);

           
                productSheet.Uuid = simpleMetadata.Uuid;
                productSheet.Title = simpleMetadata.Title;
                productSheet.Description = simpleMetadata.Abstract;
                productSheet.SupplementalDescription = simpleMetadata.SupplementalDescription;
                productSheet.Purpose = simpleMetadata.Purpose;
                productSheet.SpecificUsage = simpleMetadata.SpecificUsage;
                productSheet.UseLimitations = simpleMetadata.Constraints != null ? simpleMetadata.Constraints.UseLimitations : null;
                productSheet.UseConstraintsLicenseLink = simpleMetadata.Constraints != null ? simpleMetadata.Constraints.UseConstraintsLicenseLink : null;
                productSheet.UseConstraintsLicenseLinkText = simpleMetadata.Constraints != null ? simpleMetadata.Constraints.UseConstraintsLicenseLinkText : null;
                productSheet.ContactMetadata = simpleMetadata.ContactMetadata != null ? CreateContact(simpleMetadata.ContactMetadata) : new Contact();
                productSheet.ContactPublisher = simpleMetadata.ContactPublisher != null ? CreateContact(simpleMetadata.ContactPublisher) : new Contact();
                productSheet.ContactOwner = simpleMetadata.ContactOwner != null ? CreateContact(simpleMetadata.ContactOwner) : new Contact();
                productSheet.ResolutionScale = simpleMetadata.ResolutionScale;
                productSheet.ResolutionDistance= simpleMetadata.ResolutionDistance.HasValue ? simpleMetadata.ResolutionDistance.Value.ToString() : "";
                productSheet.KeywordsPlace = CreateKeywords(SimpleKeyword.Filter(simpleMetadata.Keywords, SimpleKeyword.TYPE_PLACE, null));
                productSheet.ProcessHistory = simpleMetadata.ProcessHistory;
                productSheet.MaintenanceFrequency = simpleMetadata.MaintenanceFrequency;
                productSheet.Status = simpleMetadata.Status;
                productSheet.DistributionFormatName = simpleMetadata.DistributionFormat != null ? simpleMetadata.DistributionFormat.Name : null;
                productSheet.DistributionFormatVersion = simpleMetadata.DistributionFormat != null ? simpleMetadata.DistributionFormat.Version : null;
                productSheet.DistributionFormats = GetDistributionFormats(simpleMetadata.DistributionFormats);
                productSheet.AccessConstraints = simpleMetadata.Constraints != null ? register.GetRestriction(simpleMetadata.Constraints.AccessConstraints, simpleMetadata.Constraints.OtherConstraintsAccess) : null;
                productSheet.LegendDescriptionUrl = simpleMetadata.LegendDescriptionUrl;
                productSheet.ProductPageUrl = simpleMetadata.ProductPageUrl;
                productSheet.ProductSpecificationUrl = simpleMetadata.ProductSpecificationUrl;
                foreach (var thumbnail in simpleMetadata.Thumbnails){
                    productSheet.Thumbnail=thumbnail.URL;
                    if (!thumbnail.URL.StartsWith("http"))
                    {
                        productSheet.Thumbnail = "https://www.geonorge.no/geonetwork/srv/nor/resources.get?uuid=" + simpleMetadata.Uuid + "&access=public&fname=" + thumbnail.URL;
                    }
                    if (thumbnail.Type == "large_thumbnail")
                        break;
                }
                productSheet.BBoxNorth = simpleMetadata.BoundingBox != null ? simpleMetadata.BoundingBox.NorthBoundLatitude : null;
                productSheet.BBoxSouth = simpleMetadata.BoundingBox != null ? simpleMetadata.BoundingBox.SouthBoundLatitude : null;
                productSheet.BBoxWest = simpleMetadata.BoundingBox != null ? simpleMetadata.BoundingBox.WestBoundLongitude : null;
                productSheet.BBoxEast = simpleMetadata.BoundingBox != null ? simpleMetadata.BoundingBox.EastBoundLongitude : null;
            //productSheet.CoverageArea = !string.IsNullOrWhiteSpace(simpleMetadata.CoverageUrl) ? GetCoverageLink(simpleMetadata.CoverageUrl, uuid) : "";
                productSheet.CoverageArea = metedataExtended.CoverageUrl;
                productSheet.Projections = simpleMetadata.ReferenceSystems != null ? getProjections(simpleMetadata.ReferenceSystems) : "";
                if (!string.IsNullOrEmpty((string)metedataExtended.ServiceUuid))
                {
                    productSheet.ServiceDetails = "https://kartkatalog.geonorge.no/metadata/" + metedataExtended.ServiceUuid;
                }

            return productSheet;

        }

        private string GetDistributionFormats(List<SimpleDistributionFormat> distributionFormats)
        {
            if(distributionFormats == null || distributionFormats.Count == 0)
                return null;    

            var distinctFormats = distributionFormats.Select(d => d.Name).Distinct().ToList().OrderBy(o => o);

            string formats = null;
            foreach(var distributionFormat in distinctFormats)
            {
                formats = formats + distributionFormat + Environment.NewLine;
            }

            return formats;
        }

        public ProductSheet CreateProductSheetFromMetadata(string uuid)
        {
            return UpdateProductSheetFromMetadata(uuid, new ProductSheet());
        }

        private string CreateKeywords(IEnumerable<SimpleKeyword> keywords)
        {
            return String.Join(", ", keywords.Select(simpleKeyword => simpleKeyword.Keyword));
        }

        private Contact CreateContact(SimpleContact contact)
        {
            return new Contact
            {
                Email = contact.Email != null ? contact.Email : "" ,
                Name = contact.Name != null ? contact.Name : "",
                Organization = contact.Organization != null ? contact.Organization : ""
            };
        }


        public Logo FindLogoForOrganization(string organization)
        {
            return _dbContext.Logo.FirstOrDefault(l => l.Organization == organization); ;
        }

        public string GetLogoForOrganization(string organization)
        {
            if (organization != null)
            {
                Task<Organization> getOrganizationTask = _organizationService.GetOrganizationByName(organization);
                Organization organizationInfo = getOrganizationTask.Result;
                if (organizationInfo != null)
                {
                    return organizationInfo.LogoLargeUrl;
                }
            }

            return null;
        }

        public List<ProductSheet> FindProductSheetsForOrganization(string organization)
        {
            return _dbContext.ProductSheet.Where(ps => ps.ContactMetadata.Organization == organization).ToList();
        }

        public string GetCoverageLink(string coverageUrl, string uuid)
        {

            MD_Metadata_Type metadata = _geonorge.GetRecordByUuid(uuid);
            if (metadata == null)
                throw new InvalidOperationException("Metadata not found for uuid: " + uuid);

            var simpleMetadata = new SimpleMetadata(metadata);

            

            string CoverageLink = coverageUrl;
            var coverageStr = coverageUrl;

            if (coverageStr.IndexOf("TYPE:") != -1)
            {
                var startPos = 5;
                var endPosType = coverageStr.IndexOf("@PATH");
                var typeStr = coverageStr.Substring(startPos, endPosType - startPos);

                var endPath = coverageStr.IndexOf("@LAYER");
                var pathStr = coverageStr.Substring(endPosType + startPos + 1, endPath - (endPosType + startPos + 1));

                var startLayer = endPath + 7;
                var endLayer = coverageStr.Length - startLayer;
                var layerStr = coverageStr.Substring(startLayer, endLayer);


                int zoomLevel = simpleMetadata.BoundingBox.WestBoundLongitude != null ? ZoomLevel(simpleMetadata.BoundingBox.WestBoundLongitude, simpleMetadata.BoundingBox.SouthBoundLatitude, simpleMetadata.BoundingBox.EastBoundLongitude, simpleMetadata.BoundingBox.NorthBoundLatitude) : 7;

                if (typeStr == "WMS")
                {
                    CoverageLink = "http://norgeskart.no/geoportal/#" + zoomLevel + "/355422/6668909/l/wms/[" + RemoveQueryString(pathStr) + "]/+" + layerStr;
                }

                else if (typeStr == "WFS")
                {
                    CoverageLink = "http://norgeskart.no/geoportal/#" + zoomLevel + "/255216/6653881/l/wfs/[" + RemoveQueryString(pathStr) + "]/+" + layerStr;
                }

                else if (typeStr == "GeoJSON")
                {
                    CoverageLink = "http://norgeskart.no/geoportal/staging/#" + zoomLevel + "/355422/6668909/l/geojson/[" + RemoveQueryString(pathStr) + "]/+" + layerStr;
                }

            }

            return CoverageLink;

        }


        string RemoveQueryString(string URL)
        {
            int startQueryString = URL.IndexOf("?");

            if (startQueryString != -1)
                URL = URL.Substring(0, startQueryString);

            return URL;
        }


        public int ZoomLevel(string WestBoundLongitude, string SouthBoundLatitude, string EastBoundLongitude, string NorthBoundLatitude)
        {
            //Set Zoom to default
            double zoomLevel = 8;

            //if (string.IsNullOrEmpty(WestBoundLongitude) || string.IsNullOrEmpty(SouthBoundLatitude) ||
            //    string.IsNullOrEmpty(EastBoundLongitude) || string.IsNullOrEmpty(NorthBoundLatitude))
            //    return Convert.ToInt16(zoomLevel);

            //GeoCoordinate[] locations = new GeoCoordinate[] 
            //{ 
            //    new GeoCoordinate(Convert.ToDouble(SouthBoundLatitude), Convert.ToDouble(WestBoundLongitude)), 
            //    new GeoCoordinate(Convert.ToDouble(NorthBoundLatitude), Convert.ToDouble(EastBoundLongitude)) 
            //};

            //double maxLat = -85;
            //double minLat = 85;
            //double maxLon = -180;
            //double minLon = 180;

            ////calculate bounding rectangle
            //for (int i = 0; i < locations.Count(); i++)
            //{
            //    if (locations[i].Latitude > maxLat)
            //    {
            //        maxLat = locations[i].Latitude;
            //    }

            //    if (locations[i].Latitude < minLat)
            //    {
            //        minLat = locations[i].Latitude;
            //    }

            //    if (locations[i].Longitude > maxLon)
            //    {
            //        maxLon = locations[i].Longitude;
            //    }

            //    if (locations[i].Longitude < minLon)
            //    {
            //        minLon = locations[i].Longitude;
            //    }
            //}

            //double zoom1 = 0; double zoom2 = 0;
            //double mapWidth = 1359; //Map width in pixels
            //double mapHeight = 940; //Map height in pixels
            //int buffer = 1; //Width in pixels to use to create a buffer around the map. This is to keep pushpins from being cut off on the edge
            ////Determine the best zoom level based on the map scale and bounding coordinate information
            //if (maxLon != minLon && maxLat != minLat)
            //{
            //    //best zoom level based on map width
            //    zoom1 = Math.Log(360.0 / 256.0 * (mapWidth - 2 * buffer) / (maxLon - minLon)) / Math.Log(2);
            //    //best zoom level based on map height
            //    zoom2 = Math.Log(180.0 / 256.0 * (mapHeight - 2 * buffer) / (maxLat - minLat)) / Math.Log(2);
            //}

            ////use the most zoomed out of the two zoom levels
            //zoomLevel = (zoom1 < zoom2) ? zoom1 : zoom2;

            return Convert.ToInt16(zoomLevel);
        }


        public string getProjections(List<SimpleReferenceSystem> refsys)
        {
            string projections = "";

            for (int r = 0; r < refsys.Count; r++)
            {
              projections = projections + GetReferenceSystemName(refsys[r].CoordinateSystem);
              if (r != refsys.Count - 1)
                  projections = projections + "\r\n";
            }

            return projections;
        }



        public string GetReferenceSystemName(string coordinateSystem)
        {       
            System.Net.WebClient c = new System.Net.WebClient();
            c.Encoding = System.Text.Encoding.UTF8;
            var data = c.DownloadString(System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "api/register/epsg-koder");
            var response = Newtonsoft.Json.Linq.JObject.Parse(data);

            var refs = response["containeditems"];

            foreach (var refsys in refs)
            {

                var documentreference = refsys["documentreference"].ToString();
                if (documentreference == coordinateSystem)
                {
                    coordinateSystem = refsys["label"].ToString();
                    break;
                }
            }

            return coordinateSystem;
        }

    }
}