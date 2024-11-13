using CRE.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using CRE.Data.Validations;
using Microsoft.Identity.Client; // Ensure you have this using directive for IFormFile

namespace CRE.ViewModels
{
    public class UploadFormsViewModel
    {
        public EthicsApplication? EthicsApplication { get; set; }
        public NonFundedResearchInfo? NonFundedResearchInfo { get; set; }
        public IEnumerable<EthicsApplicationForms>? EthicsApplicationForms { get; set; } // List for multiple forms
        public IEnumerable<EthicsApplicationLog>? EthicsApplicationLog { get; set; } // For new logs when uploading the forms
        public ReceiptInfo? ReceiptInfo { get; set; } // To display the PDF receipt
        public List<CoProponent>? CoProponent { get; set; } // To display other proponents 
        public InitialReview? InitialReview { get; set; } // Add InitialReview to track status
        public string? LatestComment { get; set; } // To hold the latest comment from logs
        public string? NewComment { get; set; } // For the new comment input
        public EthicsClearance? EthicsClearance { get; set; }
        public CompletionCertificate? CompletionCertificate { get; set; }
        public CompletionReport? CompletionReport { get; set; }
        

        // Flags to determine if the study involves human subjects and minors
        public bool InvolvesHumanSubjects { get; set; }
        public bool InvolvesMinors { get; set; }

        [Required(ErrorMessage = "Form Upload is Required.")]
        [Display(Name = "Form 9 Application for Ethics Review of New Protocol: ")]
        public IFormFile FORM9 { get; set; }
        public IFormFile? editFORM9 { get; set; }

        [Required(ErrorMessage = "Form Upload is Required.")]
        [Display(Name = "Form 10 Research Study Protocol: ")]
        public IFormFile FORM10 { get; set; }
        public IFormFile? editFORM10 { get; set; }


        [Display(Name = "Form 10 Research Study Protocol: ")]
        public IFormFile? FORM15 { get; set; }
        public IFormFile? editFORM15 { get; set; }

        [Required(ErrorMessage = "Form Upload is Required.")]
        [Display(Name = "Researcher/s Curriculum Vitae: ")]
        public IFormFile RCV { get; set; }
        public IFormFile? editRCV { get; set; }

        [Required(ErrorMessage = "Form Upload is Required.")]
        [Display(Name = "Certificate of Validity: ")]
        public IFormFile CV { get; set; }
        public IFormFile? editCV { get; set; }

        [Required(ErrorMessage = "Form Upload is Required.")]
        [Display(Name = "Co-Authorship Agreement: ")]
        public IFormFile CAA { get; set; }
        public IFormFile? editCAA { get; set; }

        [Required(ErrorMessage = "Form Upload is Required.")]
        [Display(Name = "Letter of Intent: ")]
        public IFormFile LI { get; set; }
        public IFormFile? editLI { get; set; }

        // Conditional required fields based on involvement with human subjects and minors
        [RequiredIfHumanSubjects(ErrorMessage = "Form Upload is Required if human subjects are involved.")]
        [Display(Name = "Form 11 Informed Consent: ")]
        public IFormFile FORM11 { get; set; }
        public IFormFile? editFORM11 { get; set; }

        [RequiredIfHumanSubjects(ErrorMessage = "Form Upload is Required if human subjects are involved.")]
        [Display(Name = "Form 12 Assent Form: ")]
        public IFormFile FORM12 { get; set; }
        public IFormFile? editFORM12 { get; set; }

        [RequiredIfMinors(ErrorMessage = "Form Upload is Required if minors are involved.")]
        [Display(Name = "Form 10.1 Non-Human Determination Form: ")]
        public IFormFile FORM10_1 { get; set; }
        public IFormFile? editFORM10_1 { get; set; }

    }
}
