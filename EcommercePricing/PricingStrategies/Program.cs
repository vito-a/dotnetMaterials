using System;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using Microsoft.ML.Data;
using System.Collections.Generic;

namespace ProductPricing
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new ML context for ML.NET operations
            var mlContext = new MLContext();

            // Load sample data
            var data = new List<ProductData>
            {
                new ProductData { Category = "Electronics", Sales = 100, Season = "Winter", Price = 199.99f },
                new ProductData { Category = "Electronics", Sales = 150, Season = "Summer", Price = 249.99f },
                new ProductData { Category = "Clothing", Sales = 200, Season = "Winter", Price = 79.99f },
                new ProductData { Category = "Clothing", Sales = 250, Season = "Summer", Price = 89.99f },
                new ProductData { Category = "Books", Sales = 300, Season = "Winter", Price = 9.99f },
                new ProductData { Category = "Books", Sales = 400, Season = "Summer", Price = 12.99f }
            };

            // Convert the data into an IDataView
            IDataView trainingData = mlContext.Data.LoadFromEnumerable(data);

            // Define the data preparation and model training pipeline
            var pipeline = mlContext.Transforms.Categorical.OneHotEncoding(nameof(ProductData.Category), outputKind: OneHotEncodingEstimator.OutputKind.Indicator)
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(nameof(ProductData.Season), outputKind: OneHotEncodingEstimator.OutputKind.Indicator))
                .Append(mlContext.Transforms.Concatenate("Features", nameof(ProductData.Category), nameof(ProductData.Sales), nameof(ProductData.Season)))
                .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Price", featureColumnName: "Features"));

            // Train the model
            var model = pipeline.Fit(trainingData);

            // Create a prediction engine
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ProductData, ProductPricePrediction>(model);

            // Make a prediction for a specific product
            var prediction = predictionEngine.Predict(new ProductData { Category = "Books", Sales = 350, Season = "Winter" });

            Console.WriteLine($"Predicted price for Books with 350 sales in Winter: {prediction.Price}");
        }
    }

    public class ProductData
    {
        public string Category { get; set; }
        public float Sales { get; set; }
        public string Season { get; set; }
        public float Price { get; set; }
    }

    public class ProductPricePrediction
    {
        [ColumnName("Score")]
        public float Price { get; set; }
    }
}
