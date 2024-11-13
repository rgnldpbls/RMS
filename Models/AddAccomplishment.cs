using ResearchManagementSystem.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Models
{
    public class AddAccomplishment
    {
        [Key]
        public string ProductionId { get; set; } = Guid.NewGuid().ToString();


        [Display(Name = "Research Title")]
        public string? ResearchTitle { get; set; }

        // Lead Researcher
        [ForeignKey(nameof(LeadResearcher))] // Refers to ApplicationUser by ID
        [Display(Name = "Lead Researcher")]
        public string? LeadResearcherId { get; set; }

        // Navigation property for Lead Researcher
        public ApplicationUser? LeadResearcher { get; set; }

        // Co-Lead Researcher
        [ForeignKey(nameof(CoLeadResearcher))]
        [Display(Name = "Co-Lead Researcher")]
        public string? CoLeadResearcherId { get; set; }

        // Navigation property for Co-Lead Researcher
        public ApplicationUser? CoLeadResearcher { get; set; }

        // Member1 Researcher
        [ForeignKey(nameof(Memberone))]
        [Display(Name = "Member")]
        public string? MemberoneId { get; set; }

        // Navigation property for Member1
        public ApplicationUser? Memberone { get; set; }

        // Member2 Researcher
        [ForeignKey(nameof(Membertwo))]
        [Display(Name = "Member")]
        public string? MembertwoId { get; set; }

        // Navigation property for Member2
        public ApplicationUser? Membertwo { get; set; }

        // Member3 Researcher
        [ForeignKey(nameof(Memberthree))]
        [Display(Name = "Member")]
        public string? MemberthreeId { get; set; }

        // Navigation property for Member3
        public ApplicationUser? Memberthree { get; set; }

        // College and Department
        public string? College { get; set; }
        public string? Department { get; set; }

        // Funding details
        [Display(Name = "Funding Agency")]
        public string? FundingAgency { get; set; }

        [Display(Name = "Amount of Funding")]
        public double? FundingAmount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date Started")]
        public DateTime DateStarted { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Completion")]
        public DateTime? DateCompleted { get; set; }

        // File properties to store binary data
        public byte[]? RequiredFile1Data { get; set; }
        public byte[]? RequiredFile2Data { get; set; }
        public byte[]? ConditionalFileData { get; set; }

        // Original filenames for reference
        public string? RequiredFile1Name { get; set; }
        public string? RequiredFile2Name { get; set; }
        public string? ConditionalFileName { get; set; }

        // Other properties
        public bool IsStudentInvolved { get; set; }
        public string? Notes { get; set; }


        [ForeignKey(nameof(CreatedBy))]
        [Display(Name = "CreatedBy")]
        public string? CreatedById { get; set; }

        public ApplicationUser? CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }



        //Collections for related accomplishments
         public virtual ICollection<AddPresentation> Presentation { get; set; } = new List<AddPresentation>();

        public virtual ICollection<AddPublication> Publication { get; set; } = new List<AddPublication>();

       
         public virtual ICollection<AddUtilization> Utilization { get; set; } = new List<AddUtilization>();

         
         public virtual ICollection<AddPatent> Patent { get; set; } = new List<AddPatent>();

         

        public virtual ICollection<AddCitation> Citations { get; set; } = new List<AddCitation>();
    }
}
