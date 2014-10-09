using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Produktark.Models
{
    public class ProductSheet
    {
        public int Id { get; set; }
        
        // from metadata
        public string Uuid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SupplementalDescription { get; set; }
        public string Purpose { get; set; }
        public string SpecificUsage { get; set; }
        public string UseLimitations { get; set; }
        public Contact ContactMetadata { get; set; }
        public Contact ContactPublisher { get; set; }
        public Contact ContactOwner { get; set; }
        public string ResolutionScale { get; set; }
        public string KeywordsPlace { get; set; }
        public string ProcessHistory { get; set; }
        public string MaintenanceFrequency { get; set; }
        public string Status { get; set; }
        public string DistributionFormatName { get; set; }
        public string DistributionFormatVersion { get; set; }
        public string AccessConstraints { get; set; }
        public string LegendDescriptionUrl { get; set; }
        public string ProductPageUrl { get; set; }
        public string ProductSpecificationUrl { get; set; }
        
        // additional fields
        public string PrecisionInMeters { get; set; }
        public string CoverageArea { get; set; }
        public string Projections { get; set; }
        public string ServiceDetails { get; set; }
        public string ListOfFeatureTypes { get; set; }
        public string ListOfAttributes { get; set; }
    }

    public class Contact
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Organization { get; set; }
    }
}