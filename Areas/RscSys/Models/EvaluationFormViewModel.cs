namespace rscSys_final.Models
{
    public class EvaluationFormViewModel
    {
        public int SelectedApplicationTypeId { get; set; }
        public List<ApplicationType> ApplicationTypes { get; set; } = new List<ApplicationType>();
        public List<CriterionViewModel> Criteria { get; set; } = new List<CriterionViewModel>();
        public List<EvaluationFormResponse>? Responses { get; set; }
        public string? GeneralComment { get; set; } // For general comments
    }
}
