using System;
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
        public Stream CreatePdf(int? productsheet,string imagePath)
        {
            Document doc = new Document();

            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Font font1 = new Font(Font.NORMAL, 22f);
            Font font2 = new Font(Font.NORMAL, 12f);
            Font font3 = new Font(Font.NORMAL, 9f);
            Font font3_bold = new Font();
            font3_bold.SetStyle("bold");
            font3_bold.Size = 9f;

            Font font_link = new Font();
            font_link.SetFamily("Helvetica");
            font_link.SetStyle("underline");
            font_link.Size = 9f;
            font_link.SetColor(0, 0, 255);

            MemoryStream output = new MemoryStream();


            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, output);
                writer.CloseStream = false;

                // must be initialized before document is opened.
                writer.PageEvent = new PdfHeaderFooter(imagePath);

                doc.Open();

                PdfContentByte cb = writer.DirectContent;
                ColumnText ct = new ColumnText(cb);

                cb.BeginText();
                cb.SetFontAndSize(bf, 22);
                cb.SetTextMatrix(doc.Left+35, doc.Top-60);
                cb.ShowText("Produktark: <sett inn ”Navn” på datasett>");
                cb.EndText();


                ct.AddElement(writeTblHeader("BESKRIVELSE"));
                Paragraph p_description = new Paragraph();
                var image_mapexample = Image.GetInstance(imagePath + "/logo_karteksempel.png");
                image_mapexample.ScalePercent(50);
                ct.AddElement(image_mapexample);
                ct.AddElement(p_description);

                Paragraph p_sammendrag = new Paragraph("”Sammendrag”",font3);
                ct.AddElement(p_sammendrag);

                Paragraph p_suppelerende_beskrivelse = new Paragraph("”Supplerende beskrivelse”", font3);
                ct.AddElement(p_suppelerende_beskrivelse);

                ct.AddElement(writeTblHeader("FORMÅL/BRUKSOMRÅDE"));

                Paragraph p_formaal = new Paragraph("”Formål”", font3);
                ct.AddElement(p_formaal);

                Paragraph p_bruksomraade = new Paragraph("”Bruksområde”", font3);
                ct.AddElement(p_bruksomraade);

                Paragraph p_bruksbegrensninger = new Paragraph("”Bruksbegrensninger”", font3);
                ct.AddElement(p_bruksbegrensninger);

                ct.AddElement(writeTblHeader("EIER/KONTAKTPERSON"));

                Paragraph p_etat_faglig_kontakt = new Paragraph("”Navn” på etat for faglig kontakt", font3);

                Chunk p_etat_kontakt_teknisk_lede = new Chunk("Datateknisk: ", font3_bold);
                Chunk p_etat_kontakt_teknisk_navn = new Chunk("”Navn” på teknisk kontakt", font3);
                Chunk p_etat_kontakt_teknisk_mail = new Chunk(", ”e-post”", font3);

                Phrase p_etat_faglig = new Phrase();
                p_etat_faglig.Add(p_etat_kontakt_teknisk_lede);
                p_etat_faglig.Add(p_etat_kontakt_teknisk_navn);
                p_etat_faglig.Add(p_etat_kontakt_teknisk_mail);

                ct.AddElement(p_etat_faglig_kontakt);
                ct.AddElement(p_etat_faglig);


                Chunk p_etat_kontakt_fag_lede = new Chunk("Fagekspert: ", font3_bold);
                Chunk p_etat_kontakt_fag_navn = new Chunk("”Navn på faglig kontaktperson”", font3);
                Chunk p_etat_kontakt_fag_mail = new Chunk(", ”e-post”", font3);

                Phrase p_etat_fag = new Phrase();
                p_etat_fag.Add(p_etat_kontakt_fag_lede);
                p_etat_fag.Add(p_etat_kontakt_fag_navn);
                p_etat_fag.Add(p_etat_kontakt_fag_mail);

                ct.AddElement(p_etat_fag);

                ct.AddElement(writeTblHeader("DATASETTOPPLØSNING"));

                Chunk ds_maalestokktall_lede = new Chunk("Målestokktall: ", font3_bold);
                Chunk ds_maalestokktall = new Chunk("”målestokktall”", font3);

                Phrase p_maalestokk = new Phrase();
                p_maalestokk.Add(ds_maalestokktall_lede);
                p_maalestokk.Add(ds_maalestokktall);
                ct.AddElement(p_maalestokk);

                Chunk ds_stedfesting_lede = new Chunk("Stedfestingsnøyaktighet (meter): ", font3_bold);
                Chunk ds_stedfesting = new Chunk("”Mangler, kan vente”", font3);

                Phrase p_stedfesting = new Phrase();
                p_stedfesting.Add(ds_stedfesting_lede);
                p_stedfesting.Add(ds_stedfesting);
                ct.AddElement(p_stedfesting);

                ct.AddElement(writeTblHeader("UTSTREKNINGSINFORMASJON"));

                Phrase p_utstrekningsbeskrivelse_overskrift = new Phrase("Utstrekningsbeskrivelse", font3_bold);
                ct.AddElement(p_utstrekningsbeskrivelse_overskrift);
                Phrase p_utstrekningsbeskrivelse = new Phrase("”Nøkkelord” sted", font3);
                ct.AddElement(p_utstrekningsbeskrivelse);

                Phrase p_dekningsoversikt_overskrift = new Phrase("Dekningsoversikt", font3_bold);
                ct.AddElement(p_dekningsoversikt_overskrift);
                Phrase p_dekningsoversikt = new Phrase("”MANGLER”", font3);
                ct.AddElement(p_dekningsoversikt);

                ct.AddElement(writeTblHeader("KILDER OG METODE"));

                Phrase p_prosesshistorie = new Phrase("”Prosesshistorie”", font3);
                ct.AddElement(p_prosesshistorie);

                ct.AddElement(writeTblHeader("AJOURFØRING OG OPPDATERING"));

                Phrase p_oppdateringshyppighet = new Phrase("”Oppdateringshyppighet”", font3);
                ct.AddElement(p_oppdateringshyppighet);

                Phrase p_status_overskrift = new Phrase("Status", font3_bold);
                ct.AddElement(p_status_overskrift);
                Phrase p_status = new Phrase("”Status”", font3);
                ct.AddElement(p_status);

                ct.AddElement(writeTblHeader("LEVERANSEBESKRIVELSE"));

                Phrase p_format_overskrift = new Phrase("Format (versjon)", font3_bold);
                ct.AddElement(p_format_overskrift);

                Paragraph p_format = new Paragraph("", font3);
                List format = new List(List.UNORDERED);
                format.SetListSymbol("\u2022");
                ListItem liFormat = new ListItem("”Format”, ”Versjon”",font3);
                format.Add(liFormat);
                p_format.Add(format);
                ct.AddElement(p_format);


                Phrase p_projeksjoner_overskrift = new Phrase("Projeksjoner", font3_bold);
                ct.AddElement(p_projeksjoner_overskrift);
                Phrase p_projeksjoner = new Phrase("”MANGLER”", font3);
                ct.AddElement(p_projeksjoner);

                Phrase p_tilgangsrestriksjoner_overskrift = new Phrase("Tilgangsrestriksjoner", font3_bold);
                ct.AddElement(p_tilgangsrestriksjoner_overskrift);
                Phrase p_tilgangsrestriksjoner = new Phrase("”Tilgangsrestriksjoner”", font3);
                ct.AddElement(p_tilgangsrestriksjoner);

                Phrase p_tjeneste_overskrift = new Phrase("Tjeneste", font3_bold);
                ct.AddElement(p_tjeneste_overskrift);

                Phrase p_tjeneste_info = new Phrase("Sett inn tjeneste informasjon, evt. forklaring til deltema innenfor tjenester", font3);
                ct.AddElement(p_tjeneste_info);

                Phrase p_tjeneste_link = new Phrase();
                Anchor anchor_tjeneste = new Anchor("Sett inn lenke til tjeneste", font_link);
                anchor_tjeneste.Reference = "http://www.geonorge.no";
                p_tjeneste_link.Add(anchor_tjeneste);
                ct.AddElement(p_tjeneste_link);

                Phrase p_tjeneste_kall = new Phrase("Beskrivelse av kall for datasettets tjenester – eks. GetMap, GetFeatureInfo", font3);
                ct.AddElement(p_tjeneste_kall);

                ct.AddElement(writeTblHeader("OBJEKTTYPELISTE"));

                Paragraph p_objekt_type = new Paragraph("", font3);
                List l_objekt_type = new List(List.UNORDERED);
                l_objekt_type.SetListSymbol("\u2022");
                ListItem liObjekt_type = new ListItem("<sett inn objekttype og forklaring>", font3);
                l_objekt_type.Add(liObjekt_type);
                p_objekt_type.Add(l_objekt_type);
                ct.AddElement(p_objekt_type);


                ct.AddElement(writeTblHeader("EGENSKAPSLISTE"));

                Paragraph p_egenskap = new Paragraph("", font3);
                List l_egenskap = new List(List.UNORDERED);
                l_egenskap.SetListSymbol("\u2022");
                ListItem liEgenskap = new ListItem("<sett inn egenskap og forklaring>", font3);
                l_egenskap.Add(liEgenskap);
                p_egenskap.Add(l_egenskap);
                ct.AddElement(p_egenskap);

                ct.AddElement(writeTblHeader("LENKER"));

                Paragraph p_lenker = new Paragraph("", font3);

                List lenker = new List(List.UNORDERED);
                lenker.SetListSymbol("\u2022");

                ListItem liMetadata = new ListItem();
                Anchor anchorMetadata = new Anchor("Link til metadata i Geonorge", font_link);
                anchorMetadata.Reference = "http://www.geonorge.no";
                liMetadata.Add(anchorMetadata);

                ListItem liProduktspesifikasjon = new ListItem();
                Anchor anchorProduktspesifikasjon = new Anchor("Link til produktspesifikasjon", font_link);
                anchorProduktspesifikasjon.Reference = "http://www.geonorge.no";
                liProduktspesifikasjon.Add(anchorProduktspesifikasjon);

                ListItem liTegnregler = new ListItem();
                Anchor anchorTegnregler = new Anchor("Link til tegnregler", font_link);
                anchorTegnregler.Reference = "http://www.geonorge.no";
                liTegnregler.Add(anchorTegnregler);

                ListItem liProduktside = new ListItem();
                Anchor anchorProduktside = new Anchor("Link til produktside", font_link);
                anchorProduktside.Reference = "http://www.geonorge.no";
                liProduktside.Add(anchorProduktside);

                lenker.Add(liMetadata);
                lenker.Add(liProduktspesifikasjon);
                lenker.Add(liTegnregler);
                lenker.Add(liProduktside);
                p_lenker.Add(lenker);
                ct.AddElement(p_lenker);



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
                        //ct.SetColumns(left, right);
                        ct.SetSimpleColumn(doc.Left, doc.Bottom, doc.Right - colwidth, doc.Top);
                        i++;
                    }
                    else
                    {
                        //write the second column
                        //ct.SetColumns(left2, right2);
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
            //PdfPCell cell = new PdfPCell();
            cell.BackgroundColor = new BaseColor(0, 150, 0);
            cell.Border = 0;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //Chunk tblHead = new Chunk(txt);
            //cell.AddElement(tblHead);
            table.AddCell(cell);
            return table;
        
        }
    }
}