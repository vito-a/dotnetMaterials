/* ML code for portfolio optimization */
using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Trainers.LightGbm;
using Microsoft.ML.Data;

namespace AssetPricePrediction
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new ML context for ML.NET operations
            var mlContext = new MLContext();

            // Sample asset data
            var data = new List<AssetData>
            {
                new AssetData { AssetType = "Gold", MarketPrice = 1800, Year = 2022, Location = "USA", HistoricalPrice = 1750 },
                new AssetData { AssetType = "Silver", MarketPrice = 25, Year = 2022, Location = "Canada", HistoricalPrice = 24 },
                new AssetData { AssetType = "Platinum", MarketPrice = 1000, Year = 2022, Location = "UK", HistoricalPrice = 950 },
                new AssetData { AssetType = "Gold", MarketPrice = 1850, Year = 2023, Location = "USA", HistoricalPrice = 1800 }
            };

            // Convert the data into an IDataView
            IDataView trainingData = mlContext.Data.LoadFromEnumerable(data);

            // Define data preparation pipeline
            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "MarketPrice")
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "AssetTypeEncoded", inputColumnName: "AssetType"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "LocationEncoded", inputColumnName: "Location"))
                .Append(mlContext.Transforms.Concatenate("Features", "AssetTypeEncoded", "Year", "LocationEncoded", "HistoricalPrice"))
                .Append(mlContext.Regression.Trainers.LightGbm());

            // Train the model
            var model = pipeline.Fit(trainingData);

            // Sample prediction input
            var input = new AssetData { AssetType = "Gold", Year = 2023, Location = "USA", HistoricalPrice = 1850 };

            // Make a prediction
            var predictionEngine = mlContext.Model.CreatePredictionEngine<AssetData, AssetPrediction>(model);
            var prediction = predictionEngine.Predict(input);

            Console.WriteLine($"Predicted Price for AssetType {input.AssetType}: {prediction.Score}");

            // Example decision based on prediction (simple strategy)
            if (prediction.Score > 1800)
            {
                Console.WriteLine("Recommendation: Consider buying the asset.");
            }
            else
            {
                Console.WriteLine("Recommendation: Consider selling the asset.");
            }
        }
    }

    // Data classes
    public class AssetData
    {
        public string AssetType { get; set; }
        public float MarketPrice { get; set; }
        public float Year { get; set; }
        public string Location { get; set; }
        public float HistoricalPrice { get; set; }
    }

    public class AssetPrediction
    {
        [ColumnName("Score")]
        public float Score { get; set; }
    }
}
