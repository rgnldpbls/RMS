using Microsoft.Identity.Client;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class ReportGenerationViewModel
    {
        // User-selected parameters
        public string? SelectedCollege { get; set; }
        public string SelectedFieldOfStudy { get; set; }
        public string? SelectedCampus{ get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool ExternalApplications { get; set; } = false;
        public string? ReaserchType { get; set; }
        public string? InternalResearcherType { get; set; }
        public string SelectedReportType { get; set; }
        // Dropdown data
        public List<string> Colleges { get; set; } = new List<string>
    {
        "All Colleges",
        "College of Accountancy and Finance (CAF)",
        "College of Architecture, Design and the Built Environment (CADBE)",
        "College of Arts and Letters (CAL)",
        "College of Business Administration (CBA)",
        "College of Communication (COC)",
        "College of Computer and Information Sciences (CCIS)",
        "College of Education (COED)",
        "College of Engineering (CE)",
        "College of Human Kinetics (CHK)",
        "College of Law (CL)",
        "College of Political Science and Public Administration (CPSPA)",
        "College of Social Sciences and Development (CSSD)",
        "College of Science (CS)",
        "College of Tourism, Hospitality and Transportation Management (CTHTM)",
        "Institute of Technology"
    };

        public List<string> FieldsOfStudy { get; set; } = new List<string>
    {
        "All Field of Study",
        "Education",
        "Computer Science, Information Systems, and Technology",
        "Engineering, Architecture, and Design",
        "Humanities, Language, and Communication",
        "Business",
        "Social Sciences",
        "Science, Mathematics, and Statistics"
    }; public List<string> Campuses { get; set; } = new List<string>
        {
            "Whole University",
           "Sta. Mesa (MAIN CAMPUS)",
            "Taguig City (BRANCH)",
            "Quezon City (BRANCH)",
            "San Juan City (BRANCH)",
            "Parañaque City (CAMPUS)",
            "Bataan (BRANCH)",
            "Sta. Maria, Bulacan (CAMPUS)",
            "Pulilan, Bulacan (CAMPUS)",
            "Cabiao, Nueva Ecija (CAMPUS)",
            "Lopez, Quezon (BRANCH)",
            "Malunay, Quezon (BRANCH)",
            "Unisan, Quezon (BRANCH)",
            "Ragay, Camarines Sur (BRANCH)",
            "Sto. Tomas, Batangas (BRANCH)",
            "Maragondon, Cavite (BRANCH)",
            "Bansud, Oriental Mindoro (BRANCH)",
            "Sablayan, Occidental Mindoro (BRANCH)",
            "Biñan, Laguna (CAMPUS)",
            "San Pedro, Laguna (CAMPUS)",
            "Sta. Rosa, Laguna (CAMPUS)",
            "Calauan, Laguna (CAMPUS)"
        };
    }


}
