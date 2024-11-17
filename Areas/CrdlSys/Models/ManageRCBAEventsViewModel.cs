using CrdlSys.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CrdlSys.ViewModels
{
    public class ManageRCBAEventsViewModel
    {
        public string ResearchEventId { get; set; }  
        public byte[] EventThumbnail { get; set; }   
        public string EventName { get; set; }        
        public string EventDescription { get; set; }

        [Required(ErrorMessage = "Event location is required.")]
        [StringLength(500)]
        public string EventLocation { get; set; } 
        
        public string EventType { get; set; }

        [Required(ErrorMessage = "Event date is required.")]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Registration open is required.")]
        public DateTime RegistrationOpen { get; set; }

        [Required(ErrorMessage = "Registration deadline is required.")]
        public DateTime RegistrationDeadline { get; set; } 

        public string EventStatus { get; set; }

        [Required(ErrorMessage = "Participant slot is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Participant slot must be at least 1.")]
        public int ParticipantsSlot { get; set; }  
        
        public int ParticipantsCount { get; set; }   
        public bool IsArchived { get; set; }        
        public DateTime UpdatedAt { get; set; }
        public IEnumerable<ResearchEvent> ResearchEvents { get; set; } = new List<ResearchEvent>();

    }

    public class ManageRCBAEventsPageViewModel
    {
        public List<ManageRCBAEventsViewModel> OpenRegistrationEvents { get; set; }
        public List<ManageRCBAEventsViewModel> InvitationalEvents { get; set; }
        public List<ManageRCBAEventsViewModel> ArchivedEvents { get; set; }
        public List<ManageRCBAEventsViewModel> PostponedEvents { get; set; }


        public ManageRCBAEventsPageViewModel()
        {
            OpenRegistrationEvents = new List<ManageRCBAEventsViewModel>();
            InvitationalEvents = new List<ManageRCBAEventsViewModel>();
            ArchivedEvents = new List<ManageRCBAEventsViewModel>();
            PostponedEvents = new List<ManageRCBAEventsViewModel>();
        }
    }

}
