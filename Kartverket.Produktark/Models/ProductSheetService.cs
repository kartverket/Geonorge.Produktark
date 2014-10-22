using System;
using System.Collections.Generic;
using System.Linq;
using GeoNorgeAPI;
using www.opengis.net;
using System.Data.Entity.Infrastructure;

namespace Kartverket.Produktark.Models
{
    public class ProductSheetService : IProductSheetService
    {
        private readonly GeoNorge _geonorge;
        private readonly ProductSheetContext _dbContext;

        public ProductSheetService(GeoNorge geoNorge, ProductSheetContext dbContext)
        {
            _geonorge = geoNorge;
            _dbContext = dbContext;
        }

        public ProductSheet UpdateProductSheetFromMetadata(string uuid, ProductSheet productSheet) {
            MD_Metadata_Type metadata = _geonorge.GetRecordByUuid(uuid);
            if (metadata == null)
                throw new InvalidOperationException("Metadata not found for uuid: " + uuid);

            var simpleMetadata = new SimpleMetadata(metadata);

           
                productSheet.Uuid = simpleMetadata.Uuid;
                productSheet.Title = simpleMetadata.Title;
                productSheet.Description = simpleMetadata.Abstract;
                productSheet.SupplementalDescription = simpleMetadata.SupplementalDescription;
                productSheet.Purpose = simpleMetadata.Purpose;
                productSheet.SpecificUsage = simpleMetadata.SpecificUsage;
                productSheet.UseLimitations = simpleMetadata.Constraints != null ? simpleMetadata.Constraints.UseLimitations : null;
                productSheet.ContactMetadata = simpleMetadata.ContactMetadata != null ? CreateContact(simpleMetadata.ContactMetadata) : new Contact();
                productSheet.ContactPublisher = simpleMetadata.ContactPublisher != null ? CreateContact(simpleMetadata.ContactPublisher) : new Contact();
                productSheet.ContactOwner = simpleMetadata.ContactOwner != null ? CreateContact(simpleMetadata.ContactOwner) : new Contact();
                productSheet.ResolutionScale = simpleMetadata.ResolutionScale;
                productSheet.KeywordsPlace = CreateKeywords(SimpleKeyword.Filter(simpleMetadata.Keywords, SimpleKeyword.TYPE_PLACE, null));
                productSheet.ProcessHistory = simpleMetadata.ProcessHistory;
                productSheet.MaintenanceFrequency = simpleMetadata.MaintenanceFrequency;
                productSheet.Status = simpleMetadata.Status;
                productSheet.DistributionFormatName = simpleMetadata.DistributionFormat != null ? simpleMetadata.DistributionFormat.Name : null;
                productSheet.DistributionFormatVersion = simpleMetadata.DistributionFormat != null ? simpleMetadata.DistributionFormat.Version : null;
                productSheet.AccessConstraints = simpleMetadata.Constraints != null ? simpleMetadata.Constraints.AccessConstraints : null;
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

            return productSheet;

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

        public List<ProductSheet> FindProductSheetsForOrganization(string organization)
        {
            return _dbContext.ProductSheet.Where(ps => ps.ContactMetadata.Organization == organization).ToList();
        }

    }
}