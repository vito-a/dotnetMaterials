using System;
using Microsoft.ML;
using Microsoft.ML.Data;

class Program
{
    public class HouseData
    {
        public float Size { get; set; }
        public float Price { get; set; }
    }

    public class Prediction
    {
        [ColumnName("Score")]
        public float Price { get; set; }
    }

    static void Main(string[] args)
    {
        MLContext mlContext = new MLContext();

        // 1. Import or create training data
        HouseData[] houseData = {
            new HouseData() { Size = 1.1F, Price = 1.2F },
            new HouseData() { Size = 1.2F, Price = 1.4F },
            new HouseData() { Size = 1.3F, Price = 1.6F },
            new HouseData() { Size = 1.5F, Price = 1.9F },
            new HouseData() { Size = 1.7F, Price = 2.1F },
            new HouseData() { Size = 1.9F, Price = 2.3F },
            new HouseData() { Size = 2.1F, Price = 2.3F },
            new HouseData() { Size = 2.2F, Price = 2.5F },
            new HouseData() { Size = 2.3F, Price = 2.7F },
            new HouseData() { Size = 2.7F, Price = 3.0F },
            new HouseData() { Size = 2.9F, Price = 3.2F },
            new HouseData() { Size = 3.1F, Price = 3.4F },
            new HouseData() { Size = 3.2F, Price = 3.6F },
            new HouseData() { Size = 3.3F, Price = 3.9F },
            new HouseData() { Size = 3.7F, Price = 4.1F },
            new HouseData() { Size = 3.9F, Price = 4.3F } };
        IDataView trainingData = mlContext.Data.LoadFromEnumerable(houseData);

        // 2. Specify data preparation and model training pipeline
        var pipeline = mlContext.Transforms.Concatenate("Features", new[] { "Size" })
            .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Price", maximumNumberOfIterations: 100));

        // 3. Train model
        var model = pipeline.Fit(trainingData);

        // 4. Make a prediction
        var size = new HouseData() { Size = 2.5F };
        var price = mlContext.Model.CreatePredictionEngine<HouseData, Prediction>(model).Predict(size);

        Console.WriteLine($"Predicted price for size: {size.Size*1000} sq ft= {price.Price*100:C}k");

        // Predicted price for size: 2500 sq ft= $261.98k
    }
}
