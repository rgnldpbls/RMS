using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Models
{
    public class AddPresentation
    {
        [Key]
        public string ConferenceId { get; set; } = Guid.NewGuid().ToString();


        [ForeignKey(nameof(AddAccomplishment))]
        [Display(Name = "ProductionNo")]
        public virtual string? ProductionId { get; set; }

        public AddAccomplishment? AddAccomplishment { get; set; }



        [Display(Name = "Organizer")]
        public string? OrganizerOne { get; set; }


        [Display(Name = "Organizer")]
        public string? OrganizerTwo { get; set; }

        [Display(Name = "Presenter")]
        public string? PresenterOne { get; set; }

        [Display(Name = "Presenter")]
        public string? PresenterTwo { get; set; }

        [Display(Name = "Presenter")]
        public string? PresenterThree { get; set; }

        [Display(Name = "Presenter")]
        public string? PresenterFour { get; set; }

        [Display(Name = "Presenter")]
        public string? PresenterFive { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Prensentation")]
        public DateTime? DateofPresentation { get; set; }

        [Display(Name = "Level")]
        public string? Level { get; set; }

        [Display(Name = "Venue")]
        public string? Venue { get; set; }



        // File properties to store binary data
        public byte[]? CertificateofParticipationFileData { get; set; }
        public string? CertificateofParticipationFileName { get; set; }



    }
}


