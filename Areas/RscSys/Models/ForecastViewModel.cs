namespace rscSys_final.Models
{
    public class ForecastViewModel
    {
        public List<RequestSpentData> HistoricalData { get; set; }  // Historical data (2019–2024)
        public List<ForecastResult> Forecasts { get; set; }        // Forecasted data (2025–2026)
    }
}

