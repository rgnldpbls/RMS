namespace rscSys_final.Models
{
    public class EvaluationViewModel
    {
        public int EvaluatorAssignmentId { get; set; }
        public string DtsNo { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicationType { get; set; }
        public string FieldOfStudy { get; set; }
        public DateTime FiledDate { get; set; }
        public string Status { get; set; }
        public List<Requirement>? SubmittedDocuments { get; set; }  // List of submitted documents
        public string OwnerEmail { get; set; } // Add this property
        public List<EvaluationForm> EvaluationForms { get; set; } = new List<EvaluationForm>();

        /*public List<EvaluationFormResponse> UserResponses { get; set; } = new List<EvaluationFormResponse>();*/

        // For general comments
        public string Comments { get; set; } // Property for general comments
    }
}
