using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;



namespace ResearchManagementSystem.Models
{
    public class AddCitation
    {
        [Key]
        public string CitationId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(AddAccomplishment))]
        [Display(Name = "ProductionNo")]
        public string? ProductionId { get; set; }

        public AddAccomplishment? AddAccomplishment { get; set; }


        [ForeignKey(nameof(ArticleTitle))]
        [Display(Name = "Article/Title")]
        public string? ArticleTitle { get; set; }

        public AddPublication? Publication { get; set; }

        // Lead Researcher
        [ForeignKey(nameof(LeadResearcher))] // Refers to ApplicationUser by ID
        [Display(Name = "Lead Researcher")]
        public string? LeadResearcherId { get; set; }

        // Navigation property for Lead Researcher
        public AddAccomplishment? LeadResearcher { get; set; }

        // Co-Lead Researcher
        [ForeignKey(nameof(CoLeadResearcher))]
        [Display(Name = "Co-Lead Researcher")]
        public string? CoLeadResearcherId { get; set; }

        // Navigation property for Co-Lead Researcher
        public AddAccomplishment? CoLeadResearcher { get; set; }

        // Member1 Researcher
        [ForeignKey(nameof(Memberone))]
        [Display(Name = "Member")]
        public string? MemberoneId { get; set; }

        // Navigation property for Member1
        public AddAccomplishment? Memberone { get; set; }

        // Member2 Researcher
        [ForeignKey(nameof(Membertwo))]
        [Display(Name = "Member")]
        public string? MembertwoId { get; set; }

        // Navigation property for Member2
        public AddAccomplishment? Membertwo { get; set; }

        // Member3 Researcher
        [ForeignKey(nameof(Memberthree))]
        [Display(Name = "Member")]
        public string? MemberthreeId { get; set; }

        // Navigation property for Member2
        public AddAccomplishment? Memberthree { get; set; }

        [Display(Name = "Title of Refereed Journal where the Original Research Article was Published")]
        public string? OriginalArticlePublished { get; set; }

        [Display(Name = "Title of New Publication where the Original Research article was cited")]
        public string? NewPublicationTitle { get; set; }

        [Display(Name = "Authors of the New Article who cited the Original Research Article")]
        public string? AuthorsofNewArticle { get; set; }


        [Display(Name = "Title of Refereed Journal where the New Research Article was published")]
        public string? NewArticlePublished { get; set; }

        [Display(Name = "Volume No./ Issue No.")]
        public string? VolNoIssueNo { get; set; }

        [Display(Name = "No.of Pages")]
        public int? Pages { get; set; }

        [Display(Name = "Year of Publication")]
        public int? YearofPublication { get; set; }

        [Display(Name = "Indexing")]
        public string? Indexing { get; set; }


        public byte[]? CitationProofData { get; set; }
        public string? CitationProofFileName { get; set; }


    }
}


