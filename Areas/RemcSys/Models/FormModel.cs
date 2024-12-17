using System.ComponentModel.DataAnnotations;
namespace RemcSys.Models
{
    public class FormModel
    {
        [Required(ErrorMessage = "Research Type is required.")]
        public string ResearchType { get; set; }
        [Required(ErrorMessage = "Project Title is required.")]
        [RegularExpression(@"\S.*", ErrorMessage = "Project Title cannot be empty or whitespace.")]
        public string ProjectTitle { get; set; }
        [Required(ErrorMessage = "Project Leader is required.")]
        public string ProjectLeader { get; set; }
        [Required(ErrorMessage = "At least one project member is required.")]
        public string ProjectMembers { get; set; }
        [Required(ErrorMessage = "Implementing Institution is required.")]
        [RegularExpression(@"\S.*", ErrorMessage = "Implementing Institution cannot be empty or whitespace.")]
        public string ImplementingInstitution { get; set; }
        [Required(ErrorMessage = "Collaborating Institution is required.")]
        [RegularExpression(@"\S.*", ErrorMessage = "Collaborating Institution cannot be empty or whitespace.")]
        public string CollaboratingInstitution { get; set; }
        [Required(ErrorMessage = "Project Duration is required.")]
        public string ProjectDuration { get; set; }
        [Required(ErrorMessage = "Total Project Cost is required.")]
        [RegularExpression(@"\S.*", ErrorMessage = "Total Project Cost cannot be empty or whitespace.")]
        public string TotalProjectCost { get; set; }
        [Required(ErrorMessage = "Objectives are required.")]
        [RegularExpression(@"\S.*", ErrorMessage = "Objectives cannot be empty or whitespace.")]
        public string Objectives { get; set; }
        [Required(ErrorMessage = "Scope is required.")]
        [RegularExpression(@"\S.*", ErrorMessage = "Scope cannot be empty or whitespace.")]
        public string Scope { get; set; }
        [Required(ErrorMessage = "Methodology is required.")]
        [RegularExpression(@"\S.*", ErrorMessage = "Methodology cannot be empty or whitespace.")]
        public string Methodology { get; set; }
        [Required(ErrorMessage = "Study Field is required.")]
        public string StudyField {  get; set; }
        [RegularExpression(@"\S.*", ErrorMessage = "External Funding Agency cannot be empty or whitespace.")]
        public string? NameOfExternalFundingAgency { get; set; }
    }

    public class ViewEvaluationVM
    {
        public string evaluation_Id { get; set; }
        public string fra_Id { get; set; }
        public string dts_No { get; set; }
        public string research_Title { get; set; }
        public string field_of_Study { get; set; }
        public string application_Status { get; set; }
        public DateTime? evaluation_deadline { get; set; }
    }

    public class ViewChiefEvaluationVM
    {
        public string evaluator_Name { get; set; }
        public List<string> field_of_Interest { get; set; }
        public double? evaluation_Grade { get; set; }
        public string remarks { get; set; }
    }

    public class ViewNTP
    {
        public string dts_No { get; set; }
        public string research_Title { get; set; }
        public string field_of_Study { get; set; }
        public string fra_Type {  get; set; }
        public string fra_Id { get; set; }
        public string? fre_Id { get; set; }
        public byte[]? clearanceFile {  get; set; }
        public string? file_Status {  get; set; }

    }
}
