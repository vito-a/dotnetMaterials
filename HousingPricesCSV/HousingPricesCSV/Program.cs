using System;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System.Globalization;
using System.Reflection;

class Program
{
    public class HouseData
    {
        [LoadColumn(0)] public string RegionID { get; set; }
        [LoadColumn(1)] public int SizeRank { get; set; }
        [LoadColumn(2)] public string RegionName { get; set; }
        [LoadColumn(3)] public string RegionType { get; set; }
        [LoadColumn(4)] public string StateName { get; set; }
        [LoadColumn(5)] public string State { get; set; }
        [LoadColumn(6)] public string Metro { get; set; }
        [LoadColumn(7)] public string CountyName { get; set; }
        [LoadColumn(8, 301)] public float[] Prices { get; set; }
    }

    public class ProcessedData
    {
        public float DateIndex { get; set; }
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

        // Load data from CSV
        string dataPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"../../../Data/Vienna.csv");
        IDataView dataView = mlContext.Data.LoadFromTextFile<HouseData>(dataPath, hasHeader: true, separatorChar: ',');

        // Preprocess the data: flatten the time series
        var processedData = mlContext.Data.CreateEnumerable<HouseData>(dataView, reuseRowObject: false)
            .SelectMany(house => house.Prices.Select((price, index) => new ProcessedData
            {
                DateIndex = index,
                Price = price
            }));

        IDataView trainingData = mlContext.Data.LoadFromEnumerable(processedData);

        // Define the training pipeline
        var pipeline = mlContext.Transforms.Concatenate("Features", nameof(ProcessedData.DateIndex))
            .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(ProcessedData.Price), maximumNumberOfIterations: 100));

        // Train the model
        var model = pipeline.Fit(trainingData);

        // Make a prediction
        var predictionEngine = mlContext.Model.CreatePredictionEngine<ProcessedData, Prediction>(model);
        var prediction = predictionEngine.Predict(new ProcessedData { DateIndex = 300 }); // Predicting for the last date in the dataset

        Console.WriteLine($"Predicted price for date index 300: {prediction.Price:C}");
    }
}
