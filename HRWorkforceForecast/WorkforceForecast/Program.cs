/* ML code for workforce demand forecasts */
using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;

namespace WorkforceDemandForecast
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new ML context for ML.NET operations
            var mlContext = new MLContext();

            // Sample data for demonstration
            var data = new List<WorkforceData>
            {
                new WorkforceData { Month = 1, Year = 2023, NumberOfEmployees = 100 },
                new WorkforceData { Month = 2, Year = 2023, NumberOfEmployees = 110 },
                new WorkforceData { Month = 3, Year = 2023, NumberOfEmployees = 105 },
                new WorkforceData { Month = 4, Year = 2023, NumberOfEmployees = 115 },
                new WorkforceData { Month = 5, Year = 2023, NumberOfEmployees = 120 },
                new WorkforceData { Month = 6, Year = 2023, NumberOfEmployees = 125 }
            };

            // Convert the data into an IDataView
            IDataView trainingData = mlContext.Data.LoadFromEnumerable<WorkforceData>(data);

            // Define the data preparation pipeline
            var pipeline = mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: nameof(WorkforcePrediction.ForecastedNumberOfEmployees),
                    inputColumnName: nameof(WorkforceData.NumberOfEmployees),
                    windowSize: 5,
                    seriesLength: 6,
                    trainSize: 6,
                    horizon: 1,
                    confidenceLevel: 0.95f,
                    confidenceLowerBoundColumn: "LowerBoundNumberOfEmployees",
                    confidenceUpperBoundColumn: "UpperBoundNumberOfEmployees")
                .Append(mlContext.Transforms.CopyColumns(outputColumnName: "ForecastedNumberOfEmployees", inputColumnName: nameof(WorkforcePrediction.ForecastedNumberOfEmployees)));

            // Train the model
            var model = pipeline.Fit(trainingData);

            // Create a forecasting engine
            var forecastingEngine = model.CreateTimeSeriesEngine<WorkforceData, WorkforcePrediction>(mlContext);

            // Sample input for forecasting
            var input = new WorkforceData { Month = 7, Year = 2023 };

            // Forecast the number of employees for the next month
            var forecast = forecastingEngine.Predict();

            Console.WriteLine($"Forecasted number of employees for {input.Month}/{input.Year}:");
            Console.WriteLine($"Predicted: {forecast.ForecastedNumberOfEmployees}");
            Console.WriteLine($"Lower Bound (95% confidence): {forecast.LowerBoundNumberOfEmployees}");
            Console.WriteLine($"Upper Bound (95% confidence): {forecast.UpperBoundNumberOfEmployees}");
        }
    }

    // Data classes
    public class WorkforceData
    {
        public float Month { get; set; }
        public float Year { get; set; }
        public float NumberOfEmployees { get; set; }
    }

    public class WorkforcePrediction
    {
        [ColumnName("ForecastedNumberOfEmployees")]
        public float ForecastedNumberOfEmployees { get; set; }

        [ColumnName("LowerBoundNumberOfEmployees")]
        public float LowerBoundNumberOfEmployees { get; set; }

        [ColumnName("UpperBoundNumberOfEmployees")]
        public float UpperBoundNumberOfEmployees { get; set; }
    }
}
