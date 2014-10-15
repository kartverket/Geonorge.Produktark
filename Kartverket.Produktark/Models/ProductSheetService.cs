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

        public ProductSheet CreateProductSheetFromMetadata(string uuid)
        {
            MD_Metadata_Type metadata = _geonorge.GetRecordByUuid(uuid);
            if (metadata == null)
                throw new InvalidOperationException("Metadata not found for uuid: " + uuid);

            var simpleMetadata = new SimpleMetadata(metadata);

            return new ProductSheet
            {
                Uuid = simpleMetadata.Uuid,
                Title = simpleMetadata.Title,
                Description = simpleMetadata.Abstract,
                SupplementalDescription = simpleMetadata.SupplementalDescription,
                Purpose = simpleMetadata.Purpose,
                SpecificUsage = simpleMetadata.SpecificUsage,
                UseLimitations = simpleMetadata.Constraints != null ? simpleMetadata.Constraints.UseLimitations : null,
                ContactMetadata = CreateContact(simpleMetadata.ContactMetadata),
                ContactPublisher = CreateContact(simpleMetadata.ContactPublisher),
                ContactOwner = CreateContact(simpleMetadata.ContactOwner),
                ResolutionScale = simpleMetadata.ResolutionScale,
                KeywordsPlace = CreateKeywords(SimpleKeyword.Filter(simpleMetadata.Keywords, SimpleKeyword.TYPE_PLACE, null)),
                ProcessHistory = simpleMetadata.ProcessHistory,
                MaintenanceFrequency = simpleMetadata.MaintenanceFrequency,
                Status = simpleMetadata.Status,
                DistributionFormatName = simpleMetadata.DistributionFormat != null ? simpleMetadata.DistributionFormat.Name : null,
                DistributionFormatVersion = simpleMetadata.DistributionFormat != null ? simpleMetadata.DistributionFormat.Version : null,
                AccessConstraints = simpleMetadata.Constraints != null ? simpleMetadata.Constraints.AccessConstraints : null,
                LegendDescriptionUrl = simpleMetadata.LegendDescriptionUrl,
                ProductPageUrl = simpleMetadata.ProductPageUrl,
                ProductSpecificationUrl = simpleMetadata.ProductSpecificationUrl
            };
        }

        private string CreateKeywords(IEnumerable<SimpleKeyword> keywords)
        {
            return String.Join(", ", keywords.Select(simpleKeyword => simpleKeyword.Keyword));
        }

        private Contact CreateContact(SimpleContact contact)
        {
            return new Contact
            {
                Email = contact.Email,
                Name = contact.Name,
                Organization = contact.Organization
            };
        }


        public Logo FindLogoForOrganization(string organization)
        {
            return _dbContext.Logo.FirstOrDefault(l => l.Organization == organization); ;
        }
    }
}