using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Models
{
    public class AddPublication
    {
        [Key]
        public string publicationId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(AddAccomplishment))]
        [Display(Name = "ProductionNo")]
        public virtual string? ProductionId { get; set; }

        public AddAccomplishment? AddAccomplishment { get; set; }

        [Display(Name = "Article/Title")]
        public string? ArticleTitle { get; set; }

        [Display(Name = "Date of Publication")]
        public DateTime? PublicationDate { get; set; }

        [Display(Name = "Title of Journal Publication")]
        public string? JournalPubTitle { get; set; }

        [Display(Name = "DocumentType")]
        public string? DocumentType { get; set; }

        [Display(Name = "Vol.No and Issue.No")]
        public string? VolnoIssueNo { get; set; }

        [Display(Name = "ISSN/ISBN/ESSN")]
        public string? IssnIsbnEssn { get; set; }

        [Display(Name = "DOI")]
        public string? DOI { get; set; }

        [Display(Name = "Indicate where the Journal is Index")]
        public string? IndexJournal { get; set; }

        [Display(Name = "Supporting Document")]
        public string? SuppDocs { get; set; }

        [Display(Name = "Link")]
        public string? Link { get; set; }

        public byte[]? ManuscriptJournalData { get; set; }
        public string? ManuscriptJournalFileName { get; set; }

    }
}


