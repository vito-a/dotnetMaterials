using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;

namespace GoodsIdentification
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new ML context for ML.NET operations
            var mlContext = new MLContext();

            // Load sample data
            var data = new List<GoodsData>
            {
                new GoodsData { Size = 1.1f, Weight = 2.2f, Type = "A" },
                new GoodsData { Size = 0.9f, Weight = 1.8f, Type = "B" },
                new GoodsData { Size = 1.0f, Weight = 2.0f, Type = "A" },
                new GoodsData { Size = 1.2f, Weight = 2.5f, Type = "C" },
                new GoodsData { Size = 1.5f, Weight = 3.0f, Type = "B" },
                new GoodsData { Size = 1.3f, Weight = 2.7f, Type = "A" }
            };

            // Convert the data into an IDataView
            IDataView trainingData = mlContext.Data.LoadFromEnumerable(data);

            // Define data preparation pipeline
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(GoodsData.Type))
                .Append(mlContext.Transforms.Concatenate("Features", nameof(GoodsData.Size), nameof(GoodsData.Weight)))
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedType", "Label"));

            // Define the trainer
            var trainer = mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy();

            // Wrap the trainer in the pipeline
            var trainingPipeline = pipeline.Append(trainer);

            // Train the model
            var model = trainingPipeline.Fit(trainingData);

            // Create a prediction engine
            var predictionEngine = mlContext.Model.CreatePredictionEngine<GoodsData, GoodsPrediction>(model);

            // Make a prediction for a new data point
            var prediction = predictionEngine.Predict(new GoodsData { Size = 1.4f, Weight = 2.8f });

            Console.WriteLine($"Predicted Type: {prediction.PredictedType}");
        }
    }

    public class GoodsData
    {
        public float Size { get; set; }
        public float Weight { get; set; }
        public string Type { get; set; }
    }

    public class GoodsPrediction
    {
        [ColumnName("PredictedType")]
        public string PredictedType { get; set; }
    }
}
