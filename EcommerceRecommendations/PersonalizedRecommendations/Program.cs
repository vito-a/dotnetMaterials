/* ML code for personalized recommendations */
using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;

namespace PersonalizedRecommendations
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new ML context for ML.NET operations
            var mlContext = new MLContext();

            // Load sample data
            var data = new List<ProductRating>
            {
                new ProductRating { UserId = 1, ProductId = 1, Label = 4 },
                new ProductRating { UserId = 1, ProductId = 2, Label = 5 },
                new ProductRating { UserId = 2, ProductId = 1, Label = 5 },
                new ProductRating { UserId = 2, ProductId = 3, Label = 3 },
                new ProductRating { UserId = 3, ProductId = 2, Label = 4 },
                new ProductRating { UserId = 3, ProductId = 3, Label = 5 },
            };

            // Convert the data into an IDataView
            IDataView trainingData = mlContext.Data.LoadFromEnumerable(data);

            // Define the data preparation and model training pipeline
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductRating.UserId))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductRating.ProductId)))
                .Append(mlContext.Recommendation().Trainers.MatrixFactorization(
                    labelColumnName: "Label",
                    matrixColumnIndexColumnName: nameof(ProductRating.UserId),
                    matrixRowIndexColumnName: nameof(ProductRating.ProductId)));

            // Train the model
            var model = pipeline.Fit(trainingData);

            // Create a prediction engine
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ProductRating, ProductRatingPrediction>(model);

            // Make a prediction for a specific user and product
            var prediction = predictionEngine.Predict(new ProductRating { UserId = 1, ProductId = 3 });

            Console.WriteLine($"Predicted rating for UserId 1 and ProductId 3: {prediction.Score}");
        }
    }

    public class ProductRating
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public float Label { get; set; }
    }

    public class ProductRatingPrediction
    {
        public float Score { get; set; }
    }
}
