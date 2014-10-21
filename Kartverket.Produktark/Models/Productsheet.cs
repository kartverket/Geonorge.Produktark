﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Kartverket.Produktark.Models
{
    public class ProductSheet
    {
        public int Id { get; set; }
        
        // from metadata
        public string Uuid { get; set; }
        [DisplayName("Tittel")]
        public string Title { get; set; }
        [DisplayName("Sammendrag")]
        public string Description { get; set; }
        [DisplayName("Supplerende beskrivelse")]
        public string SupplementalDescription { get; set; }
        [DisplayName("Formål")]
        public string Purpose { get; set; }
        [DisplayName("Bruksbegrensninger")]
        public string SpecificUsage { get; set; }
        [DisplayName("Brukerrestriksjoner")]
        public string UseLimitations { get; set; }
        public Contact ContactMetadata { get; set; }
        public Contact ContactPublisher { get; set; }
        public Contact ContactOwner { get; set; }
        [DisplayName("Målestokktall")]
        public string ResolutionScale { get; set; }
        [DisplayName("Nøkkelord")]
        public string KeywordsPlace { get; set; }
        [DisplayName("Prosesshistorie")]
        public string ProcessHistory { get; set; }
        [DisplayName("Oppdateringshyppighet")]
        public string MaintenanceFrequency { get; set; }
        [DisplayName("Status")]
        public string Status { get; set; }
        [DisplayName("Format")]
        public string DistributionFormatName { get; set; }
        [DisplayName("Versjon")]
        public string DistributionFormatVersion { get; set; }
        [DisplayName("Tilgangsrestriksjoner")]
        public string AccessConstraints { get; set; }
        [DisplayName("Tegneregler(URL)")]
        public string LegendDescriptionUrl { get; set; }
        [DisplayName("Produktside(URL)")]
        public string ProductPageUrl { get; set; }
        [DisplayName("Produktspesifikasjon(URL)")]
        public string ProductSpecificationUrl { get; set; }
        
        // additional fields
        [DisplayName("Stedfestingsnøyaktighet (meter)")]
        public string PrecisionInMeters { get; set; }
        [DisplayName("Dekningsoversikt")]
        public string CoverageArea { get; set; }
        [DisplayName("Projeksjoner")]
        public string Projections { get; set; }
        [DisplayName("Tjeneste")]
        public string ServiceDetails { get; set; }
        [DisplayName("Objekttypeliste")]
        public string ListOfFeatureTypes { get; set; }
        [DisplayName("Egenskapsliste")]
        public string ListOfAttributes { get; set; }
        [DisplayName("Karteksempel fra datasett")]
        public string Thumbnail { get; set; }

        public string GetMaintenanceFrequencyTranslated()
        {
            string returnTxt = MaintenanceFrequency;
            switch (MaintenanceFrequency)
            {
                case "continual":
                    returnTxt = "Kontinuerlig";
                    break;
                case "daily":
                    returnTxt = "Daglig";
                    break;
                case "weekly":
                    returnTxt = "Ukentlig";
                    break;
                case "fortnightly":
                    returnTxt = "Annenhver uke";
                    break;
                case "monthly":
                    returnTxt = "Månedlig";
                    break;
                case "quarterly":
                    returnTxt = "Hvert kvartal";
                    break;
                case "biannually":
                    returnTxt = "Hvert halvår";
                    break;
                case "annually":
                    returnTxt = "Årlig";
                    break;
                case "asNeeded":
                    returnTxt = "Etter behov";
                    break;
                case "irregular":
                    returnTxt = "Ujevnt";
                    break;
                case "notPlanned":
                    returnTxt = "Ikke planlagt";
                    break;
                case "unknown":
                    returnTxt = "Ukjent";
                    break;
            }
            return returnTxt;
        }

        public string GetStatusTranslated()
        {
            string returnTxt = Status;
            switch (Status)
            {
                case "completed":
                    returnTxt = "Fullført";
                    break;
                case "historicalArchive":
                    returnTxt = "Arkivert";
                    break;
                case "obsolete":
                    returnTxt = "Utdatert";
                    break;
                case "onGoing":
                    returnTxt = "Kontinuerlig oppdatert";
                    break;
                case "planned":
                    returnTxt = "Planlagt";
                    break;
                case "required":
                    returnTxt = "Må oppdateres";
                    break;
                case "underDevelopment":
                    returnTxt = "Under arbeid";
                    break;
            }
            return returnTxt;
        }


    }

    public class Contact
    {
        [DisplayName("Navn")]
        public string Name { get; set; }
        [DisplayName("Epost")]
        public string Email { get; set; }
        [DisplayName("Organisasjon")]
        public string Organization { get; set; }
    }


}