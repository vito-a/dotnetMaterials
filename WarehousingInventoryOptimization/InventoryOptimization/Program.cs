/* ML code for inventory optimization */
using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;
using Microsoft.ML.TimeSeries;
using Microsoft.ML.Transforms.TimeSeries;

namespace InventoryOptimization
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new ML context for ML.NET operations
            var mlContext = new MLContext();

            // Load sample data (historical inventory levels)
            var data = new List<InventoryData>
            {
                new InventoryData { Date = DateTime.Parse("2023-01-01"), InventoryLevel = 100 },
                new InventoryData { Date = DateTime.Parse("2023-01-02"), InventoryLevel = 110 },
                new InventoryData { Date = DateTime.Parse("2023-01-03"), InventoryLevel = 105 },
                new InventoryData { Date = DateTime.Parse("2023-01-04"), InventoryLevel = 120 },
                new InventoryData { Date = DateTime.Parse("2023-01-05"), InventoryLevel = 115 },
                new InventoryData { Date = DateTime.Parse("2023-01-06"), InventoryLevel = 130 }
            };

            // Convert the data into an IDataView
            IDataView trainingData = mlContext.Data.LoadFromEnumerable(data);

            // Define data preparation pipeline
            var pipeline = mlContext.Transforms.Concatenate("Features", nameof(InventoryData.InventoryLevel))
                .Append(mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: "ForecastedInventory",
                    inputColumnName: "Features",
                    windowSize: 3,
                    seriesLength: data.Count, // Specify series length
                    trainSize: data.Count,   // Use all data for training
                    horizon: 1,              // Predict one step ahead
                    confidenceLevel: 0.95f,  // Confidence level for prediction intervals
                    storageType: Microsoft.ML.Transforms.TimeSeries.DataStorageType.Sparse));

            // Train the model
            var model = pipeline.Fit(trainingData);

            // Create a forecasting engine
            var forecastingEngine = model.CreateTimeSeriesEngine<InventoryData, InventoryPrediction>(mlContext);

            // Make a prediction for future inventory levels
            var prediction = forecastingEngine.Predict();

            Console.WriteLine($"Predicted Inventory Level for next day: {prediction.ForecastedInventory[0]:F2}");
        }
    }

    public class InventoryData
    {
        public DateTime Date { get; set; }

        [LoadColumn(1)]
        public float InventoryLevel { get; set; }
    }

    public class InventoryPrediction
    {
        [ColumnName("ForecastedInventory")]
        public float[] ForecastedInventory { get; set; }
    }
}
