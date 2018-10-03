// <Snippet17>
using System;
using System.IO;
// </Snippet17>
// <Snippet1>
using Microsoft.ML.Legacy;
using Microsoft.ML.Legacy.Data;
using Microsoft.ML.Legacy.Models;
using Microsoft.ML.Legacy.Trainers;
using Microsoft.ML.Legacy.Transforms;
// </Snippet1>
// <Snippet9>
using System.Threading.Tasks;
// </Snippet9>

namespace TaxiFarePrediction
{
    class Program
    {
        // <Snippet2>
        static readonly string _datapath = Path.Combine(Environment.CurrentDirectory, "Data", "taxi-fare-train.csv");
        static readonly string _testdatapath = Path.Combine(Environment.CurrentDirectory, "Data", "taxi-fare-test.csv");
        static readonly string _modelpath = Path.Combine(Environment.CurrentDirectory, "Data", "Model.zip");
        // </Snippet2>

        // <Snippet8>
        static async Task Main(string[] args)
        // </Snippet8>
        {
            Console.WriteLine(Environment.CurrentDirectory);
            // <Snippet7>
            PredictionModel<TaxiTrip, TaxiTripFarePrediction> model = await Train();
            // </Snippet7>

            // <Snippet10>
            Evaluate(model);
            // </Snippet10>

            // <Snippet16>
            TaxiTripFarePrediction prediction = model.Predict(TestTrips.Trip1);
            Console.WriteLine("Predicted fare: {0}, actual fare: 29.5", prediction.FareAmount);
            // </Snippet16>
        }

        // <Snippet6>
        public static async Task<PredictionModel<TaxiTrip, TaxiTripFarePrediction>> Train()
        // </Snippet6>
        {
            // <Snippet3>
            var pipeline = new LearningPipeline
            {
                new TextLoader(_datapath).CreateFrom<TaxiTrip>(useHeader: true, separator: ','),
                new ColumnCopier(("FareAmount", "Label")),
                new CategoricalOneHotVectorizer(
                    "VendorId",
                    "RateCode",
                    "PaymentType"),
                new ColumnConcatenator(
                    "Features",
                    "VendorId",
                    "RateCode",
                    "PassengerCount",
                    "TripDistance",
                    "PaymentType"),
                new FastTreeRegressor()
            };
            // </Snippet3>

            // <Snippet4>
            PredictionModel<TaxiTrip, TaxiTripFarePrediction> model = pipeline.Train<TaxiTrip, TaxiTripFarePrediction>();
            // </Snippet4>
            // <Snippet5>
            await model.WriteAsync(_modelpath);
            return model;
            // </Snippet5>
        }

        private static void Evaluate(PredictionModel<TaxiTrip, TaxiTripFarePrediction> model)
        {
            // <Snippet12>
            var testData = new TextLoader(_testdatapath).CreateFrom<TaxiTrip>(useHeader: true, separator: ',');
            // </Snippet12>

            // <Snippet13>
            var evaluator = new RegressionEvaluator();
            RegressionMetrics metrics = evaluator.Evaluate(model, testData);
            // </Snippet13>

            // <Snippet14>
            Console.WriteLine($"Rms = {metrics.Rms}");
            // </Snippet14>
            // <Snippet15>
            Console.WriteLine($"RSquared = {metrics.RSquared}");
            // </Snippet15>
        }
    }
}
