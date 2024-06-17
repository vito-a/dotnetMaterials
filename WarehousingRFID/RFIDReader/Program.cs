using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;

namespace WarehousingGoodsLocation
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new ML context for ML.NET operations
            var mlContext = new MLContext();

            // Sample data for demonstration
            var data = new List<WarehouseItem>
            {
                new WarehouseItem { RFID = "RFID001", Location = "A1" },
                new WarehouseItem { RFID = "RFID002", Location = "B3" },
                new WarehouseItem { RFID = "RFID003", Location = "C2" },
                new WarehouseItem { RFID = "RFID004", Location = "A4" },
                new WarehouseItem { RFID = "RFID005", Location = "B1" },
                new WarehouseItem { RFID = "RFID006", Location = "C3" }
            };

            // Convert the data into an IDataView
            IDataView trainingData = mlContext.Data.LoadFromEnumerable(data);

            // Define the data preparation pipeline
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(WarehouseItem.Location))
                .Append(mlContext.Transforms.Text.FeaturizeText("Features", nameof(WarehouseItem.RFID)))
                .Append(mlContext.Transforms.Concatenate("Features", "Features"))
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLocation", "Label"));

            // Train the model
            var model = pipeline.Fit(trainingData);

            // Create a prediction engine
            var predictionEngine = mlContext.Model.CreatePredictionEngine<WarehouseItem, WarehouseItemPrediction>(model);

            // Sample RFID or barcode to locate
            var input = new WarehouseItem { RFID = "RFID003" };

            // Predict the location
            var prediction = predictionEngine.Predict(input);

            Console.WriteLine($"Predicted Location for RFID {input.RFID}: {prediction.PredictedLocation}");
        }
    }

    // Data classes
    public class WarehouseItem
    {
        public string RFID { get; set; }
        public string Location { get; set; }
    }

    public class WarehouseItemPrediction
    {
        [ColumnName("PredictedLocation")]
        public string PredictedLocation { get; set; }
    }
}
