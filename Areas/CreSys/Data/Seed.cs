using CRE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using ResearchManagementSystem.Data;

namespace CRE.Data
{
    public class Seed
    {
        public byte[] ReadFileToByteArray(string filePath)
        {
            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch (IOException ex)
            {
                // Log the exception or handle it as necessary
                Console.WriteLine($"Error reading file {filePath}: {ex.Message}");
                return null; // Or handle the error according to your application's needs
            }
        }
        public static async Task SeedDataAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var seed = new Seed(); //intance to use readfile method
                var context = serviceScope.ServiceProvider.GetService<CreDbContext>();
                // Seed Expertise
                if (!context.Expertise.Any())
                {
                    context.Expertise.AddRange(
                        new Expertise { expertiseName = "Education" },
                        new Expertise { expertiseName = "Computer Science, Information Systems, and Technology" },
                        new Expertise { expertiseName = "Engineering, Architecture, and Design" },
                        new Expertise { expertiseName = "Humanities, Language, and Communication" },
                        new Expertise { expertiseName = "Business" },
                        new Expertise { expertiseName = "Social Sciences" },
                        new Expertise { expertiseName = "Science, Mathematics, and Statistics" }
                    );
                    context.SaveChanges();
                }

                // Seed EvaluationForms
                /*if (!context.EvaluationForms.Any())
                {
                    context.EvaluationForms.AddRange(
                        new EvaluationForms
                        {
                            evalFormName = "Informed Consent Form",
                            evalFormFile = seed.ReadFileToByteArray("EvaluationTemplates\\Informed-Consent-Form-Evaluation-Sheet-TEMPLATE.docx")
                        },
                        new EvaluationForms
                        {
                            evalFormName = "Protocol Review Sheet",
                            evalFormFile = seed.ReadFileToByteArray("EvaluationTemplates\\ProtocolReviewSheet-TEMPLATE.docx")
                        }
                    );
                    await context.SaveChangesAsync(); // Save changes to the context
                }*/


                // Seed EthicsForms
                if (!context.EthicsForm.Any())
                {
                    context.EthicsForm.AddRange(
                        new EthicsForm
                        {
                            ethicsFormId = "FORM9",
                            formName = "Application for Ethics Review of New Protocol",
                            formDescription = "This form is required to be accomplished by all of the applicants.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-9-Application for Ethics Review of New Protocol.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "FORM10",
                            formName = "Research Study Protocol",
                            formDescription = "This form is required to be accomplished by all of the applicants.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-10-Research-Study Protocol.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "FORM10_1",
                            formName = "Research Study Protocol",
                            formDescription = "This form is for non-human research/es.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-10.1-NON-HUMAN-DETERMINANT-TEMPLATE.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "FORM11",
                            formName = "Informed Consent Form",
                            formDescription = "This form should be used for studies that requires human respondents.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-10.1-NON-HUMAN-DETERMINANT-TEMPLATE.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "FORM12",
                            formName = "Assent Form",
                            formDescription = "This form should be used for studies with minor respondents.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-12-Assent Form for Minors-Participants.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "FORM15",
                            formName = "Application for Ethics Review of Amendments",
                            formDescription = "This form should be used for detail changes and addition with regards to the application.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-15-Application for Ethics Review of Amendments.docx")
                        },
                        //Form 16 is disabled due to the interview 
                        //new EthicsForm
                        //{
                        //    ethicsFormId = "FORM-16",
                        //    formName = "Application for Cancellation of Ethics Review",
                        //    formDescription = "This form should be used in the event of cancellation of ethics clearance application.",
                        //    file = seed.ReadFileToByteArray("FormFiles\\FORM-16-Application for Cancellation of Ethics Review.docx")
                        //},
                        new EthicsForm
                        {
                            ethicsFormId = "FORM18",
                            formName = "Terminal Report Template",
                            formDescription = "This form should be submitted upon completion of the study.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-18-Terminal-Report-Template.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "CAA",
                            formName = "Co-Authorship Agreement",
                            formDescription = "This document is required for those studies with co-author/s.",
                            file = seed.ReadFileToByteArray("FormFiles\\CO-AUTHORSHIP-AGREEMENT.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "RCV",
                            formName = "Researcher Curriculum Vitae",
                            formDescription = "This document is required to be submitted by all of the applicants.",
                            file = seed.ReadFileToByteArray("FormFiles\\RESEARCHER CURRICULUM VITAE.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "CV",
                            formName = "Certificate of Validity",
                            formDescription = "This document should be submitted the study instrument is researcher-made or adapted but modified.",
                            file = seed.ReadFileToByteArray("FormFiles\\CERTIFICATE OF VALIDITY.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "LI",
                            formName = "Letter of Intent",
                            formDescription = "This document is to be submitted to the Office of the VPRED",
                            file = seed.ReadFileToByteArray("FormFiles\\LETTER OF INTENT.docx")
                        }
                    );
                    context.SaveChanges();
                }

            }
        }
    }
}
