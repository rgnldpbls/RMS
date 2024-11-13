namespace rscSys_final.Models
{
    public class BudgetForecastViewModel
    {
        public int Year { get; set; }
        public double ForecastedBudget { get; set; } // Change type to double
        public List<double> ForecastedBudgets { get; set; } // Change type to double
        public List<HistoricalSpending> HistoricalData { get; set; }
    }
}
