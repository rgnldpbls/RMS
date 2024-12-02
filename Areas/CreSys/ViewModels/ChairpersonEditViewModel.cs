namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class ChairpersonEditViewModel
    {
        public int ChairpersonId { get; set; }
        public string Name { get; set; }
        public string FieldOfStudy { get; set; }
        public List<string> AllFieldsOfStudy { get; set; }
    }
}
