namespace rscSys_final.Models
{
    public class ReportData
    {
        public int Year { get; set; }
        public int TotalRequests { get; set; }
        public double TotalSpent { get; set; }
        public List<rscSys_final.Models.Request> Requests { get; set; }
    }
}
