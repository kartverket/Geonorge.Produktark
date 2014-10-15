using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.events;
using Kartverket.Produktark.Logging;
using System.Text.RegularExpressions;


namespace Kartverket.Produktark.Models
{
    public class PdfGenerator
    {

        private static readonly ILog Logger = LogProvider.For<PdfGenerator>();
        ProductSheet productsheet; 
        string imagePath;
        string logoPath;
        Document doc;
        BaseFont bf;
        Font font1;
        Font font2;
        Font font3;
        Font font3Bold;
        Font fontLink;
        MemoryStream output;
        PdfWriter writer;
        PdfContentByte cb;
        ColumnText ct;

        public PdfGenerator(ProductSheet productSheet, string imagePath, string logoPath)
        {
            this.productsheet = productSheet;
            this.imagePath = imagePath;
            this.logoPath = logoPath;
        }


        public Stream CreatePdf()
        {

            try
            {
                Startup();

                AddDescription();
                AddPurpose();
                AddContactOwner();
                AddResolution();
                AddCoverage();
                AddProcessHistory();
                AddMaintenance();
                AddDistribution();
                AddFeatureTypes();
                AddAttributes();
                AddLinks();

                WriteToColumns();
                

            }

            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            finally
            {

                PdfStructureTreeRoot root = writer.StructureTreeRoot;
                PdfStructureElement div = new PdfStructureElement(root, new PdfName("Div"));
                cb.BeginMarkedContentSequence(div);
                cb.EndMarkedContentSequence();


                doc.CloseDocument();
                output.Flush();
                output.Position = 0; 
                
                
            }
            return output;
        }

        private void WriteToColumns()
        {
            float gutter = -20f;

            float colwidth = (doc.Right - doc.Left - gutter) / 2;


            int status = 0;
            int i = 0;
            int count = 0;
            bool newPage = false;

            //Checks the value of status to determine if there is more text
            //If there is, status is 2, which is the value of NO_MORE_COLUMN

            while (ColumnText.HasMoreText(status))
            {
                if (i == 0)
                {
                    //Writing the first column
                    ct.SetSimpleColumn(doc.Left, doc.Bottom, doc.Right - colwidth, doc.Top);
                    i++;
                }
                else
                {
                    //write the second column
                    ct.SetSimpleColumn(doc.Left + colwidth, doc.Bottom, doc.Right, doc.Top);

                }
                //Needs to be here to prevent app from hanging
                if (!newPage)
                    ct.YLine = doc.Top - 80f;
                else
                    ct.YLine = doc.Top - 30f;

                //Commit the content of the ColumnText to the document
                //ColumnText.Go() returns NO_MORE_TEXT (1) and/or NO_MORE_COLUMN (2)
                //In other words, it fills the column until it has either run out of column, or text, or both

                status = ct.Go();

                if (++count > 1)
                {
                    count = 0;
                    if (ColumnText.HasMoreText(status))
                    {
                        doc.NewPage(); newPage = true;

                    }

                    i = 0;
                }
            }
        }

        private void Startup()
        {
            doc = new Document();

            bf = BaseFont.CreateFont(@"C:\WINDOWS\Fonts\Arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            font1 = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 22f, Font.NORMAL, BaseColor.BLACK);
            font2 = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 12f, Font.NORMAL, BaseColor.BLACK);
            font3 = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 10f, Font.NORMAL, BaseColor.BLACK);
            font3Bold = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 10f, Font.BOLD, BaseColor.BLACK);
            fontLink = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 10f, Font.UNDERLINE);
            fontLink.SetColor(0, 0, 255);

            output = new MemoryStream();

            writer = PdfWriter.GetInstance(doc, output);
            writer.CloseStream = false;
            writer.PdfVersion = PdfWriter.VERSION_1_7;
            //Tagged
            writer.SetTagged();
            writer.UserProperties = true;


            // must be initialized before document is opened.
            writer.PageEvent = new PdfHeaderFooter(productsheet, imagePath, logoPath);

            doc.Open();
            doc.AddTitle(productsheet.Title);
            doc.AddLanguage("Norwegian");

            cb = writer.DirectContent;


            ct = new ColumnText(cb);
            cb.BeginText();
            cb.SetFontAndSize(bf, 22);
            cb.SetTextMatrix(doc.Left, doc.Top - 60);
            cb.ShowText("Produktark: " + productsheet.Title);
            cb.EndText();
        }

        private void AddLinks()
        {
            ct.AddElement(writeTblFooter(""));
            ct.AddElement(writeTblHeader("LENKER"));

            Paragraph linksParagraph = new Paragraph("", font3);

            List links = new List(List.UNORDERED);
            links.SetListSymbol("\u2022");
            links.IndentationLeft = 5;

            ListItem metaData = new ListItem();
            Anchor metaDatalink = new Anchor("Link til metadata i Geonorge", fontLink);
            metaDatalink.Reference = "https://www.geonorge.no/geonetwork/?uuid=" + productsheet.Uuid;
            metaData.Add(metaDatalink);
            links.Add(metaData);

            if (!string.IsNullOrWhiteSpace(productsheet.ProductSpecificationUrl))
            {
                ListItem ProductSpecificationUrl = new ListItem();
                Anchor ProductSpecificationLink = new Anchor("Link til produktspesifikasjon", fontLink);
                ProductSpecificationLink.Reference = productsheet.ProductSpecificationUrl;
                ProductSpecificationUrl.Add(ProductSpecificationLink);
                links.Add(ProductSpecificationUrl);
            }

            if (!string.IsNullOrWhiteSpace(productsheet.LegendDescriptionUrl))
            {
                ListItem legendDescription = new ListItem();
                Anchor legendDescriptionUrl = new Anchor("Link til tegnregler", fontLink);
                legendDescriptionUrl.Reference = productsheet.LegendDescriptionUrl;
                legendDescription.Add(legendDescriptionUrl);
                links.Add(legendDescription);
            }

            if (!string.IsNullOrWhiteSpace(productsheet.ProductPageUrl))
            {
                ListItem productPage = new ListItem();
                Anchor productPageUrl = new Anchor("Link til produktside", fontLink);
                productPageUrl.Reference = productsheet.ProductPageUrl;
                productPage.Add(productPageUrl);
                links.Add(productPage);
            }

            linksParagraph.Add(links);
            ct.AddElement(linksParagraph);
        }

        private void AddAttributes()
        {
            if (!string.IsNullOrWhiteSpace(productsheet.ListOfAttributes))
            {
                ct.AddElement(writeTblFooter(""));
                ct.AddElement(writeTblHeader("EGENSKAPSLISTE"));

                List listOfAttributes = new List(List.UNORDERED);
                listOfAttributes.SetListSymbol("\u2022");
                listOfAttributes.IndentationLeft = 5;

                var Attributes = Regex.Split(productsheet.ListOfAttributes, "\r\n");
                foreach (string Attribute in Attributes)
                {
                    ListItem liAttribute = new ListItem(Attribute, font3);
                    listOfAttributes.Add(liAttribute);
                }

                ct.AddElement(listOfAttributes);
            }

        }

        private void AddFeatureTypes()
        {
            if (!string.IsNullOrWhiteSpace(productsheet.ListOfFeatureTypes))
            {
                ct.AddElement(writeTblFooter(""));
                ct.AddElement(writeTblHeader("OBJEKTTYPELISTE"));

                List listOfFeatureTypes = new List(List.UNORDERED);
                listOfFeatureTypes.SetListSymbol("\u2022");
                listOfFeatureTypes.IndentationLeft = 5;

                var FeatureTypes = Regex.Split(productsheet.ListOfFeatureTypes,"\r\n");
                foreach (string FeatureType in FeatureTypes) {
                    ListItem liFeature = new ListItem(FeatureType, font3);
                    listOfFeatureTypes.Add(liFeature);
                }

                ct.AddElement(listOfFeatureTypes);

            }
        }

        private void AddDistribution()
        {
            ct.AddElement(writeTblHeader("LEVERANSEBESKRIVELSE"));

            if (!string.IsNullOrWhiteSpace(productsheet.DistributionFormatName))
            {
                Phrase distributionFormatHeading = new Phrase("Format (versjon)", font3Bold);
                ct.AddElement(distributionFormatHeading);

                Paragraph distributionFormat = new Paragraph("", font3);
                List format = new List(List.UNORDERED);
                format.SetListSymbol("\u2022");
                format.IndentationLeft = 5;
                string formatVersion = productsheet.DistributionFormatName;
                if (!string.IsNullOrWhiteSpace(productsheet.DistributionFormatVersion))
                {
                    formatVersion = formatVersion + ", " + productsheet.DistributionFormatVersion;
                }

                ListItem liFormat = new ListItem(formatVersion, font3);
                format.Add(liFormat);
                distributionFormat.Add(format);
                ct.AddElement(distributionFormat);
            }

            if (!string.IsNullOrWhiteSpace(productsheet.Projections))
            {
                Phrase projectionsHeading = new Phrase("\n" + "Projeksjoner", font3Bold);
                ct.AddElement(projectionsHeading);
                Phrase projections = new Phrase(productsheet.Projections, font3);
                ct.AddElement(projections);
            }

            if (!string.IsNullOrWhiteSpace(productsheet.AccessConstraints))
            {
                Phrase accessConstraintsHeading = new Phrase("\n" + "Tilgangsrestriksjoner", font3Bold);
                ct.AddElement(accessConstraintsHeading);
                Phrase accessConstraints = new Phrase(productsheet.AccessConstraints, font3);
                ct.AddElement(accessConstraints);
            }

            if (!string.IsNullOrWhiteSpace(productsheet.ServiceDetails))
            {
                Phrase serviceDetailsHeading = new Phrase("\n" + "Tjeneste", font3Bold);
                ct.AddElement(serviceDetailsHeading);

                Phrase serviceDetails = new Phrase(productsheet.ServiceDetails, font3);
                ct.AddElement(serviceDetails);
            }
        }

        private void AddMaintenance()
        {
            ct.AddElement(writeTblHeader("AJOURFØRING OG OPPDATERING"));

            if (!string.IsNullOrWhiteSpace(productsheet.MaintenanceFrequency))
            {
                Phrase maintenanceFrequency = new Phrase(getMaintenanceFrequencyValue(productsheet.MaintenanceFrequency), font3);
                ct.AddElement(maintenanceFrequency);
            }

            if (!string.IsNullOrWhiteSpace(productsheet.Status))
            {
                Phrase statusHeading = new Phrase("Status", font3Bold);
                ct.AddElement(statusHeading);
                Phrase statusValue = new Phrase(getStatusValue(productsheet.Status), font3);
                ct.AddElement(statusValue);
            }

            ct.AddElement(writeTblFooter(""));
        }

        private void AddProcessHistory()
        {
            ct.AddElement(writeTblHeader("KILDER OG METODE"));

            if (!string.IsNullOrWhiteSpace(productsheet.ProcessHistory))
            {
                Phrase processHistory = new Phrase(productsheet.ProcessHistory, font3);
                ct.AddElement(processHistory);
            }

            ct.AddElement(writeTblFooter(""));
        }

        private void AddCoverage()
        {
            ct.AddElement(writeTblHeader("UTSTREKNINGSINFORMASJON"));

            if (productsheet.KeywordsPlace != null)
            {
                Phrase keywordsPlaceHeading = new Phrase("Utstrekningsbeskrivelse", font3Bold);
                ct.AddElement(keywordsPlaceHeading);
                ct.AddElement(new Phrase(productsheet.KeywordsPlace, font3));
            }

            if (!string.IsNullOrWhiteSpace(productsheet.CoverageArea))
            {
                Phrase coverageAreaHeading = new Phrase("Dekningsoversikt", font3Bold);
                ct.AddElement(coverageAreaHeading);
                Phrase coverageArea = new Phrase(productsheet.CoverageArea, font3);
                ct.AddElement(coverageArea);
            }

            ct.AddElement(writeTblFooter(""));
        }

        private void AddResolution()
        {
            ct.AddElement(writeTblHeader("DATASETTOPPLØSNING"));

            if (!string.IsNullOrWhiteSpace(productsheet.ResolutionScale))
            {
                Chunk resolutionScaleHeading = new Chunk("Målestokktall: ", font3Bold);
                Chunk resolutionScaleValue = new Chunk(productsheet.ResolutionScale, font3);
                Phrase resolutionScale = new Phrase();
                resolutionScale.Add(resolutionScaleHeading);
                resolutionScale.Add(resolutionScaleValue);
                ct.AddElement(resolutionScale);
            }

            if (!string.IsNullOrWhiteSpace(productsheet.PrecisionInMeters))
            {
                Chunk precisionInMetersHeading = new Chunk("Stedfestingsnøyaktighet (meter): ", font3Bold);
                Chunk precisionInMeters = new Chunk(productsheet.PrecisionInMeters, font3);

                Phrase precision = new Phrase();
                precision.Add(precisionInMetersHeading);
                precision.Add(precisionInMeters);
                ct.AddElement(precision);
            }

            ct.AddElement(writeTblFooter(""));
        }

        private void AddContactOwner()
        {
            ct.AddElement(writeTblHeader("EIER/KONTAKTPERSON"));

            Paragraph contactOwnerOrganization = new Paragraph(productsheet.ContactOwner.Organization, font3);

            Phrase contactPublisher = new Phrase();

            if (!string.IsNullOrWhiteSpace(productsheet.ContactPublisher.Name))
            {
                Chunk contactPublisherHeading = new Chunk("Datateknisk: ", font3Bold);
                Chunk contactPublisherName = new Chunk(productsheet.ContactPublisher.Name, font3);
                contactPublisher.Add(contactPublisherHeading);
                contactPublisher.Add(contactPublisherName);
                if (!string.IsNullOrWhiteSpace(productsheet.ContactPublisher.Email))
                {
                    Chunk contactPublisherEmail = new Chunk(", " + productsheet.ContactPublisher.Email, font3);
                    contactPublisher.Add(contactPublisherEmail);
                }
            }
            ct.AddElement(contactOwnerOrganization);
            ct.AddElement(contactPublisher);

            Phrase contactOwner = new Phrase();

            if (!string.IsNullOrWhiteSpace(productsheet.ContactOwner.Name))
            {
                Chunk contactOwnerHeading = new Chunk("Fagekspert: ", font3Bold);
                Chunk contactOwnerName = new Chunk(productsheet.ContactOwner.Name, font3);
                contactOwner.Add(contactOwnerHeading);
                contactOwner.Add(contactOwnerName);
                if (!string.IsNullOrWhiteSpace(productsheet.ContactOwner.Email))
                {
                    Chunk contactOwnerEmail = new Chunk(", " + productsheet.ContactOwner.Email, font3);
                    contactOwner.Add(contactOwnerEmail);
                }
            }

            ct.AddElement(contactOwner);

            ct.AddElement(writeTblFooter(""));
        }

        private void AddPurpose()
        {
            ct.AddElement(writeTblHeader("FORMÅL/BRUKSOMRÅDE"));

            if (!string.IsNullOrWhiteSpace(productsheet.Purpose))
            {
                Paragraph purposeHeading = new Paragraph(productsheet.Purpose, font3);
                ct.AddElement(purposeHeading);
            }

            if (!string.IsNullOrWhiteSpace(productsheet.SpecificUsage))
            {
                Paragraph specificUsage = new Paragraph("\n" + productsheet.SpecificUsage, font3);

                ct.AddElement(specificUsage);
            }

            if (!string.IsNullOrWhiteSpace(productsheet.UseLimitations))
            {
                Paragraph useLimitations = new Paragraph("\n" + productsheet.UseLimitations, font3);
                ct.AddElement(useLimitations);
            }

            ct.AddElement(writeTblFooter(""));
        }

        private void AddDescription()
        {
            ct.AddElement(writeTblHeader("BESKRIVELSE"));
            Paragraph descriptionHeading = new Paragraph();
            var imageMap = Image.GetInstance(imagePath + "/logo_karteksempel.png");
            imageMap.Alt = "Bilde av karteksempel";
            imageMap.ScalePercent(50);
            imageMap.SpacingBefore = 4;
            ct.AddElement(imageMap);
            ct.AddElement(descriptionHeading);

            if (!string.IsNullOrWhiteSpace(productsheet.Description))
            {
                Paragraph Description = new Paragraph(productsheet.Description, font3);
                ct.AddElement(Description);
            }
            if (!string.IsNullOrWhiteSpace(productsheet.SupplementalDescription))
            {
                Paragraph supplementalDescription = new Paragraph("\n" + productsheet.SupplementalDescription, font3);
                ct.AddElement(supplementalDescription);
            }

            ct.AddElement(writeTblFooter(""));
        }

        PdfPTable writeTblHeader(string txt)
        {
            Font fontHead = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 10f, Font.BOLD, BaseColor.WHITE);
            
            Anchor content = new Anchor(txt, fontHead);
            content.Name = txt;

            PdfOutline root = writer.RootOutline;
            PdfOutline mbot = new PdfOutline(root, PdfAction.GotoLocalPage(txt, false), txt); 

            PdfPTable table = new PdfPTable(1);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(content);
            cell.BackgroundColor = new BaseColor(0, 150, 0);
            cell.Border = 0;
            cell.PaddingTop = 0;
            cell.PaddingBottom = 3;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);
            return table;
        
        }

        PdfPTable writeTblFooter(string txt)
        {

            PdfPTable table = new PdfPTable(1);
            table.WidthPercentage = 100;
            //table.SpacingBefore = 2;
            table.SpacingAfter = 20;
            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            table.AddCell(cell);
            return table;

        }

        string getMaintenanceFrequencyValue(string mf)
        {
            string returnTxt = mf;
            switch(mf){
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

        string getStatusValue(string status)
        {
            string returnTxt = status;
            switch (status)
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
}