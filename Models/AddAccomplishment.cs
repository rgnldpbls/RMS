using DocumentFormat.OpenXml.Presentation;
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


        [Display(Name = "Year")]
        public int? Year { get; set; }

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

        public string? Status {  get; set; }

        // College and Department
        public string? College { get; set; }
        public string? Department { get; set; }
        public string? BranchCampus { get; set; }

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
        public byte[]? RequiredFile3Data { get; set; }
        public byte[]? ConditionalFileData { get; set; }

        // Original filenames for reference
        public string? RequiredFile1Name { get; set; }
        public string? RequiredFile2Name { get; set; }
        public string? RequiredFile3Name { get; set; }
        public string? ConditionalFileName { get; set; }

        // Other properties
        public bool IsStudentInvolved { get; set; }
        public string? Notes { get; set; }


        [ForeignKey(nameof(CreatedBy))]
        [Display(Name = "CreatedBy")]
        public string? CreatedById { get; set; }

        public ApplicationUser? CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;



        //    //Collections for related accomplishments
        public virtual ICollection<AddPresentation>? AddPresentations { get; set; }

        public virtual ICollection<AddPublication>? AddPublications { get; set; }


        public virtual ICollection<AddUtilization>? AddUtilizations { get; set; }

        public virtual ICollection<AddPatent>? AddPatents { get; set; }

        public virtual ICollection<AddCitation>? AddCitations { get; set; }



        //     public virtual ICollection<AddUtilization> Utilization { get; set; } = new List<AddUtilization>();


        //     public virtual ICollection<AddPatent> Patent { get; set; } = new List<AddPatent>();



        //public virtual ICollection<AddCitation> Citations { get; set; } = new List<AddCitation>();
        public class FacultyAccomplishmentViewModel
        {
            public string ProductionId { get; set; }

            [Display(Name = "Research Title")]
            public string? ResearchTitle { get; set; }

            [Display(Name = "Year")]
            public int? Year { get; set; }

            // Lead Researcher
            [Display(Name = "Lead Researcher")]
            public string? LeadResearcherId { get; set; }
            public string? LeadResearcherFirstName { get; set; }
            public string? LeadResearcherLastName { get; set; }
            public string LeadResearcherFullName => $"{LeadResearcherFirstName} {LeadResearcherLastName}";

            // Co-Lead Researcher
            [Display(Name = "Co-Lead Researcher")]
            public string? CoLeadResearcherId { get; set; }
            public string? CoLeadResearcherFirstName { get; set; }
            public string? CoLeadResearcherLastName { get; set; }
            public string CoLeadResearcherFullName => $"{CoLeadResearcherFirstName} {CoLeadResearcherLastName}";

            // Member1 Researcher
            [Display(Name = "Member")]
            public string? MemberoneId { get; set; }
            public string? MemberOneFirstName { get; set; }
            public string? MemberOneLastName { get; set; }
            public string MemberOneFullName => $"{MemberOneFirstName} {MemberOneLastName}";

            // Member2 Researcher
            [Display(Name = "Member")]
            public string? MembertwoId { get; set; }
            public string? MemberTwoFirstName { get; set; }
            public string? MemberTwoLastName { get; set; }
            public string MemberTwoFullName => $"{MemberTwoFirstName} {MemberTwoLastName}";

            // Member3 Researcher
            [Display(Name = "Member")]
            public string? MemberthreeId { get; set; }
            public string? MemberThreeFirstName { get; set; }
            public string? MemberThreeLastName { get; set; }
            public string MemberThreeFullName => $"{MemberThreeFirstName} {MemberThreeLastName}";

            public string? College { get; set; }
            public string? Department { get; set; }
            public string? BranchCampus { get; set; }

            [Display(Name = "Date Started")]
            public DateTime DateStarted { get; set; }


            [Display(Name = "Date of Completion")]
            public DateTime? DateCompleted { get; set; }

            // Add other relevant properties
            [Display(Name = "CreatedBy")]
            public string? CreatedById { get; set; }
            public string? CreatedByFirstName { get; set; }
            public string? CreatedByLastName { get; set; }
            public string? CreatedByFullName => $"{CreatedByFirstName} {CreatedByLastName}";


            public DateTime CreatedOn { get; set; }

        }


        public class RMCCAccomplishmentViewModel
        {
            public string ProductionId { get; set; }


            [Display(Name = "Research Title")]
            public string? ResearchTitle { get; set; }


            // Add other relevant properties
            [Display(Name = "CreatedBy")]
            public string? CreatedById { get; set; }
            public string? CreatedByFirstName { get; set; }
            public string? CreatedByLastName { get; set; }
            public string? CreatedByFullName => $"{CreatedByFirstName} {CreatedByLastName}";



            [Display(Name = "Year")]
            public int? Year { get; set; }

            // Lead Researcher
            [Display(Name = "Lead Researcher")]
            public string? LeadResearcherId { get; set; }
            public string? LeadResearcherFirstName { get; set; }
            public string? LeadResearcherLastName { get; set; }
            public string LeadResearcherFullName => $"{LeadResearcherFirstName} {LeadResearcherLastName}";

            // Co-Lead Researcher
            [Display(Name = "Co-Lead Researcher")]
            public string? CoLeadResearcherId { get; set; }
            public string? CoLeadResearcherFirstName { get; set; }
            public string? CoLeadResearcherLastName { get; set; }
            public string CoLeadResearcherFullName => $"{CoLeadResearcherFirstName} {CoLeadResearcherLastName}";

            // Member1 Researcher
            [Display(Name = "Member")]
            public string? MemberoneId { get; set; }
            public string? MemberOneFirstName { get; set; }
            public string? MemberOneLastName { get; set; }
            public string MemberOneFullName => $"{MemberOneFirstName} {MemberOneLastName}";

            // Member2 Researcher
            [Display(Name = "Member")]
            public string? MembertwoId { get; set; }
            public string? MemberTwoFirstName { get; set; }
            public string? MemberTwoLastName { get; set; }
            public string MemberTwoFullName => $"{MemberTwoFirstName} {MemberTwoLastName}";

            // Member3 Researcher
            [Display(Name = "Member")]
            public string? MemberthreeId { get; set; }
            public string? MemberThreeFirstName { get; set; }
            public string? MemberThreeLastName { get; set; }
            public string MemberThreeFullName => $"{MemberThreeFirstName} {MemberThreeLastName}";

            public string? College { get; set; }
            public string? Department { get; set; }
            public DateTime CreatedOn { get; set; }
            public string? BranchCampus { get; set; }

            [Display(Name = "Date Started")]
            public DateTime DateStarted { get; set; }


            [Display(Name = "Date of Completion")]
            public DateTime? DateCompleted { get; set; }
        }




        //Filter 
        public class AddAccomplishmentFilter
        {
            public string? ResearchTitle { get; set; }
            public int? Year { get; set; }
            public string? College { get; set; }
            public string? Department { get; set; }
            public string? BranchCampus { get; set; }
            public string? FundingAgency { get; set; }
            public double? FundingAmount { get; set; }
            public DateTime? DateStartedFrom { get; set; }
            public DateTime? DateStartedTo { get; set; }
            public DateTime? DateCompletedFrom { get; set; }
            public DateTime? DateCompletedTo { get; set; }
            public bool? IsStudentInvolved { get; set; }
        }


        public class FacultyDetailsViewModel
        {
            public AddAccomplishment Production { get; set; }
            public List<AddPresentation> Presentations { get; set; }
            public List<AddPublication> Publications { get; set; }
            public List<AddPatent> Patents { get; set; }
            public List<AddUtilization> Utilizations { get; set; }
            public List<AddCitation> Citations { get; set; }
            // Additional properties for other accomplishments
        }

        public class AccomplishmentViewModel
        {
            public string ResearchTitle { get; set; }
            public string LeadResearcherName { get; set; }
            public string LeadResearcherEmail { get; set; }
            public string College { get; set; }
            public string BranchCampus { get; set; }
            public DateTime DateStarted { get; set; }
            public DateTime? DateCompleted { get; set; }
        }


    }
}


