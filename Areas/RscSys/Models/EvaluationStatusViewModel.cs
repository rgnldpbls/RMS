namespace rscSys_final.Models
{
    public class EvaluationStatusViewModel
    {
        public Request Request { get; set; }
        public List<EvaluatorAssignment> EvaluatorAssignments { get; set; }
        public List<EvaluationDocument> EvaluationDocuments { get; set; }
        public List<EvaluationGeneralComment> GeneralComments { get; set; }

        // New properties for average percentage and decision
        public decimal AverageUserPercentage { get; set; }
        public string Decision { get; set; }
        public string EvaluationMessage { get; set; }

    }
}
