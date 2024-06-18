/* ML code for connectors count prediction */
using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ConnectorsPrediction
{
    class Program
    {
        static void Main(string[] args)
        {
            MLContext mlContext = new MLContext();

            Balcony[] balconyData = {
                    new Balcony() { A = 500F, L = 180F, HB = 20F, CountConnectors = 10F },
                    new Balcony() { A = 500F, L = 180F, HB = 20F, CountConnectors = 10F },
                    new Balcony() { A = 500F, L = 180F, HB = 20F, CountConnectors = 10F },
                    new Balcony() { A = 500F, L = 180F, HB = 20F, CountConnectors = 10F },
                    new Balcony() { A = 500F, L = 180F, HB = 20F, CountConnectors = 10F },
                    new Balcony() { A = 500F, L = 180F, HB = 20F, CountConnectors = 10F },
                    new Balcony() { A = 500F, L = 180F, HB = 20F, CountConnectors = 10F },
                    new Balcony() { A = 500F, L = 180F, HB = 20F, CountConnectors = 10F },
                    new Balcony() { A = 500F, L = 180F, HB = 20F, CountConnectors = 10F },
                    };

            IDataView trainingData = mlContext.Data.LoadFromEnumerable(balconyData);

            var pipeline = mlContext.Transforms.Concatenate("Features", "A", "L", "HB")
                .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "CountConnectors", maximumNumberOfIterations: 100));

            var model = pipeline.Fit(trainingData);

            var balcony = new Balcony() { A = 500F, L = 180F, HB = 20F };
            var countConnectors = mlContext.Model.CreatePredictionEngine<Balcony, PredictionConnectors>(model).Predict(balcony);

            Console.WriteLine($"Predicted A: {balcony.A}, L: {balcony.L}, HB: {balcony.HB} connectors= {countConnectors.CountConnectors} pcs.");
        }
    }

    public class Balcony
    {
        public float A { get; set; }
        public float L { get; set; }
        public float HB { get; set; }
        public float CountConnectors { get; set; }
    }

    public class PredictionConnectors
    {
        [ColumnName("Score")]
        public float CountConnectors { get; set; }
    }
}