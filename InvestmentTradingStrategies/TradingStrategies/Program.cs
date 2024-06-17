/* ML code for automated trading strategies */
using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.FastTree;
using Microsoft.ML.Data;

namespace AutomatedTrading
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new ML context for ML.NET operations
            var mlContext = new MLContext();

            // Sample historical stock data (date, open, high, low, close)
            var data = new List<StockData>
            {
                new StockData { Date = DateTime.Parse("2023-01-01"), Open = 100, High = 105, Low = 98, Close = 102 },
                new StockData { Date = DateTime.Parse("2023-01-02"), Open = 102, High = 107, Low = 99, Close = 105 },
                new StockData { Date = DateTime.Parse("2023-01-03"), Open = 105, High = 110, Low = 102, Close = 108 },
                new StockData { Date = DateTime.Parse("2023-01-04"), Open = 108, High = 112, Low = 105, Close = 110 },
                new StockData { Date = DateTime.Parse("2023-01-05"), Open = 110, High = 115, Low = 108, Close = 112 }
            };

            // Convert the data into an IDataView
            IDataView trainingData = mlContext.Data.LoadFromEnumerable<StockData>(data);

            // Define data preparation pipeline
            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "Close")
                .Append(mlContext.Transforms.Concatenate("Features", "Open", "High", "Low"))
                .Append(mlContext.Regression.Trainers.FastTree());

            // Train the model
            var model = pipeline.Fit(trainingData);

            // Sample prediction input (use latest data for prediction)
            var input = new StockData { Date = DateTime.Parse("2023-01-06"), Open = 112, High = 116, Low = 110, Close = 114 };

            // Make a prediction
            var predictionEngine = mlContext.Model.CreatePredictionEngine<StockData, StockPrediction>(model);
            var prediction = predictionEngine.Predict(input);

            Console.WriteLine($"Prediction for {input.Date.ToShortDateString()}: Predicted Close Price = {prediction.PredictedClose}");

            // Example decision based on prediction (simple strategy)
            if (prediction.PredictedClose > input.Close)
            {
                Console.WriteLine("Recommendation: Buy");
            }
            else
            {
                Console.WriteLine("Recommendation: Sell");
            }
        }
    }

    // Data classes
    public class StockData
    {
        public DateTime Date { get; set; }
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
    }

    public class StockPrediction
    {
        [ColumnName("Score")]
        public float PredictedClose { get; set; }
    }
}
