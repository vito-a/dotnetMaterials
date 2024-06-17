using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;

namespace CompanyOrgCharts
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new ML context for ML.NET operations
            var mlContext = new MLContext();

            // Sample data for demonstration
            var data = new List<Employee>
            {
                new Employee { EmployeeId = 1, Department = "HR", ManagerId = 4 },
                new Employee { EmployeeId = 2, Department = "IT", ManagerId = 3 },
                new Employee { EmployeeId = 3, Department = "IT", ManagerId = 4 },
                new Employee { EmployeeId = 4, Department = "CEO", ManagerId = 0 },
                new Employee { EmployeeId = 5, Department = "Finance", ManagerId = 4 },
                new Employee { EmployeeId = 6, Department = "IT", ManagerId = 3 }
            };

            // Convert the data into an IDataView
            IDataView trainingData = mlContext.Data.LoadFromEnumerable<Employee>(data);

            // Define the data preparation pipeline
            var pipeline = mlContext.Transforms.Conversion.ConvertType("ManagerIdString", nameof(Employee.ManagerId), DataKind.String)
                .Append(mlContext.Transforms.Concatenate("Features", nameof(Employee.Department), "ManagerIdString"))
                .Append(mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(Employee.EmployeeId)))
                .Append(mlContext.Transforms.NormalizeMinMax("Features"));

            // Train the model
            var model = pipeline.Fit(trainingData);

            // Create a prediction engine
            var predictionEngine = mlContext.Model.CreatePredictionEngine<Employee, EmployeePrediction>(model);

            // Sample input to predict the EmployeeId
            var input = new Employee { Department = "HR", ManagerId = 4 };

            // Predict the EmployeeId
            var prediction = predictionEngine.Predict(input);

            Console.WriteLine($"Predicted EmployeeId for Department: {input.Department}, ManagerId: {input.ManagerId} is {prediction.PredictedEmployeeId}");
        }
    }

    // Data classes
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Department { get; set; }
        public int ManagerId { get; set; }
    }

    public class EmployeePrediction
    {
        [ColumnName("PredictedLabel")]
        public float PredictedEmployeeId { get; set; }
    }
}
