
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using ResearchManagementSystem.Areas.CreSys.Interfaces;
using ResearchManagementSystem.Areas.CreSys.ViewModels;
using ResearchManagementSystem.Areas.CreSys.ViewModels.FormClasses;
using ResearchManagementSystem.Areas.CreSys.ViewModels.ListViewModels;
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
            // Add title and header
            document.Add(new Paragraph("Republic of the Philippines")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12));
            document.Add(new Paragraph("POLYTECHNIC UNIVERSITY OF THE PHILIPPINES")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12)
                .SetBold());
            document.Add(new Paragraph("OFFICE of the VICE PRESIDENT for RESEARCH, EXTENSION and DEVELOPMENT")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12));
            document.Add(new Paragraph("UNIVERSITY RESEARCH ETHICS COMMITTEE")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12)
                .SetBold());
            document.Add(new Paragraph("INFORMED CONSENT FORM EVALUATION SHEET")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(14)
                .SetBold()
                .SetMarginBottom(20));

            // Add study details
            document.Add(new Paragraph($"Title of the Study: {model.NonFundedResearchInfo.Title}")
                .SetFontSize(12)
                .SetBold());
            document.Add(new Paragraph($"Discipline: {model.EthicsApplication.FieldOfStudy}")
                .SetFontSize(12));
            document.Add(new Paragraph($"Type of Review: {model.InitialReview.ReviewType}")
                .SetFontSize(12));

            document.Add(new Paragraph($"Proponent(s): {model.NonFundedResearchInfo.Name}, {string.Join(", ", model.CoProponents.Select(p => p.CoProponentName))} \n")
       .SetFontSize(12));


            document.Add(new Paragraph($"Institution: {model.NonFundedResearchInfo.University}")
                .SetFontSize(12));
            document.Add(new Paragraph($"Reviewer: {model.EthicsEvaluation.Name}")
                .SetFontSize(12));
            document.Add(new Paragraph($"Reviewer Type: {model.EvaluatorType}")
                .SetFontSize(12)
                .SetMarginBottom(20));

            // Add questions and answers
            foreach (var question in model.Questions ?? new List<Question>())
            {
                // Display the question text
                document.Add(new Paragraph(question.QuestionText ?? "Question text not provided")
                    .SetFontSize(12)
                    .SetBold());

                switch (question.AnswerType)
                {
                    case AnswerType.YesNo:
                    case AnswerType.YesNoFollowUp:
                        // Display Yes/No answer directly
                        document.Add(new Paragraph($"{question.Answer ?? "No answer provided"}")
                            .SetFontSize(12));

                        // Display follow-up text and answer if available
                        if (!string.IsNullOrEmpty(question.FollowUpText))
                        {
                            document.Add(new Paragraph($"{question.FollowUpText}")
                                .SetFontSize(12)
                                .SetItalic());
                            document.Add(new Paragraph($"{question.FollowUpAnswer ?? ""}")
                                .SetFontSize(12));
                        }
                        break;

                    case AnswerType.MultiDropDown:
                        // Display selected options
                        string selectedOptions = question.Answer ?? string.Join(", ", question.Options ?? new List<string>());
                        document.Add(new Paragraph($"{selectedOptions}")
                            .SetFontSize(12));
                        break;

                    case AnswerType.Text:
                        // Display text response
                        document.Add(new Paragraph($"{question.FollowUpAnswer ?? ""}")
                            .SetFontSize(12));
                        break;

                    default:
                        // Handle unexpected AnswerType gracefully
                        document.Add(new Paragraph("Answer type not recognized.")
                            .SetFontSize(12)
                            .SetItalic());
                        break;
                }

                // Add spacing between questions
                document.Add(new Paragraph("\n"));
            }

            

            // Signature Section
            document.Add(new Paragraph($"Reviewer: {model.EthicsEvaluation.Name}")
                .SetFontSize(12)
                .SetBold());
            document.Add(new Paragraph($"Review Date: {DateTime.Now:MMMM dd, yyyy}")
                .SetFontSize(12));
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
            // Add header information
            document.Add(new Paragraph("Republic of the Philippines")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12));
            document.Add(new Paragraph("POLYTECHNIC UNIVERSITY OF THE PHILIPPINES")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12)
                .SetBold());
            document.Add(new Paragraph("OFFICE of the VICE PRESIDENT for RESEARCH, EXTENSION and DEVELOPMENT")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12));
            document.Add(new Paragraph("UNIVERSITY RESEARCH ETHICS COMMITTEE")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12)
                .SetBold());
            document.Add(new Paragraph("PROTOCOL REVIEW FORM EVALUATION SHEET")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(14)
                .SetBold()
                .SetMarginBottom(20));

            // Add study details
            document.Add(new Paragraph($"Title of the Study: {model.NonFundedResearchInfo.Title}")
                .SetFontSize(12)
                .SetBold());
            document.Add(new Paragraph($"Discipline: {model.EthicsApplication.FieldOfStudy}")
                .SetFontSize(12));
            document.Add(new Paragraph($"Review Type: {model.InitialReview.ReviewType}")
                .SetFontSize(12));
            document.Add(new Paragraph($"Proponent(s): {model.NonFundedResearchInfo.Name}, {string.Join(", ", model.CoProponents.Select(p => p.CoProponentName))} \n")
      .SetFontSize(12));


            document.Add(new Paragraph($"Institution: {model.NonFundedResearchInfo.University}")
                .SetFontSize(12));
            document.Add(new Paragraph($"Reviewer: {model.EthicsEvaluation.Name}")
                .SetFontSize(12));
            document.Add(new Paragraph($"Reviewer Type: {model.EvaluatorType}")
                .SetFontSize(12)
                .SetMarginBottom(20));
            // Add questions and answers
            foreach (var question in model.Questions ?? new List<Question>())
            {
                // Display the question text
                document.Add(new Paragraph(question.QuestionText ?? "Question text not provided")
                    .SetFontSize(12)
                    .SetBold());

                switch (question.AnswerType)
                {
                    case AnswerType.MultipleChoiceWithText:
                    case AnswerType.MultipleChoiceWithTextNo:
                        // Display the main answer
                        document.Add(new Paragraph($"Answer: {question.Answer ?? "No answer selected"}")
                            .SetFontSize(12));

                        // Display follow-up question and answer if available
                        if (!string.IsNullOrEmpty(question.FollowUpText))
                        {
                            document.Add(new Paragraph($"{question.FollowUpText}")
                                .SetFontSize(12)
                                .SetItalic());
                            document.Add(new Paragraph($"{question.FollowUpAnswer ?? ""}")
                                .SetFontSize(12));
                        }
                        break;

                    case AnswerType.MultipleChoice:
                        // Display selected answer for multiple-choice
                        document.Add(new Paragraph($"{question.Answer ?? "No answer selected"}")
                            .SetFontSize(12));
                        break;

                    case AnswerType.MultiDropDown:
                        // Display selected options from the Answer field or SelectedOptions if populated
                        string selectedOptions = question.Answer ?? string.Join(", ", question.Options ?? new List<string>());
                        document.Add(new Paragraph($"{selectedOptions}")
                            .SetFontSize(12));
                        break;

                    case AnswerType.Text:
                        // Display text response
                        document.Add(new Paragraph($"{question.FollowUpAnswer ?? ""}")
                            .SetFontSize(12));
                        break;

                    case AnswerType.YesNo:
                    case AnswerType.YesNoFollowUp:
                        // Display Yes/No answer
                        document.Add(new Paragraph($"{question.Answer ?? "No answer selected"}")
                            .SetFontSize(12));

                        // Display follow-up question and answer if available
                        if (!string.IsNullOrEmpty(question.FollowUpText))
                        {
                            document.Add(new Paragraph($"{question.FollowUpText}")
                                .SetFontSize(12)
                                .SetItalic());
                            document.Add(new Paragraph($"{question.FollowUpAnswer ?? ""}")
                                .SetFontSize(12));
                        }
                        break;

                    default:
                        // Handle unexpected AnswerType gracefully
                        document.Add(new Paragraph("Answer type not recognized.")
                            .SetFontSize(12)
                            .SetItalic());
                        break;
                }

                // Add spacing between questions
                document.Add(new Paragraph("\n"));
            }

            

            // Add signature section
            document.Add(new Paragraph($"Reviewer: {model.EthicsEvaluation.Name}")
                .SetFontSize(12)
                .SetBold());
            document.Add(new Paragraph($"Review Date: {DateTime.Now:MMMM dd, yyyy}")
                .SetFontSize(12));
        }

        public async Task<byte[]> GenerateClearancePdf(EthicsClearanceViewModel viewModel)
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
            // Add header information
            document.Add(new Paragraph("Republic of the Philippines")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12));
            document.Add(new Paragraph("POLYTECHNIC UNIVERSITY OF THE PHILIPPINES")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12)
                .SetBold());
            document.Add(new Paragraph("OFFICE of the VICE PRESIDENT for RESEARCH, EXTENSION and DEVELOPMENT")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12));
            document.Add(new Paragraph("RESEARCH MANAGEMENT OFFICE")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12));
            document.Add(new Paragraph("UNIVERSITY RESEARCH ETHICS CENTER")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12)
                .SetBold()
                .SetMarginBottom(20));

            document.Add(new Paragraph($"Date: {DateTime.Now:MMMM dd, yyyy}")
               .SetFontSize(12));

            string recipients = $"{viewModel.NonFundedResearchInfo.Name}, {string.Join(", ", viewModel.CoProponents.Select(p => p.CoProponentName))}";
            document.Add(new Paragraph($"To/For: {recipients}")
                .SetFontSize(12)); // Add spacing below the block


            string researchTitle = viewModel.NonFundedResearchInfo.Title.ToUpper(); // Convert title to uppercase

            // Create the body text with the title as bold and capitalized
            string bodyText = "This is to inform you that the documentary requirements you submitted for your research project titled ";

            Paragraph paragraph = new Paragraph()
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.JUSTIFIED) // Align text for better appearance
                .Add(bodyText) // Add the main body text
                .Add(new Text($"“{researchTitle}”").SetBold()) // Add the research title in bold
                .Add(" passed the evaluation of the PUP Research Ethics Committee (REC) in accordance with the requirements set by the Philippine Health Research Ethics Board (PHREB)."); // Continue the rest of the text

            // Add the paragraph to the document
            document.Add(paragraph);

            document.Add(new Paragraph($"Subject: Ethical Clearance")
               .SetFontSize(12));
            document.Add(new Paragraph($"From: {viewModel.ChiefName}\nChief, Research Ethics Section")
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.LEFT));


                                                             // Create a table with 2 columns
            Table table = new Table(2)
                .SetWidth(UnitValue.CreatePercentValue(50)) // Set table width to 50% of the page
                .SetHorizontalAlignment(HorizontalAlignment.CENTER); // Center-align the table

            // Add rows for each label and value
            table.AddCell(new Cell().Add(new Paragraph("UREC Code:").SetBold()));
            table.AddCell(new Cell().Add(new Paragraph(viewModel.EthicsApplication.UrecNo)));

            table.AddCell(new Cell().Add(new Paragraph("Type of Review:").SetBold()));
            table.AddCell(new Cell().Add(new Paragraph(viewModel.InitialReview.ReviewType.ToUpper())));

            table.AddCell(new Cell().Add(new Paragraph("Approval Date:").SetBold()));
            table.AddCell(new Cell().Add(new Paragraph($"{DateTime.Now:MMMM dd, yyyy}")));

            table.AddCell(new Cell().Add(new Paragraph("Expiry Date:").SetBold()));
            table.AddCell(new Cell().Add(new Paragraph($"{DateTime.Now.AddYears(1):MMMM dd, yyyy}")));

            table.AddCell(new Cell().Add(new Paragraph("PUP-UREC Decision:").SetBold()));
            table.AddCell(new Cell().Add(new Paragraph("Approved")));

            // Add the table to the document
            document.Add(table);


            document.Add(new Paragraph("The standard conditions of this approval are as follows:")
                .SetFontSize(12)
                .SetBold());
            document.Add(new Paragraph("1. Conduct the project following the submitted and approved research protocol and other \n " +
                "documentary requirements.")
                .SetFontSize(12));
            document.Add(new Paragraph("2. If changes are to be made in the research project/study that will affect the research \n " +
                "participants, an amendment of the research protocol must be submitted to \n " +
                "urec@pup.edu.ph before implementing such changes.")
                .SetFontSize(12));
            document.Add(new Paragraph("3. When ethical clearance is about to expire, the researcher must apply to resubmit \n the research protocol.")
                .SetFontSize(12));
            document.Add(new Paragraph("4. A final report/terminal report must be submitted when the research project/study is \n complete.")
                .SetFontSize(12));
            document.Add(new Paragraph("5. Researchers must advise the PUP-UREC (email: urec@pup.edu.ph) in writing if the \n " +
                "research project/study has been discontinued.\n")
                .SetFontSize(12));
            document.Add(new Paragraph("You may now commence your research project/study. Good luck.")
                        .SetFontSize(12));
            // Close the document
            document.Close();
        }

    }
}
