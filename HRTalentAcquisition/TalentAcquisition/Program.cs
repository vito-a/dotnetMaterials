using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;

namespace TalentAcquisition
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new ML context for ML.NET operations
            var mlContext = new MLContext();

            // Sample data for demonstration
            var data = new List<CandidateData>
            {
                new CandidateData { YearsExperience = 3, EducationLevel = "Bachelor's", HasInternshipExperience = true, IsSuitable = true },
                new CandidateData { YearsExperience = 1, EducationLevel = "Master's", HasInternshipExperience = false, IsSuitable = false },
                new CandidateData { YearsExperience = 2, EducationLevel = "Bachelor's", HasInternshipExperience = true, IsSuitable = true },
                new CandidateData { YearsExperience = 4, EducationLevel = "PhD", HasInternshipExperience = true, IsSuitable = true },
                new CandidateData { YearsExperience = 0, EducationLevel = "Bachelor's", HasInternshipExperience = false, IsSuitable = false }
            };

            // Convert the data into an IDataView
            IDataView trainingData = mlContext.Data.LoadFromEnumerable<CandidateData>(data);

            // Define data preparation pipeline
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(CandidateData.IsSuitable))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding("EducationLevel"))
                .Append(mlContext.Transforms.Concatenate("Features", nameof(CandidateData.YearsExperience), "EducationLevel"))
                .Append(mlContext.Transforms.Conversion.ConvertType("HasInternshipExperience", nameof(CandidateData.HasInternshipExperience), DataKind.String))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

            // Train the model
            var model = pipeline.Fit(trainingData);

            // Sample prediction input
            var input = new CandidateData { YearsExperience = 2, EducationLevel = "Bachelor's", HasInternshipExperience = false };

            // Make a prediction
            var predictionEngine = mlContext.Model.CreatePredictionEngine<CandidateData, CandidatePrediction>(model);
            var prediction = predictionEngine.Predict(input);

            Console.WriteLine($"Prediction for candidate with {input.YearsExperience} years of experience, {input.EducationLevel}, and internship experience: {(prediction.Prediction ? "Suitable" : "Not Suitable")}");
        }
    }

    // Data classes
    public class CandidateData
    {
        public float YearsExperience { get; set; }
        public string EducationLevel { get; set; }
        public bool HasInternshipExperience { get; set; }
        public bool IsSuitable { get; set; }
    }

    public class CandidatePrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }
    }
}
