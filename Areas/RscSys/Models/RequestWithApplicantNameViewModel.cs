namespace rscSys_final.Models
{
    public class RequestWithApplicantNameViewModel
    {
        public string DtsNo { get; set; }
        public string ApplicationType { get; set; }
        public string FieldOfStudy { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal RequestSpent { get; set; }
        public string ApplicantName { get; set; } // New property for applicant name
    }
}
