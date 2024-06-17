using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.TimeSeries;
using System.Collections.Generic;

namespace DemandForecasts
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new ML context for ML.NET operations
            var mlContext = new MLContext();

            // Load sample data
            var data = new List<DemandData>
            {
                new DemandData { Date = DateTime.Parse("2023-01-01"), ProductId = 1, Demand = 100 },
                new DemandData { Date = DateTime.Parse("2023-01-02"), ProductId = 1, Demand = 120 },
                new DemandData { Date = DateTime.Parse("2023-01-03"), ProductId = 1, Demand = 130 },
                new DemandData { Date = DateTime.Parse("2023-01-04"), ProductId = 1, Demand = 110 },
                new DemandData { Date = DateTime.Parse("2023-01-05"), ProductId = 1, Demand = 105 },
                new DemandData { Date = DateTime.Parse("2023-01-06"), ProductId = 1, Demand = 115 }
            };

            // Convert the data into an IDataView
            IDataView trainingData = mlContext.Data.LoadFromEnumerable(data);

            // Define the data preparation and model training pipeline
            var pipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(DemandPrediction.ForecastedDemand),
                inputColumnName: nameof(DemandData.Demand),
                windowSize: 3,
                seriesLength: 6,
                trainSize: 6,
                horizon: 1);

            // Train the model
            var model = pipeline.Fit(trainingData);

            // Create a prediction engine
            var forecastEngine = model.CreateTimeSeriesEngine<DemandData, DemandPrediction>(mlContext);

            // Make a prediction for the next time step
            var forecast = forecastEngine.Predict();

            Console.WriteLine($"Forecasted demand for the next time step: {forecast.ForecastedDemand[0]}");
        }
    }

    public class DemandData
    {
        [LoadColumn(0)]
        public DateTime Date { get; set; }

        [LoadColumn(1)]
        public float ProductId { get; set; }

        [LoadColumn(2)]
        public float Demand { get; set; }
    }

    public class DemandPrediction
    {
        public float[]? ForecastedDemand { get; set; }
    }
}
