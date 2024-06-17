using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;

namespace ShopAggregator
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
                new ProductData { ProductId = 1, Category = "Electronics", Price = 199.99f },
                new ProductData { ProductId = 2, Category = "Books", Price = 9.99f },
                new ProductData { ProductId = 3, Category = "Clothing", Price = 49.99f }
            };

            // Convert the data into an IDataView
            IDataView trainingData = mlContext.Data.LoadFromEnumerable(data);

            // Define the data preparation and model training pipeline
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(ProductData.Category))
                .Append(mlContext.Transforms.Concatenate("Features", nameof(ProductData.Price)))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "Label"));

            // Train the model
            var model = pipeline.Fit(trainingData);

            // Make predictions
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ProductData, ProductPrediction>(model);
            var prediction = predictionEngine.Predict(new ProductData { Price = 99.99f });

            Console.WriteLine($"Predicted category for price 99.99: {prediction.Category}");
        }
    }

    public class ProductData
    {
        public float Price { get; set; }
        public string Category { get; set; }
        public int ProductId { get; set; }
    }

    public class ProductPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Category { get; set; }
    }
}
