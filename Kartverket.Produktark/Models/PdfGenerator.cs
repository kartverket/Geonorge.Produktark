﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.events;


namespace Kartverket.Produktark.Models
{
    public class PdfGenerator
    {
        public Stream CreatePdf(ProductSheet productsheet, string imagePath)
        {
            Document doc = new Document();

            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Font font1 = new Font(Font.NORMAL, 22f);
            Font font2 = new Font(Font.NORMAL, 12f);
            Font font3 = new Font(Font.NORMAL, 9f);
            Font font3_bold = new Font();
            font3_bold.SetStyle("bold");
            font3_bold.Size = 9f;

            Font fontLink = new Font();
            fontLink.SetFamily("Times");
            fontLink.SetStyle("underline");
            fontLink.Size = 9f;
            fontLink.SetColor(0, 0, 255);

            MemoryStream output = new MemoryStream();

            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, output);
                writer.CloseStream = false;

                // must be initialized before document is opened.
                writer.PageEvent = new PdfHeaderFooter(imagePath, productsheet);

                doc.Open();

                PdfContentByte cb = writer.DirectContent;
                ColumnText ct = new ColumnText(cb);

                cb.BeginText();
                cb.SetFontAndSize(bf, 22);
                cb.SetTextMatrix(doc.Left, doc.Top-60);
                cb.ShowText("Produktark: " + productsheet.Title);
                cb.EndText();


                ct.AddElement(writeTblHeader("BESKRIVELSE"));
                Paragraph descriptionHeading = new Paragraph();
                var imageMap = Image.GetInstance(imagePath + "/logo_karteksempel.png");
                imageMap.ScalePercent(50);
                ct.AddElement(imageMap);
                ct.AddElement(descriptionHeading);

                if (!string.IsNullOrWhiteSpace(productsheet.Description)) {
                    Paragraph Description = new Paragraph(productsheet.Description, font3);
                    ct.AddElement(Description);
                }
                if (!string.IsNullOrWhiteSpace(productsheet.SupplementalDescription))
                {
                    Paragraph supplementalDescription = new Paragraph(productsheet.SupplementalDescription, font3);
                    ct.AddElement(supplementalDescription);
                }

                ct.AddElement(writeTblHeader("FORMÅL/BRUKSOMRÅDE"));

                if (!string.IsNullOrWhiteSpace(productsheet.Purpose))
                {
                    Paragraph purposeHeading = new Paragraph(productsheet.Purpose, font3);
                    ct.AddElement(purposeHeading);
                }

                if (!string.IsNullOrWhiteSpace(productsheet.SpecificUsage)) {
                    Paragraph spesificUsage = new Paragraph(productsheet.SpecificUsage, font3);
                ct.AddElement(spesificUsage);
                }

                if (!string.IsNullOrWhiteSpace(productsheet.UseLimitations))
                {
                    Paragraph useLimitations = new Paragraph(productsheet.UseLimitations, font3);
                    ct.AddElement(useLimitations);
                }

                ct.AddElement(writeTblHeader("EIER/KONTAKTPERSON"));

                Paragraph contactOwnerOrganization = new Paragraph(productsheet.ContactOwner.Organization, font3);
                
                Phrase contactPublisher = new Phrase();

                if (!string.IsNullOrWhiteSpace(productsheet.ContactPublisher.Name))
                {
                    Chunk contactPublisherHeading = new Chunk("Datateknisk: ", font3_bold);
                    Chunk contactPublisherName = new Chunk(productsheet.ContactPublisher.Name, font3);
                    contactPublisher.Add(contactPublisherHeading);
                    contactPublisher.Add(contactPublisherName);
                    if (!string.IsNullOrWhiteSpace(productsheet.ContactPublisher.Email))
                    {
                    Chunk contactPublisherEmail = new Chunk(", "+productsheet.ContactPublisher.Email, font3);
                    contactPublisher.Add(contactPublisherEmail);
                    }
                }
                ct.AddElement(contactOwnerOrganization);
                ct.AddElement(contactPublisher);

                Phrase contactOwner = new Phrase();

                if (!string.IsNullOrWhiteSpace(productsheet.ContactOwner.Name)) {
                Chunk contactOwnerHeading = new Chunk("Fagekspert: ", font3_bold);
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


                ct.AddElement(writeTblHeader("DATASETTOPPLØSNING"));

                if (!string.IsNullOrWhiteSpace(productsheet.ResolutionScale)) { 
                Chunk resolutionScale_heading = new Chunk("Målestokktall: ", font3_bold);
                Chunk resolutionScaleValue = new Chunk(productsheet.ResolutionScale, font3);
                Phrase resolutionScale = new Phrase();
                resolutionScale.Add(resolutionScale_heading);
                resolutionScale.Add(resolutionScaleValue);
                ct.AddElement(resolutionScale);
                }


                /*Chunk ds_stedfesting_lede = new Chunk("Stedfestingsnøyaktighet (meter): ", font3_bold);
                Chunk ds_stedfesting = new Chunk("”Mangler, kan vente”", font3);

                Phrase p_stedfesting = new Phrase();
                p_stedfesting.Add(ds_stedfesting_lede);
                p_stedfesting.Add(ds_stedfesting);
                ct.AddElement(p_stedfesting);*/

                ct.AddElement(writeTblHeader("UTSTREKNINGSINFORMASJON"));

                if (productsheet.KeywordsPlace!=null) {
                Phrase keywordsPlaceHeading = new Phrase("Utstrekningsbeskrivelse", font3_bold);
                ct.AddElement(keywordsPlaceHeading);
                    foreach (var keyword in productsheet.KeywordsPlace){
                        Phrase keywordValue = new Phrase(keyword, font3);
                        ct.AddElement(keywordValue);
                    }          
                }

                /*Phrase p_dekningsoversikt_overskrift = new Phrase("Dekningsoversikt", font3_bold);
                ct.AddElement(p_dekningsoversikt_overskrift);
                Phrase p_dekningsoversikt = new Phrase("”MANGLER”", font3);
                ct.AddElement(p_dekningsoversikt);*/

                ct.AddElement(writeTblHeader("KILDER OG METODE"));

                if (!string.IsNullOrWhiteSpace(productsheet.ProcessHistory))
                {
                    Phrase processHistory = new Phrase(productsheet.ProcessHistory, font3);
                    ct.AddElement(processHistory);
                }

                ct.AddElement(writeTblHeader("AJOURFØRING OG OPPDATERING"));

                if (!string.IsNullOrWhiteSpace(productsheet.ProcessHistory)) {
                Phrase maintenanceFrequency = new Phrase(productsheet.ProcessHistory, font3);
                ct.AddElement(maintenanceFrequency);
                }

                if (!string.IsNullOrWhiteSpace(productsheet.Status))
                {
                    Phrase statusHeading = new Phrase("Status", font3_bold);
                    ct.AddElement(statusHeading);
                    Phrase statusValue = new Phrase(productsheet.Status, font3);
                    ct.AddElement(statusValue);
                }

                ct.AddElement(writeTblHeader("LEVERANSEBESKRIVELSE"));

                if (!string.IsNullOrWhiteSpace(productsheet.DistributionFormatName)) {
                Phrase distributionFormatHeading = new Phrase("Format (versjon)", font3_bold);
                ct.AddElement(distributionFormatHeading);

                Paragraph distributionFormat = new Paragraph("", font3);
                List format = new List(List.UNORDERED);
                format.SetListSymbol("\u2022");
                ListItem liFormat = new ListItem(productsheet.DistributionFormatName + ", " + productsheet.DistributionFormatVersion, font3);
                format.Add(liFormat);
                distributionFormat.Add(format);
                ct.AddElement(distributionFormat);
            }


                /*Phrase p_projeksjoner_overskrift = new Phrase("Projeksjoner", font3_bold);
                ct.AddElement(p_projeksjoner_overskrift);
                Phrase p_projeksjoner = new Phrase("”MANGLER”", font3);
                ct.AddElement(p_projeksjoner);*/

                if (!string.IsNullOrWhiteSpace(productsheet.AccessConstraints))
                {
                    Phrase accessConstraintsHeading = new Phrase("Tilgangsrestriksjoner", font3_bold);
                    ct.AddElement(accessConstraintsHeading);
                    Phrase accessConstraints = new Phrase(productsheet.AccessConstraints, font3);
                    ct.AddElement(accessConstraints);
                }

                //Phrase p_tjeneste_overskrift = new Phrase("Tjeneste", font3_bold);
                //ct.AddElement(p_tjeneste_overskrift);

                //Phrase p_tjeneste_info = new Phrase("Sett inn tjeneste informasjon, evt. forklaring til deltema innenfor tjenester", font3);
                //ct.AddElement(p_tjeneste_info);

                //Phrase p_tjeneste_link = new Phrase();
                //Anchor anchor_tjeneste = new Anchor("Sett inn lenke til tjeneste", fontLink);
                //anchor_tjeneste.Reference = "http://www.geonorge.no";
                //p_tjeneste_link.Add(anchor_tjeneste);
                //ct.AddElement(p_tjeneste_link);

                //Phrase p_tjeneste_kall = new Phrase("Beskrivelse av kall for datasettets tjenester – eks. GetMap, GetFeatureInfo", font3);
                //ct.AddElement(p_tjeneste_kall);

                //ct.AddElement(writeTblHeader("OBJEKTTYPELISTE"));

                //Paragraph p_objekt_type = new Paragraph("", font3);
                //List l_objekt_type = new List(List.UNORDERED);
                //l_objekt_type.SetListSymbol("\u2022");
                //ListItem liObjekt_type = new ListItem("<sett inn objekttype og forklaring>", font3);
                //l_objekt_type.Add(liObjekt_type);
                //p_objekt_type.Add(l_objekt_type);
                //ct.AddElement(p_objekt_type);


                //ct.AddElement(writeTblHeader("EGENSKAPSLISTE"));

                //Paragraph p_egenskap = new Paragraph("", font3);
                //List l_egenskap = new List(List.UNORDERED);
                //l_egenskap.SetListSymbol("\u2022");
                //ListItem liEgenskap = new ListItem("<sett inn egenskap og forklaring>", font3);
                //l_egenskap.Add(liEgenskap);
                //p_egenskap.Add(l_egenskap);
                //ct.AddElement(p_egenskap);

                ct.AddElement(writeTblHeader("LENKER"));

                Paragraph linksParagraph = new Paragraph("", font3);

                List links = new List(List.UNORDERED);
                links.SetListSymbol("\u2022");

                ListItem metaData = new ListItem();
                Anchor metaDatalink = new Anchor("Link til metadata i Geonorge", fontLink);
                metaDatalink.Reference = "https://www.geonorge.no/geonetwork/?uuid="+productsheet.Uuid;
                metaData.Add(metaDatalink);

                ListItem ProductSpecificationUrl = new ListItem();
                Anchor ProductSpecificationLink = new Anchor("Link til produktspesifikasjon", fontLink);
                ProductSpecificationLink.Reference = productsheet.ProductSpecificationUrl;
                ProductSpecificationUrl.Add(ProductSpecificationLink);

                ListItem legendDescription = new ListItem();
                Anchor legendDescriptionUrl = new Anchor("Link til tegnregler", fontLink);
                legendDescriptionUrl.Reference = productsheet.LegendDescriptionUrl;
                legendDescription.Add(legendDescriptionUrl);

                ListItem productPage = new ListItem();
                Anchor productPageUrl = new Anchor("Link til produktside", fontLink);
                productPageUrl.Reference = productsheet.ProductPageUrl;
                productPage.Add(productPageUrl);

                links.Add(metaData);
                links.Add(ProductSpecificationUrl);
                links.Add(legendDescription);
                links.Add(productPage);
                linksParagraph.Add(links);
                ct.AddElement(linksParagraph);


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
                    if(!newPage)
                    ct.YLine = doc.Top-80f;
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

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            finally
            {
                doc.Close();
                output.Flush(); 
                output.Position = 0; 
            }
            return output;
        }

        PdfPTable writeTblHeader(string txt)
        {

            PdfPTable table = new PdfPTable(1);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase(txt, new Font(Font.NORMAL, 12f, Font.BOLD, BaseColor.WHITE)));
            cell.BackgroundColor = new BaseColor(0, 150, 0);
            cell.Border = 0;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            table.AddCell(cell);
            return table;
        
        }
    }
}