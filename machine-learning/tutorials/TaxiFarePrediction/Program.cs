using System;
using Microsoft.ML.Models;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using Microsoft.ML;
using System.Threading.Tasks;

namespace TaxiFarePrediction
{
    class Program
    {
        const string DataPath = @"..\..\..\Data\taxi-fare-train.csv";
        const string TestDataPath = @"..\..\..\Data\taxi-fare-test.csv";
        const string ModelPath = @"..\..\..\Models\Model.zip";

        static async Task Main(string[] args)
        {
            PredictionModel<TaxiTrip, TaxiTripFarePrediction> model = await Train();
            Evaluate(model);

            var prediction = model.Predict(TestTrips.Trip1);

            Console.WriteLine("Predicted fare: {0}, actual fare: 29.5", prediction.fare_amount);
        }

        public static async Task<PredictionModel<TaxiTrip, TaxiTripFarePrediction>> Train()
        {
            var pipeline = new LearningPipeline
            {
                new TextLoader<TaxiTrip>(DataPath, useHeader: true, separator: ","),
                new ColumnCopier(("fare_amount", "Label")),
                new CategoricalOneHotVectorizer("vendor_id",
                                             "rate_code",
                                             "payment_type"),
                new ColumnConcatenator("Features",
                                                "vendor_id",
                                                "rate_code",
                                                "passenger_count",
                                                "trip_distance",
                                                "payment_type"),
                new FastTreeRegressor()
            };
            PredictionModel<TaxiTrip, TaxiTripFarePrediction> model = pipeline.Train<TaxiTrip, TaxiTripFarePrediction>();
            await model.WriteAsync(ModelPath);
            return model;
        }

        private static void Evaluate(PredictionModel<TaxiTrip, TaxiTripFarePrediction> model)
        {
            var testData = new TextLoader<TaxiTrip>(TestDataPath, useHeader: true, separator: ",");
            var evaluator = new RegressionEvaluator();
            RegressionMetrics metrics = evaluator.Evaluate(model, testData);
            // Rms should be around 2.795276
            Console.WriteLine("Rms=" + metrics.Rms);
            Console.WriteLine("RSquared = " + metrics.RSquared);

        }
    }
}
