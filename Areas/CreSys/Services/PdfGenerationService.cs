
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using MailKit;
using ResearchManagementSystem.Areas.CreSys.Interfaces;
using ResearchManagementSystem.Areas.CreSys.ViewModels;
using ResearchManagementSystem.Areas.CreSys.ViewModels.FormClasses;
using ResearchManagementSystem.Areas.CreSys.ViewModels.ListViewModels;
using System;
using System.Linq;

namespace ResearchManagementSystem.Areas.CreSys.Services
{
    public class PdfGenerationService : IPdfGenerationService
    {
        private readonly IWebHostEnvironment _environment;

        public PdfGenerationService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        public async Task<byte[]> GenerateInformedConsentPdf(InformedConsentFormViewModel model)
        {
            using (var ms = new MemoryStream())
            {
                // Initialize the PDF writer and document just once
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                try
                {
                    // Add content to the document
                    BuildInformedConsentPdfContent(document, model);
                }
                catch (Exception ex)
                {
                    // Handle any error during PDF generation
                    throw new InvalidOperationException("Error generating Informed Consent PDF", ex);
                }

                // Close the document and return the PDF as byte array
                document.Close();
                return ms.ToArray();
            }
        }

        private void BuildInformedConsentPdfContent(Document document, InformedConsentFormViewModel model)
        {
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Fonts");

            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "PUP Logo - clearance.png");
            var logo = new Image(ImageDataFactory.Create(logoPath)).SetAutoScale(true).SetWidth(100).SetHeight(100);

            var tableHeader = new Table(UnitValue.CreatePercentArray(new float[] { 1, 8 }))
               .SetWidth(UnitValue.CreatePercentValue(100));

            tableHeader.AddCell(new Cell().Add(logo).SetBorder(Border.NO_BORDER));

            string calistoFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "calisto-mt.ttf");
            PdfFont calistoFont = PdfFontFactory.CreateFont(calistoFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string californianFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "californian-fb.ttf");
            PdfFont californianFont = PdfFontFactory.CreateFont(californianFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string gothicBoldFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "century-gothic-bold.ttf");
            PdfFont gothicBoldFont = PdfFontFactory.CreateFont(gothicBoldFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string britannicFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "britannic-regular.otf");
            PdfFont britannicFont = PdfFontFactory.CreateFont(britannicFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string centuryGothicFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "century-gothic-regular.ttf");
            PdfFont centuryGothicFont = PdfFontFactory.CreateFont(centuryGothicFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string segoeUIFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "segoe-ui-regular.ttf");
            PdfFont segoeUIFont = PdfFontFactory.CreateFont(segoeUIFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string TimesNewRomanPSFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "times-new-roman-ps.otf");
            PdfFont TimesNewRomanPSFont = PdfFontFactory.CreateFont(TimesNewRomanPSFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);


            // Create the first line and set its style
            var line1 = new Text("Republic of the Philippines")
                .SetFontSize(7)
                .SetFont(calistoFont);

            // Create the second line as a Paragraph and set leading
            var line2 = new Paragraph()
                .Add(new Text("P").SetFontSize((float)9.95).SetFont(californianFont))
                .Add(new Text("OLYTECHNIC ").SetFontSize(8).SetFont(californianFont))
                .Add(new Text("U").SetFontSize(8).SetFont(californianFont))
                .Add(new Text("NIVERSITY").SetFontSize(8).SetFont(californianFont))
                .Add(new Text(" OF THE ").SetFontSize(8).SetFont(californianFont))
                .Add(new Text("P").SetFontSize((float)9.95).SetFont(californianFont))
                .Add(new Text("HILIPPINES").SetFontSize(8).SetFont(californianFont))
                .SetMargin(0);

            // Create the third line as a Paragraph and set leading
            var line3 = new Paragraph()
                .Add(new Text("O").SetFontSize(11).SetFont(californianFont))
                .Add(new Text("FFICE ").SetFontSize(9).SetFont(californianFont))
                .Add(new Text("of the").SetFontSize(9).SetFont(californianFont).SetItalic())
                .Add(new Text(" V").SetFontSize(11).SetFont(californianFont))
                .Add(new Text(" ICE ").SetFontSize(9).SetFont(californianFont))
                .Add(new Text("P").SetFontSize(11).SetFont(californianFont))
                .Add(new Text("RESIDENT ").SetFontSize(9).SetFont(californianFont))
                .Add(new Text("for ").SetFontSize(9).SetFont(californianFont).SetItalic())
                .Add(new Text("RESEARCH, EXTENSION ").SetFontSize(9).SetFont(californianFont))
                .Add(new Text("and").SetFontSize(9).SetFont(californianFont).SetItalic())
                .Add(new Text(" DEVELOPMENT").SetFontSize(9).SetFont(californianFont))
                .SetMargin(0);



            // Create the fifth line and set its style
            var line4 = new Text("UNIVERSITY RESEARCH ETHICS CENTER")
                .SetFontSize(9)
                .SetFont(britannicFont);

            // Add each line separately to the document (each line as a new Text element in the same Paragraph)
            var headerParagraph = new Paragraph()
                .Add(line1)
                .Add("\n")
                .Add(line2)
                .Add("\n")
                .Add(line3)
                .Add("\n")
                .Add(line4)
                .Add("\n")
                .SetTextAlignment(TextAlignment.LEFT)
                .SetFixedLeading(12);

            // Add the header text to the second cell
            tableHeader.AddCell(new Cell().Add(headerParagraph).SetBorder(Border.NO_BORDER));

            document.Add(tableHeader);

            var customBlue = new DeviceRgb(68, 114, 196);
            var solidLine = new SolidLine(1);
            solidLine.SetColor(customBlue);
            var blueLine = new LineSeparator(solidLine);
            blueLine.SetWidth(500);
            blueLine.SetMarginBottom(0);
            document.Add(blueLine);

            // Create a table with 4 columns for each row
            Table table = new Table(new float[] { 1, 3, 1, 3 }).SetWidth(UnitValue.CreatePercentValue(100));

            // First row (TITLE OF THE STUDY with different fonts)
            table.AddCell(new Cell(1, 4) // 1 row, 2 columns combined
                .Add(new Paragraph()
                    .Add(new Text("TITLE OF THE STUDY")
                        .SetFont(gothicBoldFont)
                        .SetFontSize(9))
                    .Add(new Text(" " + model.NonFundedResearchInfo.Title)
                        .SetFont(centuryGothicFont)
                        .SetFontSize(9)))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));


            // Second row (DISCIPLINE, TYPE OF REVIEW)
            table.AddCell(new Cell().Add(new Paragraph("DISCIPLINE")
                .SetFontSize(9)
                .SetFont(gothicBoldFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph(model.EthicsApplication.FieldOfStudy)
                .SetFontSize(9)
                .SetFont(centuryGothicFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph("TYPE OF REVIEW")
                .SetFontSize(9)
                .SetFont(gothicBoldFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph(model.InitialReview.ReviewType)
                .SetFontSize(9)
                .SetFont(centuryGothicFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            // Third row (PROPONENT, INSTITUTION)
            table.AddCell(new Cell().Add(new Paragraph("PROPONENT")
                .SetFontSize(9)
                .SetFont(gothicBoldFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            // Combine the proponent name and co-proponents in one cell
            string proponentNames = string.Join("\n", model.CoProponents.Select(p => p.CoProponentName));
            table.AddCell(new Cell().Add(new Paragraph(model.NonFundedResearchInfo.Name + "\n" + proponentNames)
                .SetFontSize(9)
                .SetFont(centuryGothicFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph("INSTITUTION")
                .SetFontSize(9)
                .SetFont(gothicBoldFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph(model.NonFundedResearchInfo.University)
                .SetFontSize(9)
                .SetFont(centuryGothicFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            // Fourth row (REVIEW, REVIEWER TYPE)
            table.AddCell(new Cell().Add(new Paragraph("REVIEW")
                .SetFontSize(9)
                .SetFont(gothicBoldFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph(model.EthicsEvaluation.Name)
                .SetFontSize(9)
                .SetFont(centuryGothicFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph("REVIEWER TYPE")
                .SetFontSize(9)
                .SetFont(gothicBoldFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph(model.EvaluatorType)
                .SetFontSize(9)
                .SetFont(centuryGothicFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            // Apply outer border to the entire table
            table.SetBorder(new SolidBorder(1)); // Set outer border thickness (1 pt)

            // Add the table to the document
            document.Add(table);

            Table redTable = new Table(1).SetWidth(UnitValue.CreatePercentValue(100)); // Make the table span the entire page width

            // Add a cell with red background and white text
            redTable.AddCell(new Cell()
                .Add(new Paragraph("INFORMED CONSENT FORM EVALUATION SHEET")
                    .SetFontSize(12)
                    .SetFont(gothicBoldFont)
                    .SetFontColor(ColorConstants.WHITE)) // Set the text color to white
                .SetBackgroundColor(new DeviceRgb(192, 0, 0))
                .SetTextAlignment(TextAlignment.CENTER) // Align text to center
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetMarginBottom(10)// Align text vertically
                .SetBorder(Border.NO_BORDER)); // Remove the border of the cell

            // Add the red table to the document
            document.Add(redTable);
            // Initialize the table with 3 columns
            Table questions = new Table(2);

            // Add questions and answers to the table
            foreach (var question in model.Questions ?? new List<Question>())
            {
                // Add the main question text to the first column of the first row
                questions.AddCell(new Cell().Add(new Paragraph(question.QuestionText ?? "Question text not provided").SetFontSize(11).SetFont(centuryGothicFont)));

                // Handle different answer types
                switch (question.AnswerType)
                {
                    case AnswerType.YesNo:
                    case AnswerType.YesNoFollowUp:
                        // Display Yes/No answer directly
                        string yesNoAnswer = question.Answer ?? "No answer provided";
                        questions.AddCell(new Cell().Add(new Paragraph(yesNoAnswer).SetFontSize(11).SetFont(centuryGothicFont)));

                        // Add follow-up question and answer in a new row if available
                        if (!string.IsNullOrEmpty(question.FollowUpText))
                        {
                            string followUpContent = $"{question.FollowUpText ?? string.Empty} {question.FollowUpAnswer ?? string.Empty}";
                            questions.AddCell(new Cell(1, 2)  // Spanning 2 columns
                                .Add(new Paragraph(followUpContent).SetFontSize(11).SetFont(centuryGothicFont)));

                        }

                        break;

                    case AnswerType.MultiDropDown:
                        // Display selected options
                        string selectedOptions = question.Answer ?? string.Join(", ", question.Options ?? new List<string>());
                        questions.AddCell(new Cell().Add(new Paragraph(selectedOptions).SetFont(centuryGothicFont).SetFontSize(11)));

                        // No follow-up for MultiDropDown, so leave the follow-up cells empty
                        questions.AddCell(new Cell().Add(new Paragraph("").SetFontSize(11)));
                        break;

                    case AnswerType.Text:
                        // Display text response
                        string textAnswer = question.FollowUpAnswer ?? string.Empty;
                        questions.AddCell(new Cell().Add(new Paragraph(textAnswer).SetFont(centuryGothicFont).SetFontSize(11)));

                        // No follow-up for Text, so leave the follow-up cells empty
                        questions.AddCell(new Cell().Add(new Paragraph("").SetFontSize(11)));
                        break;

                    default:
                        // Handle unexpected AnswerType gracefully
                        questions.AddCell(new Cell().Add(new Paragraph("Answer type not recognized.").SetFont(centuryGothicFont).SetFontSize(11).SetItalic()));
                        questions.AddCell(new Cell().Add(new Paragraph("").SetFont(centuryGothicFont).SetFontSize(11))); // Empty answer cell
                        questions.AddCell(new Cell().Add(new Paragraph("").SetFont(centuryGothicFont).SetFontSize(11))); // Empty follow-up cell
                        break;
                }
            }

            // Add the table to the document
            document.Add(questions);
            // Reviewer and Name with different fonts and underlined
            // Create a table with 2 columns and no borders
            Table reviewertable = new Table(2).SetWidth(UnitValue.CreatePercentValue(100));

            // Reviewer label and value in the same cell (no borders)
            Cell reviewerCell = new Cell().SetFontSize(11).SetBorder(Border.NO_BORDER);  // Remove border
            Paragraph reviewerParagraph = new Paragraph()
                .Add(new Text("Reviewer: ").SetFont(centuryGothicFont))  // "Reviewer" in Century Gothic
                .Add(new Text(model.EthicsEvaluation.Name).SetFont(gothicBoldFont).SetUnderline());  // Name in another font with underline
            reviewerCell.Add(reviewerParagraph);
            reviewertable.AddCell(reviewerCell);

            // Review Date label and value in the same cell (no borders)
            Cell reviewDateCell = new Cell().SetFontSize(11).SetBorder(Border.NO_BORDER);  // Remove border
            Paragraph reviewDateParagraph = new Paragraph()
                .Add(new Text("Review Date: ").SetFont(centuryGothicFont))  // "Review Date" in Century Gothic
                .Add(new Text(DateTime.Now.ToString("MMMM dd, yyyy")).SetFont(gothicBoldFont).SetUnderline());  // Date in another font with underline
            reviewDateCell.Add(reviewDateParagraph);
            reviewertable.AddCell(reviewDateCell);

            // Add the table to the document
            document.Add(reviewertable);



            // Define each line of text
            var Footerline1 = new Text("S423, 4th Floor South Wing, PUP A. Mabini Campus, Anonas Street, Sta. Mesa, Manila 1016")
                .SetFont(segoeUIFont)         // Set font for line 1
                .SetFontSize(8);               // Set font size for line 1

            var Footerline2 = new Text("Trunk Line: 335-1787 or 335-1777 local 235/357")
                .SetFont(segoeUIFont)         // Set font for line 2
                .SetFontSize(8);               // Set font size for line 2

            var Footerline3 = new Text("Website: www.pup.edu.ph | Email: vpre@pup.edu.ph")
                .SetFont(segoeUIFont)         // Set font for line 3
                .SetFontSize(8);               // Set font size for line 3


            var Footerline4 = new Paragraph()
              .Add(new Text("T").SetFontSize(14).SetFont(californianFont))
              .Add(new Text("HE ").SetFontSize(12).SetFont(californianFont))
              .Add(new Text("C").SetFontSize(14).SetFont(californianFont))
              .Add(new Text("OUNTRY’S ").SetFontSize(12).SetFont(californianFont))
              .Add(new Text("1").SetFontSize((float)9.95).SetFont(californianFont))  // Add "1"
              .Add(new Text("st").SetFontSize(8).SetFont(californianFont).SetTextRise(3))  // Superscript "st"
              .Add(new Text(" ").SetFontSize((float)9.95).SetFont(californianFont))  // Space after "1st"
              .Add(new Text("P").SetFontSize(14).SetFont(californianFont))
              .Add(new Text("OLYTECHNIC").SetFontSize((float)9.95).SetFont(californianFont))
              .Add(new Text("U ").SetFontSize((float)9.95).SetFont(californianFont))
              .SetMarginTop(5);

            // Create a Paragraph with the lines added individually
            Paragraph footerText = new Paragraph()
                .Add(Footerline1).Add("\n")   // Add line 1
                .Add(Footerline2).Add("\n")
                .Add(Footerline3).Add("\n")
                .Add(Footerline4)// Add line 3
                .SetTextAlignment(TextAlignment.LEFT)
                .SetMarginTop(80)
                .SetMarginLeft(40)            // Set left margin for positioning
                .SetFixedLeading(12)          // Adjust the leading to control line spacing
                .SetFixedPosition(50, 40, 400);  // Set the position for the paragraph

            // Add the footer text to the document
            document.Add(footerText);

            string footerPhotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "footer-image-clearance.png");
            var footerImage = new Image(ImageDataFactory.Create(footerPhotoPath))
                 .SetWidth(125)        // Set the width of the image
                 .SetHeight(100)
                 .SetFixedPosition(430, 20); // Try smaller X and Y values for testing

            document.Add(footerImage);
            document.Close();

        }

        public async Task<byte[]> GenerateProtocolReviewPdf(ProtocolReviewFormViewModel model)
        {
            using (var ms = new MemoryStream())
            {
                // Use iText's programmatic construction instead of HtmlConverter
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                try
                {
                    // Add content to the document
                    BuildProtocolReviewPdfContent(document, model);
                }
                catch (Exception ex)
                {
                    // Handle any error during PDF generation
                    throw new InvalidOperationException("Error generating Protocol Review PDF", ex);
                }

                document.Close();
                return ms.ToArray();
            }
        }

        private void BuildProtocolReviewPdfContent(Document document, ProtocolReviewFormViewModel model)
        {
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Fonts");

            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "PUP Logo - clearance.png");
            var logo = new Image(ImageDataFactory.Create(logoPath)).SetAutoScale(true).SetWidth(100).SetHeight(100);

            var tableHeader = new Table(UnitValue.CreatePercentArray(new float[] { 1, 8 }))
               .SetWidth(UnitValue.CreatePercentValue(100));

            tableHeader.AddCell(new Cell().Add(logo).SetBorder(Border.NO_BORDER));

            string calistoFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "calisto-mt.ttf");
            PdfFont calistoFont = PdfFontFactory.CreateFont(calistoFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string californianFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "californian-fb.ttf");
            PdfFont californianFont = PdfFontFactory.CreateFont(californianFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string gothicBoldFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "century-gothic-bold.ttf");
            PdfFont gothicBoldFont = PdfFontFactory.CreateFont(gothicBoldFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string britannicFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "britannic-regular.otf");
            PdfFont britannicFont = PdfFontFactory.CreateFont(britannicFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string centuryGothicFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "century-gothic-regular.ttf");
            PdfFont centuryGothicFont = PdfFontFactory.CreateFont(centuryGothicFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string segoeUIFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "segoe-ui-regular.ttf");
            PdfFont segoeUIFont = PdfFontFactory.CreateFont(segoeUIFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string TimesNewRomanPSFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "times-new-roman-ps.otf");
            PdfFont TimesNewRomanPSFont = PdfFontFactory.CreateFont(TimesNewRomanPSFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);


            // Create the first line and set its style
            var line1 = new Text("Republic of the Philippines")
                .SetFontSize(7)
                .SetFont(calistoFont);

            // Create the second line as a Paragraph and set leading
            var line2 = new Paragraph()
                .Add(new Text("P").SetFontSize((float)9.95).SetFont(californianFont))
                .Add(new Text("OLYTECHNIC ").SetFontSize(8).SetFont(californianFont))
                .Add(new Text("U").SetFontSize(8).SetFont(californianFont))
                .Add(new Text("NIVERSITY").SetFontSize(8).SetFont(californianFont))
                .Add(new Text(" OF THE ").SetFontSize(8).SetFont(californianFont))
                .Add(new Text("P").SetFontSize((float)9.95).SetFont(californianFont))
                .Add(new Text("HILIPPINES").SetFontSize(8).SetFont(californianFont))
                .SetMargin(0);

            // Create the third line as a Paragraph and set leading
            var line3 = new Paragraph()
                .Add(new Text("O").SetFontSize(11).SetFont(californianFont))
                .Add(new Text("FFICE ").SetFontSize(9).SetFont(californianFont))
                .Add(new Text("of the").SetFontSize(9).SetFont(californianFont).SetItalic())
                .Add(new Text(" V").SetFontSize(11).SetFont(californianFont))
                .Add(new Text(" ICE ").SetFontSize(9).SetFont(californianFont))
                .Add(new Text("P").SetFontSize(11).SetFont(californianFont))
                .Add(new Text("RESIDENT ").SetFontSize(9).SetFont(californianFont))
                .Add(new Text("for ").SetFontSize(9).SetFont(californianFont).SetItalic())
                .Add(new Text("RESEARCH, EXTENSION ").SetFontSize(9).SetFont(californianFont))
                .Add(new Text("and").SetFontSize(9).SetFont(californianFont).SetItalic())
                .Add(new Text(" DEVELOPMENT").SetFontSize(9).SetFont(californianFont))
                .SetMargin(0);



            // Create the fifth line and set its style
            var line4 = new Text("UNIVERSITY RESEARCH ETHICS CENTER")
                .SetFontSize(9)
                .SetFont(britannicFont);

            // Add each line separately to the document (each line as a new Text element in the same Paragraph)
            var headerParagraph = new Paragraph()
                .Add(line1)
                .Add("\n")
                .Add(line2)
                .Add("\n")
                .Add(line3)
                .Add("\n")
                .Add(line4)
                .Add("\n")
                .SetTextAlignment(TextAlignment.LEFT)
                .SetFixedLeading(12);

            // Add the header text to the second cell
            tableHeader.AddCell(new Cell().Add(headerParagraph).SetBorder(Border.NO_BORDER));

            document.Add(tableHeader);

            var customBlue = new DeviceRgb(68, 114, 196);
            var solidLine = new SolidLine(1);
            solidLine.SetColor(customBlue);
            var blueLine = new LineSeparator(solidLine);
            blueLine.SetWidth(500);
            blueLine.SetMarginBottom(0);
            document.Add(blueLine);

            // Create a table with 4 columns for each row
            Table table = new Table(new float[] { 1, 3, 1, 3 }).SetWidth(UnitValue.CreatePercentValue(100));

            // First row (TITLE OF THE STUDY with different fonts)
            table.AddCell(new Cell(1, 4) // 1 row, 2 columns combined
                .Add(new Paragraph()
                    .Add(new Text("TITLE OF THE STUDY")
                        .SetFont(gothicBoldFont)
                        .SetFontSize(9))
                    .Add(new Text(" " + model.NonFundedResearchInfo.Title)
                        .SetFont(centuryGothicFont)
                        .SetFontSize(9)))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));


            // Second row (DISCIPLINE, TYPE OF REVIEW)
            table.AddCell(new Cell().Add(new Paragraph("DISCIPLINE")
                .SetFontSize(9)
                .SetFont(gothicBoldFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph(model.EthicsApplication.FieldOfStudy)
                .SetFontSize(9)
                .SetFont(centuryGothicFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph("TYPE OF REVIEW")
                .SetFontSize(9)
                .SetFont(gothicBoldFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph(model.InitialReview.ReviewType)
                .SetFontSize(9)
                .SetFont(centuryGothicFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            // Third row (PROPONENT, INSTITUTION)
            table.AddCell(new Cell().Add(new Paragraph("PROPONENT")
                .SetFontSize(9)
                .SetFont(gothicBoldFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            // Combine the proponent name and co-proponents in one cell
            string proponentNames = string.Join("\n", model.CoProponents.Select(p => p.CoProponentName));
            table.AddCell(new Cell().Add(new Paragraph(model.NonFundedResearchInfo.Name + "\n" + proponentNames)
                .SetFontSize(9)
                .SetFont(centuryGothicFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph("INSTITUTION")
                .SetFontSize(9)
                .SetFont(gothicBoldFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph(model.NonFundedResearchInfo.University)
                .SetFontSize(9)
                .SetFont(centuryGothicFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            // Fourth row (REVIEW, REVIEWER TYPE)
            table.AddCell(new Cell().Add(new Paragraph("REVIEW")
                .SetFontSize(9)
                .SetFont(gothicBoldFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph(model.EthicsEvaluation.Name)
                .SetFontSize(9)
                .SetFont(centuryGothicFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph("REVIEWER TYPE")
                .SetFontSize(9)
                .SetFont(gothicBoldFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph(model.EvaluatorType)
                .SetFontSize(9)
                .SetFont(centuryGothicFont))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER));

            // Apply outer border to the entire table
            table.SetBorder(new SolidBorder(1)); // Set outer border thickness (1 pt)

            // Add the table to the document
            document.Add(table);


            Table redTable = new Table(1).SetWidth(UnitValue.CreatePercentValue(100)); // Make the table span the entire page width

            // Add a cell with red background and white text
            redTable.AddCell(new Cell()
                .Add(new Paragraph("PROTOCOL REVIEW SHEET")
                    .SetFontSize(9)
                    .SetFont(gothicBoldFont)
                    .SetFontColor(ColorConstants.WHITE)) // Set the text color to white
                .SetBackgroundColor(new DeviceRgb(192, 0, 0))
                .SetTextAlignment(TextAlignment.CENTER) // Align text to center
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetMarginBottom(10)// Align text vertically
                .SetBorder(Border.NO_BORDER)); // Remove the border of the cell

            // Add the red table to the document
            document.Add(redTable);
            Table questions = new Table(2);

            // Add questions and answers to the table
            foreach (var question in model.Questions ?? new List<Question>())
            {
                // Add the main question text to the first column of the first row
                questions.AddCell(new Cell().Add(new Paragraph(question.QuestionText ?? "Question text not provided").SetFontSize(11).SetFont(centuryGothicFont)));

                // Handle different answer types
                switch (question.AnswerType)
                {
                    case AnswerType.YesNo:
                    case AnswerType.YesNoFollowUp:
                        // Display Yes/No answer directly
                        string yesNoAnswer = question.Answer ?? "No answer provided";
                        questions.AddCell(new Cell().Add(new Paragraph(yesNoAnswer).SetFontSize(11).SetFont(centuryGothicFont)));

                        // Add follow-up question and answer in a new row if available
                        if (!string.IsNullOrEmpty(question.FollowUpText))
                        {
                            string followUpContent = $"{question.FollowUpText ?? string.Empty} {question.FollowUpAnswer ?? string.Empty}";
                            questions.AddCell(new Cell(1, 2)  // Spanning 2 columns
                                .Add(new Paragraph(followUpContent).SetFontSize(11).SetFont(centuryGothicFont)));

                        }

                        break;

                    case AnswerType.MultiDropDown:
                        // Display selected options
                        string selectedOptions = question.Answer ?? string.Join(", ", question.Options ?? new List<string>());
                        questions.AddCell(new Cell().Add(new Paragraph(selectedOptions).SetFont(centuryGothicFont).SetFontSize(11)));

                        // No follow-up for MultiDropDown, so leave the follow-up cells empty
                        questions.AddCell(new Cell().Add(new Paragraph("").SetFontSize(11)));
                        break;

                    case AnswerType.Text:
                        // Display text response
                        string textAnswer = question.FollowUpAnswer ?? string.Empty;
                        questions.AddCell(new Cell().Add(new Paragraph(textAnswer).SetFont(centuryGothicFont).SetFontSize(11)));

                        // No follow-up for Text, so leave the follow-up cells empty
                        questions.AddCell(new Cell().Add(new Paragraph("").SetFontSize(11)));
                        break;

                    default:
                        // Handle unexpected AnswerType gracefully
                        questions.AddCell(new Cell().Add(new Paragraph("Answer type not recognized.").SetFont(centuryGothicFont).SetFontSize(11).SetItalic()));
                        questions.AddCell(new Cell().Add(new Paragraph("").SetFont(centuryGothicFont).SetFontSize(11))); // Empty answer cell
                        questions.AddCell(new Cell().Add(new Paragraph("").SetFont(centuryGothicFont).SetFontSize(11))); // Empty follow-up cell
                        break;
                }
            }

            // Add the table to the document
            document.Add(questions);
            // Reviewer and Name with different fonts and underlined
            // Create a table with 2 columns and no borders
            Table reviewertable = new Table(2).SetWidth(UnitValue.CreatePercentValue(100));

            // Reviewer label and value in the same cell (no borders)
            Cell reviewerCell = new Cell().SetFontSize(11).SetBorder(Border.NO_BORDER);  // Remove border
            Paragraph reviewerParagraph = new Paragraph()
                .Add(new Text("Reviewer: ").SetFont(centuryGothicFont))  // "Reviewer" in Century Gothic
                .Add(new Text(model.EthicsEvaluation.Name).SetFont(gothicBoldFont).SetUnderline());  // Name in another font with underline
            reviewerCell.Add(reviewerParagraph);
            reviewertable.AddCell(reviewerCell);

            // Review Date label and value in the same cell (no borders)
            Cell reviewDateCell = new Cell().SetFontSize(11).SetBorder(Border.NO_BORDER);  // Remove border
            Paragraph reviewDateParagraph = new Paragraph()
                .Add(new Text("Review Date: ").SetFont(centuryGothicFont))  // "Review Date" in Century Gothic
                .Add(new Text(DateTime.Now.ToString("MMMM dd, yyyy")).SetFont(gothicBoldFont).SetUnderline());  // Date in another font with underline
            reviewDateCell.Add(reviewDateParagraph);
            reviewertable.AddCell(reviewDateCell);

            // Add the table to the document
            document.Add(reviewertable);



            // Define each line of text
            var Footerline1 = new Text("S423, 4th Floor South Wing, PUP A. Mabini Campus, Anonas Street, Sta. Mesa, Manila 1016")
                .SetFont(segoeUIFont)         // Set font for line 1
                .SetFontSize(8);               // Set font size for line 1

            var Footerline2 = new Text("Trunk Line: 335-1787 or 335-1777 local 235/357")
                .SetFont(segoeUIFont)         // Set font for line 2
                .SetFontSize(8);               // Set font size for line 2

            var Footerline3 = new Text("Website: www.pup.edu.ph | Email: vpre@pup.edu.ph")
                .SetFont(segoeUIFont)         // Set font for line 3
                .SetFontSize(8);               // Set font size for line 3


            var Footerline4 = new Paragraph()
              .Add(new Text("T").SetFontSize(14).SetFont(californianFont))
              .Add(new Text("HE ").SetFontSize(12).SetFont(californianFont))
              .Add(new Text("C").SetFontSize(14).SetFont(californianFont))
              .Add(new Text("OUNTRY’S ").SetFontSize(12).SetFont(californianFont))
              .Add(new Text("1").SetFontSize((float)9.95).SetFont(californianFont))  // Add "1"
              .Add(new Text("st").SetFontSize(8).SetFont(californianFont).SetTextRise(3))  // Superscript "st"
              .Add(new Text(" ").SetFontSize((float)9.95).SetFont(californianFont))  // Space after "1st"
              .Add(new Text("P").SetFontSize(14).SetFont(californianFont))
              .Add(new Text("OLYTECHNIC").SetFontSize((float)9.95).SetFont(californianFont))
              .Add(new Text("U ").SetFontSize((float)9.95).SetFont(californianFont))
              .SetMarginTop(5);

            // Create a Paragraph with the lines added individually
            Paragraph footerText = new Paragraph()
                .Add(Footerline1).Add("\n")   // Add line 1
                .Add(Footerline2).Add("\n")
                .Add(Footerline3).Add("\n")
                .Add(Footerline4)// Add line 3
                .SetTextAlignment(TextAlignment.LEFT)
                .SetMarginTop(80)
                .SetMarginLeft(40)            // Set left margin for positioning
                .SetFixedLeading(12)          // Adjust the leading to control line spacing
                .SetFixedPosition(50, 40, 400);  // Set the position for the paragraph

            // Add the footer text to the document
            document.Add(footerText);

            string footerPhotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "footer-image-clearance.png");
            var footerImage = new Image(ImageDataFactory.Create(footerPhotoPath))
                 .SetWidth(125)        // Set the width of the image
                 .SetHeight(100)
                 .SetFixedPosition(430, 20); // Try smaller X and Y values for testing

            document.Add(footerImage);
            document.Close();

        }

        public async Task<byte[]> GenerateClearancePdf(EthicsClearanceViewModel viewModel)
        {
            using (var ms = new MemoryStream())
            {
                // Use iText's programmatic construction instead of HtmlConverter
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf, iText.Kernel.Geom.PageSize.LETTER);


                try
                {

                    // Add content to the document
                    BuildEthicsClearancePdfContent(document, viewModel);
                }
                catch (Exception ex)
                {
                    // Handle any error during PDF generation
                    throw new InvalidOperationException("Error generating Ethics Clearance PDF", ex);
                }

                document.Close();
                return ms.ToArray();
            }
        }

        private void BuildEthicsClearancePdfContent(Document document, EthicsClearanceViewModel viewModel)
        {
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Fonts");

            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "PUP Logo - clearance.png");
            var logo = new Image(ImageDataFactory.Create(logoPath)).SetAutoScale(true).SetWidth(100).SetHeight(100);

            var tableHeader = new Table(UnitValue.CreatePercentArray(new float[] { 1, 8 }))
               .SetWidth(UnitValue.CreatePercentValue(100));

            tableHeader.AddCell(new Cell().Add(logo).SetBorder(Border.NO_BORDER));

            string calistoFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "calisto-mt.ttf");
            PdfFont calistoFont = PdfFontFactory.CreateFont(calistoFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string gothicBoldFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "century-gothic-bold.ttf");
            PdfFont gothicBoldFont = PdfFontFactory.CreateFont(gothicBoldFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string californianFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "californian-fb.ttf");
            PdfFont californianFont = PdfFontFactory.CreateFont(californianFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string britannicFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "britannic-regular.otf");
            PdfFont britannicFont = PdfFontFactory.CreateFont(britannicFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string centuryGothicFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "century-gothic-regular.ttf");
            PdfFont centuryGothicFont = PdfFontFactory.CreateFont(centuryGothicFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string segoeUIFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "segoe-ui-regular.ttf");
            PdfFont segoeUIFont = PdfFontFactory.CreateFont(segoeUIFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            string TimesNewRomanPSFontPath = Path.Combine(Directory.GetCurrentDirectory(), webRootPath, "times-new-roman-ps.otf");
            PdfFont TimesNewRomanPSFont = PdfFontFactory.CreateFont(TimesNewRomanPSFontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);


            // Create the first line and set its style
            var line1 = new Text("Republic of the Philippines")
                .SetFontSize(9)
                .SetFont(calistoFont);

            // Create the second line as a Paragraph and set leading
            var line2 = new Paragraph()
                .Add(new Text("P").SetFontSize(12).SetFont(californianFont))
                .Add(new Text("OLYTECHNIC ").SetFontSize((float)9.95).SetFont(californianFont))
                .Add(new Text("U").SetFontSize(12).SetFont(californianFont))
                .Add(new Text("NIVERSITY").SetFontSize((float)9.95).SetFont(californianFont))
                .Add(new Text(" OF THE ").SetFontSize((float)9.95).SetFont(californianFont))
                .Add(new Text("P").SetFontSize(12).SetFont(californianFont))
                .Add(new Text("HILIPPINES").SetFontSize((float)9.95).SetFont(californianFont))
                .SetMargin(0);

            // Create the third line as a Paragraph and set leading
            var line3 = new Paragraph()
                .Add(new Text("O").SetFontSize(12).SetFont(californianFont))
                .Add(new Text("FFICE ").SetFontSize((float)9.95).SetFont(californianFont))
                .Add(new Text("of the").SetFontSize((float)9.95).SetFont(californianFont).SetItalic())
                .Add(new Text(" V").SetFontSize(12).SetFont(californianFont))
                .Add(new Text(" ICE ").SetFontSize((float)9.95).SetFont(californianFont))
                .Add(new Text("P").SetFontSize(12).SetFont(californianFont))
                .Add(new Text("RESIDENT ").SetFontSize((float)9.95).SetFont(californianFont))
                .Add(new Text("for ").SetFontSize((float)9.95).SetFont(californianFont).SetItalic())
                .Add(new Text("RESEARCH, EXTENSION ").SetFontSize((float)9.95).SetFont(californianFont))
                .Add(new Text("and").SetFontSize((float)9.95).SetFont(californianFont).SetItalic())
                .Add(new Text(" DEVELOPMENT").SetFontSize((float)9.95).SetFont(californianFont))
                .SetMargin(0);

            // Create the fourth line and set its style
            var line4 = new Text("RESEARCH MANAGEMENT OFFICE")
                .SetFontSize(12)
                .SetFont(californianFont);

            // Create the fifth line and set its style
            var line5 = new Text("UNIVERSITY RESEARCH ETHICS CENTER")
                .SetFontSize(12)
                .SetFont(britannicFont);

            // Add each line separately to the document (each line as a new Text element in the same Paragraph)
            var headerParagraph = new Paragraph()
                .Add(line1)
                .Add("\n")
                .Add(line2)
                .Add("\n")
                .Add(line3)
                .Add("\n")
                .Add(line4)
                .Add("\n")
                .Add(line5)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetFixedLeading(12);

            // Add the header text to the second cell
            tableHeader.AddCell(new Cell().Add(headerParagraph).SetBorder(Border.NO_BORDER));

            document.Add(tableHeader);


            var customBlue = new DeviceRgb(68, 114, 196);
            var solidLine = new SolidLine(1);
            solidLine.SetColor(customBlue);
            var blueLine = new LineSeparator(solidLine);
            blueLine.SetWidth(500);
            blueLine.SetMarginBottom(20);
            document.Add(blueLine);


            // Create a table spanning the full page width with customized column widths
            Table subjectTable = new Table(new float[] { 1, 2 })
                .UseAllAvailableWidth()
                .SetMarginLeft(10)// Makes the table span the full width of the page
                .SetWidth(UnitValue.CreatePercentValue(100)); // Ensures the table fits within the page

            // Add Date Row
            subjectTable.AddCell(new Cell().Add(new Paragraph("Date:")
                .SetFontSize(9.95f).SetFont(centuryGothicFont))
                .SetBorder(Border.NO_BORDER).SetPaddingLeft(30)
                .SetTextAlignment(TextAlignment.LEFT));
            subjectTable.AddCell(new Cell().Add(new Paragraph($"{DateTime.Now:MMMM dd, yyyy}")
                .SetFontSize(9.95f).SetFont(centuryGothicFont))
                .SetBorder(Border.NO_BORDER));

            // To/For Row
            string recipients = $"{viewModel.NonFundedResearchInfo.Name.ToUpper()}\n" +
                                $"{string.Join("\n", viewModel.CoProponents.Select(p => p.CoProponentName.ToUpper()))}";
            subjectTable.AddCell(new Cell().Add(new Paragraph("To/For:")
                .SetFontSize(9.95f).SetFont(centuryGothicFont))
                .SetBorder(Border.NO_BORDER).SetPaddingLeft(30)
                .SetTextAlignment(TextAlignment.LEFT));
            subjectTable.AddCell(new Cell().Add(new Paragraph(recipients)
                .SetFontSize(9.95f).SetFont(gothicBoldFont))
                .SetBorder(Border.NO_BORDER));

            // Subject Row
            subjectTable.AddCell(new Cell().Add(new Paragraph("Subject:")
                .SetFontSize(9.95f).SetFont(centuryGothicFont))
                .SetBorder(Border.NO_BORDER).SetPaddingLeft(30)
                .SetTextAlignment(TextAlignment.LEFT));
            subjectTable.AddCell(new Cell().Add(new Paragraph("Ethical Clearance")
                .SetFontSize(9.95f).SetFont(centuryGothicFont))
                .SetBorder(Border.NO_BORDER));

            // From Row
            subjectTable.AddCell(new Cell().Add(new Paragraph("From:")
                .SetFontSize(9.95f).SetFont(centuryGothicFont))
                .SetBorder(Border.NO_BORDER).SetPaddingLeft(30).SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetTextAlignment(TextAlignment.LEFT));

            // Add the signature image and text
            using (var ms = new MemoryStream())
            {
                viewModel.UploadedSignature.CopyTo(ms);
                byte[] signatureImageBytes = ms.ToArray();
                var signatureImage = new Image(ImageDataFactory.Create(signatureImageBytes))
                    .SetWidth(80)
                    .SetHeight(40);

                var signatureParagraph = new Paragraph()
                    .SetMargin(0)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                    .Add(signatureImage)
                    .Add(new Text($"\n{viewModel.ChiefName}")
                        .SetFontSize(9.95f).SetFont(gothicBoldFont))
                    .Add(new Text("\nChief, Research Ethics Section")
                        .SetFontSize(9.95f).SetFont(centuryGothicFont).SetItalic());

                subjectTable.AddCell(new Cell().Add(signatureParagraph)
                    .SetBorder(Border.NO_BORDER));
            }

            // Add the table to the document
            document.Add(subjectTable);


            // Create a solid black line
            var balckSolidLine = new SolidLine(1);
            solidLine.SetColor(new DeviceRgb(0, 0, 0)); // Set color to black

            // Create a LineSeparator with the black solid line
            var blackLine = new LineSeparator(balckSolidLine);
            blackLine.SetWidth(500); // Set the width of the line
            blackLine.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            blackLine.SetMarginBottom((float)9.95); // Add margin below the line

            // Add the black line to the document
            document.Add(blackLine);

            string researchTitle = viewModel.NonFundedResearchInfo.Title.ToUpper(); // Convert title to uppercase

            // Create the body text with the title as bold and capitalized
            string bodyText = "This is to inform you that the documentary requirements you submitted for your research project titled ";


            Paragraph paragraph = new Paragraph()
                .SetFontSize((float)9.95)
                .SetTextAlignment(TextAlignment.JUSTIFIED) // Align text for better appearance
                .Add(bodyText) // Add the main body text
                .Add(new Text($"“{researchTitle}”").SetBold()) // Add the research title in bold
                .Add(" passed the evaluation of the PUP Research Ethics Committee (REC) in accordance with the requirements set by the Philippine Health Research Ethics Board (PHREB).\n")
                .SetFont(centuryGothicFont)
                .SetMarginLeft(20)
                .SetFixedLeading(12).SetMarginBottom(5)
                .SetMarginRight(20);
            // Add the paragraph to the document
            document.Add(paragraph);


            // Create a table with 2 columns
            Table table = new Table(2)
                .SetWidth(UnitValue.CreatePercentValue(100)) // Set table width to 80% of the page (adjust if needed)
                .SetHorizontalAlignment(HorizontalAlignment.LEFT)
                .SetMarginLeft(20)
                .SetMarginRight(20); // Center-align the table

            // Add rows for each label and value
            table.AddCell(new Cell().Add(new Paragraph("UREC Code:").SetFontSize((float)9.95).SetFont(centuryGothicFont)).SetTextAlignment(TextAlignment.LEFT));
            table.AddCell(new Cell().Add(new Paragraph(viewModel.EthicsApplication.UrecNo).SetFontSize((float)9.95).SetFont(centuryGothicFont).SetTextAlignment(TextAlignment.LEFT)));

            table.AddCell(new Cell().Add(new Paragraph("Type of Review:").SetFontSize((float)9.95).SetFont(centuryGothicFont)).SetTextAlignment(TextAlignment.LEFT));
            table.AddCell(new Cell().Add(new Paragraph(viewModel.InitialReview.ReviewType.ToUpper()).SetFontSize((float)9.95).SetFont(centuryGothicFont).SetTextAlignment(TextAlignment.LEFT)));

            table.AddCell(new Cell().Add(new Paragraph("Approval Date:").SetFont(centuryGothicFont)).SetFontSize((float)9.95).SetTextAlignment(TextAlignment.LEFT));
            table.AddCell(new Cell().Add(new Paragraph($"{DateTime.Now:MMMM dd, yyyy}").SetFontSize((float)9.95).SetFont(centuryGothicFont).SetTextAlignment(TextAlignment.LEFT)));

            table.AddCell(new Cell().Add(new Paragraph("Expiry Date:").SetFont(centuryGothicFont)).SetFontSize((float)9.95).SetTextAlignment(TextAlignment.LEFT));
            table.AddCell(new Cell().Add(new Paragraph($"{DateTime.Now.AddYears(1):MMMM dd, yyyy}").SetFontSize((float)9.95).SetFont(centuryGothicFont).SetTextAlignment(TextAlignment.LEFT)));

            table.AddCell(new Cell().Add(new Paragraph("PUP-UREC Decision:").SetFontSize((float)9.95).SetFont(gothicBoldFont).SetTextAlignment(TextAlignment.LEFT)));
            table.AddCell(new Cell().Add(new Paragraph("Approved").SetFontSize((float)9.95).SetFont(gothicBoldFont).SetTextAlignment(TextAlignment.LEFT)));

            // Add the table to the document
            document.Add(table);

            // Create the header as a paragraph
            Paragraph header = new Paragraph("The standard conditions of this approval are as follows:")
                .SetFontSize((float)9.95)
                .SetFont(centuryGothicFont)
                .SetMarginLeft(20)
                .SetMarginBottom(0)
                .SetFixedLeading(12);

            document.Add(header);

            // Create a single paragraph for the numbered list
            Paragraph numberedList = new Paragraph()
                .SetFontSize((float)9.95)
                .SetFont(centuryGothicFont)
                .SetMarginLeft(40) // Indent for the list
                .SetMarginRight(20)
                .SetFixedLeading(12)
                .SetMarginBottom(0)// Custom line spacing
                .Add("1.   Conduct the project following the submitted and approved research protocol and other documentary requirements.\n")
                .Add("2.   If changes are to be made in the research project/study that will affect the research participants, an amendment of the research protocol must be submitted to urec@pup.edu.ph before implementing such changes.\n")
                .Add("3.   When ethical clearance is about to expire, the researcher must apply to resubmit the research protocol.\n")
                .Add("4.   A final report/terminal report must be submitted when the research project/study is complete.\n")
                .Add("5.   Researchers must advise the PUP-UREC (email: urec@pup.edu.ph) in writing if the research project/study has been discontinued.");

            // Add the list paragraph to the document
            document.Add(numberedList);

            // Create the footer as a paragraph
            Paragraph footer = new Paragraph("You may now commence your research project/study. Good luck.")
                .SetFontSize((float)9.95)
                .SetFont(centuryGothicFont)
                .SetMarginLeft(20);

            document.Add(footer);


            // Define each line of text
            var Footerline1 = new Text("S423, 4th Floor South Wing, PUP A. Mabini Campus, Anonas Street, Sta. Mesa, Manila 1016")
                .SetFont(segoeUIFont)         // Set font for line 1
                .SetFontSize(8);               // Set font size for line 1

            var Footerline2 = new Text("Trunk Line: 335-1787 or 335-1777 local 235/357")
                .SetFont(segoeUIFont)         // Set font for line 2
                .SetFontSize(8);               // Set font size for line 2

            var Footerline3 = new Text("Website: www.pup.edu.ph | Email: vpre@pup.edu.ph")
                .SetFont(segoeUIFont)         // Set font for line 3
                .SetFontSize(8);               // Set font size for line 3


            var Footerline4 = new Paragraph()
              .Add(new Text("T").SetFontSize(14).SetFont(californianFont))
              .Add(new Text("HE ").SetFontSize(12).SetFont(californianFont))
              .Add(new Text("C").SetFontSize(14).SetFont(californianFont))
              .Add(new Text("OUNTRY’S ").SetFontSize(12).SetFont(californianFont))
              .Add(new Text("1").SetFontSize((float)9.95).SetFont(californianFont))  // Add "1"
              .Add(new Text("st").SetFontSize(8).SetFont(californianFont).SetTextRise(3))  // Superscript "st"
              .Add(new Text(" ").SetFontSize((float)9.95).SetFont(californianFont))  // Space after "1st"
              .Add(new Text("P").SetFontSize(14).SetFont(californianFont))
              .Add(new Text("OLYTECHNIC").SetFontSize((float)9.95).SetFont(californianFont))
              .Add(new Text("U ").SetFontSize((float)9.95).SetFont(californianFont))
              .SetMarginTop(5);

            // Create a Paragraph with the lines added individually
            Paragraph footerText = new Paragraph()
                .Add(Footerline1).Add("\n")   // Add line 1
                .Add(Footerline2).Add("\n")
                .Add(Footerline3).Add("\n")
                .Add(Footerline4)// Add line 3
                .SetTextAlignment(TextAlignment.LEFT)
                .SetMarginTop(60)
                .SetMarginLeft(40)            // Set left margin for positioning
                .SetFixedLeading(12)          // Adjust the leading to control line spacing
                .SetFixedPosition(50, 40, 400);  // Set the position for the paragraph

            // Add the footer text to the document
            document.Add(footerText);

            string footerPhotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "footer-image-clearance.png");
            var footerImage = new Image(ImageDataFactory.Create(footerPhotoPath))
                 .SetWidth(125)        // Set the width of the image
                 .SetHeight(100)
                 .SetFixedPosition(430, 40); // Try smaller X and Y values for testing

            document.Add(footerImage);
            document.Close();

        }

    }
}
