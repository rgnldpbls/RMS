    using CRE.Models;

    namespace CRE.ViewModels
    {
        public class EvaluatedExpeditedApplication
        {
            public EthicsApplication EthicsApplication { get; set; }
            public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
            public ICollection<EthicsEvaluation> EthicsEvaluation { get; set; }
            public List<EthicsEvaluator> EthicsEvaluators { get; set; } // Add this line
            public Faculty Faculty { get; set; }
            public InitialReview InitialReview { get; set; }
            public ICollection<EthicsApplicationLog> EthicsApplicationLog { get; set; }

        }
    }
