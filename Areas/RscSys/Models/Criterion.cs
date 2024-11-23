namespace rscSys_final.Models
{
    public class Criterion
    {
        public int CriterionId { get; set; }
        public int FormId { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        public EvaluationForm EvaluationForm { get; set; }
        // Navigation property to link to EvaluationFormResponses
        public virtual ICollection<EvaluationFormResponse> EvaluationFormResponses { get; set; } = new List<EvaluationFormResponse>();
    }
}
