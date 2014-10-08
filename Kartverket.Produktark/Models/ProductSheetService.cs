using System;
using System.Collections.Generic;
using System.Linq;
using GeoNorgeAPI;
using www.opengis.net;

namespace Kartverket.Produktark.Models
{
    public class ProductSheetService
    {
        private readonly GeoNorge _geonorge;

        public ProductSheetService()
        {
            _geonorge = new GeoNorge();
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

        private List<string> CreateKeywords(IEnumerable<SimpleKeyword> keywords)
        {
            return keywords.Select(simpleKeyword => simpleKeyword.Keyword).ToList();
        }

        private Contact CreateContact(SimpleContact contactMetadata)
        {
            return new Contact
            {
                Email = contactMetadata.Email,
                Name = contactMetadata.Name
            };
        }
    }
}