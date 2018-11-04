// <Snippet1>
using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Transforms;
using Microsoft.ML.Transforms.Categorical;
using Microsoft.ML.Transforms.Normalizers;
using Microsoft.ML.Transforms.Text;
// </Snippet1>

namespace TaxiFarePrediction
{
    class Program
    {
        // <Snippet2>
        static readonly string _trainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "taxi-fare-train.csv");
        static readonly string _testDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "taxi-fare-test.csv");
        static readonly string _modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "Model.zip");
        static TextLoader _textLoader;
        // </Snippet2>

        // <Snippet8>
        static void Main(string[] args)
        // </Snippet8>
        {
            Console.WriteLine(Environment.CurrentDirectory);

            MLContext mlContext = new MLContext(seed: 0);

            _textLoader = mlContext.Data.TextReader(new TextLoader.Arguments()
            {
                Separator = ",",
                HasHeader = true,
                Column = new[]
                            {
                                new TextLoader.Column("VendorId", DataKind.Text, 0),
                                new TextLoader.Column("RateCode", DataKind.Text, 1),
                                new TextLoader.Column("PassengerCount", DataKind.R4, 2),
                                new TextLoader.Column("TripTime", DataKind.R4, 3),
                                new TextLoader.Column("TripDistance", DataKind.R4, 4),
                                new TextLoader.Column("PaymentType", DataKind.Text, 5),
                                new TextLoader.Column("FareAmount", DataKind.R4, 6)
                            }
            }
            );

            // <Snippet7>
            var model = Train(mlContext, _trainDataPath);
            // </Snippet7>

            // <Snippet10>
            Evaluate(mlContext, model);
            // </Snippet10>

            // <Snippet16>
            TestSinglePrediction(mlContext);
            // </Snippet16>
        }

        // <Snippet6>
        public static ITransformer Train(MLContext mlContext, string dataPath)
        // </Snippet6>
        {
            // <Snippet3>
            IDataView dataView = _textLoader.Read(new MultiFileSource(dataPath));

            var pipeline = new CopyColumnsEstimator(mlContext, "FareAmount", "Label")
                    .Append(new OneHotEncodingEstimator(mlContext, "VendorId"))
                    .Append(new OneHotEncodingEstimator(mlContext, "RateCode"))
                    .Append(new OneHotEncodingEstimator(mlContext, "PaymentType"))
                    .Append(new NormalizingEstimator(mlContext, "PassengerCount", mode: NormalizingEstimator.NormalizerMode.MeanVariance))
                    .Append(new NormalizingEstimator(mlContext, "TripTime", mode: NormalizingEstimator.NormalizerMode.MeanVariance))
                    .Append(new NormalizingEstimator(mlContext, "TripDistance", mode: NormalizingEstimator.NormalizerMode.MeanVariance))
                    .Append(new ColumnConcatenatingEstimator(mlContext, "Features", "VendorId", "RateCode", "PassengerCount", "TripTime", "TripDistance", "PaymentType"))
                    .Append(mlContext.Regression.Trainers.StochasticDualCoordinateAscent("Label", "Features"));
            // </Snippet3>

            Console.WriteLine("=============== Create and Train the Model ===============");
            
            // <Snippet4>
            var model = pipeline.Fit(dataView);
            // </Snippet4>
            Console.WriteLine("=============== End of training ===============");
            Console.WriteLine();
            // <Snippet5>
            SaveModelAsFile(mlContext, model);
            return model;
            // </Snippet5>
        }

        private static void Evaluate(MLContext mlContext, ITransformer model)
        {
            // <Snippet12>
            IDataView dataView = _textLoader.Read(new MultiFileSource(_testDataPath));
            // </Snippet12>

            // <Snippet13>
            var predictions = model.Transform(dataView);
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");
            // </Snippet13>

            // <Snippet14>
            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics evaluation         ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine($"*       R2 Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       RMS loss:      {metrics.Rms:#.##}");
            Console.WriteLine($"*       Absolute loss: {metrics.L1:#.##}");
            Console.WriteLine($"*       Squared loss:  {metrics.L2:#.##}");
            Console.WriteLine($"*************************************************");
            // </Snippet14>
        }

        private static void TestSinglePrediction(MLContext mlContext)
        {
            ITransformer loadedModel;
            using (var stream = new FileStream(_modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                loadedModel = TransformerChain.LoadFrom(mlContext, stream);
            }

            //Prediction test
            // Create prediction engine and make prediction.
            var engine = loadedModel.MakePredictionFunction<TaxiTrip, TaxiTripFarePrediction>(mlContext);

            //Sample: 
            //vendor_id,rate_code,passenger_count,trip_time_in_secs,trip_distance,payment_type,fare_amount
            //VTS,1,1,1140,3.75,CRD,15.5

            var taxiTripSample = new TaxiTrip()
            {
                VendorId = "VTS",
                RateCode = "1",
                PassengerCount = 1,
                TripTime = 1140,
                TripDistance = 3.75f,
                PaymentType = "CRD",
                FareAmount = 0 // To predict. Actual/Observed = 15.5
            };

            var prediction = engine.Predict(taxiTripSample);
            Console.WriteLine($"**********************************************************************");
            Console.WriteLine($"Predicted fare: {prediction.FareAmount:0.####}, actual fare: 29.5");
            Console.WriteLine($"**********************************************************************");
        }

        private static void SaveModelAsFile(MLContext mlContext, ITransformer model)
        {
            // <Snippet24> 
            using (var fs = new FileStream(_modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
                model.SaveTo(mlContext, fs);
            // </Snippet24>

            Console.WriteLine("The model is saved to {0}", _modelPath);
        }
    }
}
