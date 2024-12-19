using Microsoft.ML.Data;

namespace rscSys_final.Models
{
    public class RequestForecast
    {
        [VectorType(2)] // 2 years of forecasted values
        public float[] ForecastedSpent { get; set; } // Predicted values

    }
}
