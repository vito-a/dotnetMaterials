/* ML code for crypto currencies price prediction */
using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace CryptoPricePrediction
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new ML context for ML.NET operations
            var mlContext = new MLContext();

            // Sample historical cryptocurrency price data (date, open, high, low, close, volume)
            var data = new List<CryptoData>
            {
                new CryptoData { Date = DateTime.Parse("2023-01-01"), Open = 1000, High = 1050, Low = 980, Close = 1020, Volume = 5000 },
                new CryptoData { Date = DateTime.Parse("2023-01-02"), Open = 1020, High = 1070, Low = 1000, Close = 1050, Volume = 5500 },
                new CryptoData { Date = DateTime.Parse("2023-01-03"), Open = 1050, High = 1100, Low = 1030, Close = 1080, Volume = 6000 },
                new CryptoData { Date = DateTime.Parse("2023-01-04"), Open = 1080, High = 1120, Low = 1060, Close = 1100, Volume = 6500 },
                new CryptoData { Date = DateTime.Parse("2023-01-05"), Open = 1100, High = 1150, Low = 1080, Close = 1120, Volume = 7000 }
            };

            // Convert the data into an IDataView
            IDataView trainingData = mlContext.Data.LoadFromEnumerable<CryptoData>(data);

            // Define data preparation pipeline
            /*
            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "Close")
                .Append(mlContext.Transforms.Concatenate("Features", "Open", "High", "Low", "Volume"))
                .Append(mlContext.Regression.Trainers.FastTree());
            */
            /*
            var pipeline = mlContext.Transforms.Concatenate("Features", "Open", "High", "Low", "Volume")
                .Append(mlContext.Regression.Trainers.FastTree(labelColumnName: "Open"));
            */
            /*
            var pipeline = mlContext.Transforms.Concatenate("Features", "Open", "High", "Low", "Volume")
                .Append(mlContext.Regression.Trainers.FastTreeTweedie(labelColumnName: "Open", featureColumnName: "Features"));
            */
            var pipeline = mlContext.Transforms.Concatenate("Features", "Open", "High", "Low", "Volume")
                .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Open", maximumNumberOfIterations: 2000));

            // Train the model
            var model = pipeline.Fit(trainingData);

            // Sample prediction input (use latest data for prediction)
            var input = new CryptoData { Date = DateTime.Parse("2023-01-06"), Open = 1120, High = 2160, Low = 1100, Close = 1140, Volume = 12500 };

            // Make a prediction
            var predictionEngine = mlContext.Model.CreatePredictionEngine<CryptoData, CryptoPrediction>(model);
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
    public class CryptoData
    {
        public DateTime Date { get; set; }
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
        public float Volume { get; set; }
    }

    public class CryptoPrediction
    {
        [ColumnName("Score")]
        public float PredictedClose { get; set; }
    }
}
