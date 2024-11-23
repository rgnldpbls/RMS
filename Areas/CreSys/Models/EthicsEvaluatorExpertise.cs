namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class EthicsEvaluatorExpertise
    {
        public int EthicsEvaluatorId { get; set; }
        public int ExpertiseId { get; set; }


        public EthicsEvaluator EthicsEvaluator { get; set; }
        public Expertise Expertise { get; set; }
    }
}
