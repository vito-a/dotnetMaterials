/* ML code for loan default prediction */
using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace LoanDefaultPrediction
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new ML context for ML.NET operations
            var mlContext = new MLContext();

            // Sample loan data
            var data = new List<LoanData>
            {
                new LoanData { LoanAmount = 10000, InterestRate = 0.05f, LoanTerm = 30, MonthlyIncome = 3000, CreditScore = 700, IsDefault = false },
                new LoanData { LoanAmount = 15000, InterestRate = 0.06f, LoanTerm = 30, MonthlyIncome = 4000, CreditScore = 650, IsDefault = true },
                new LoanData { LoanAmount = 20000, InterestRate = 0.04f, LoanTerm = 20, MonthlyIncome = 5000, CreditScore = 720, IsDefault = false },
                new LoanData { LoanAmount = 5000, InterestRate = 0.07f, LoanTerm = 15, MonthlyIncome = 2500, CreditScore = 600, IsDefault = true }
            };

            // Convert the data into an IDataView
            IDataView trainingData = mlContext.Data.LoadFromEnumerable(data);

            // Define data preparation pipeline
            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "IsDefault")
                .Append(mlContext.Transforms.Concatenate("Features", "LoanAmount", "InterestRate", "LoanTerm", "MonthlyIncome", "CreditScore"))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(maximumNumberOfIterations: 100));

            // Train the model
            var model = pipeline.Fit(trainingData);

            // Sample prediction input
            var input = new LoanData { LoanAmount = 12000, InterestRate = 0.055f, LoanTerm = 25, MonthlyIncome = 3500, CreditScore = 680 };

            // Make a prediction
            var predictionEngine = mlContext.Model.CreatePredictionEngine<LoanData, LoanPrediction>(model);
            var prediction = predictionEngine.Predict(input);

            Console.WriteLine($"Prediction: {(prediction.PredictedLabel ? "Default" : "No Default")} with Probability: {prediction.Probability}");

            // Example decision based on prediction (simple strategy)
            if (prediction.PredictedLabel)
            {
                Console.WriteLine("Recommendation: Reject the loan application.");
            }
            else
            {
                Console.WriteLine("Recommendation: Approve the loan application.");
            }
        }
    }

    // Data classes
    public class LoanData
    {
        public float LoanAmount { get; set; }
        public float InterestRate { get; set; }
        public float LoanTerm { get; set; }
        public float MonthlyIncome { get; set; }
        public float CreditScore { get; set; }
        public bool IsDefault { get; set; }
    }

    public class LoanPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool PredictedLabel { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
