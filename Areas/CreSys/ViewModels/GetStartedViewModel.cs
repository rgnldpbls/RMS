using ResearchManagementSystem.Areas.CreSys.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class GetStartedViewModel
    {
        public List<Expertise> ExpertiseList { get; set; } // List of all expertise options
        public List<int> SelectedExpertise { get; set; } // IDs of selected expertise
    }
}
