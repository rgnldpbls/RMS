namespace rscSys_final.Models
{
    public class EvaluationForm
    {
        public int FormId { get; set; }
        public string Title { get; set; }
        public List<Criterion> Criteria { get; set; } = new List<Criterion>();
    }
}
