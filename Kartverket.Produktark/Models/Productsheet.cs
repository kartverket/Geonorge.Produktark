using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Produktark.Models
{
    public class ProductSheet
    {
        public int Id { get; set; }
        public string Uuid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SupplementalDescription { get; set; }
        public string Purpose { get; set; }
        public string SpecificUsage { get; set; }
        public string UseLimitations { get; set; }
        
    }
}