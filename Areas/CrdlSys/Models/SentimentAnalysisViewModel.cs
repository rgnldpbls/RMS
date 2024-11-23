namespace CrdlSys.Models
{
    public class SentimentAnalysisViewModel
    {
        public IEnumerable<GeneratedSentimentAnalysis> Analyses { get; set; }
        public GeneratedSentimentAnalysis NewAnalysis { get; set; }
    }
}
