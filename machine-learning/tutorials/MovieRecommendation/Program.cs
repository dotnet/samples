// <SnippetUsingStatements>
using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using Microsoft.ML.Data;
using Microsoft.Data.DataView;
// </SnippetUsingStatements>

namespace MovieRecommendation
{

    class Program
    {
        static void Main(string[] args)
        {

            // Create MLContext to be shared across the model creation workflow objects 
            // <SnippetMLContext>
            MLContext mlContext = new MLContext();
            // </SnippetMLContext>

            // Load data
            // <SnippetLoadDataMain>
            IDataView trainingDataView = LoadData(mlContext).training;
            IDataView testDataView = LoadData(mlContext).test;
            // </SnippetLoadDataMain>

            // Build & train model
            // <SnippetBuildTrainModelMain>
            ITransformer model = BuildAndTrainModel(mlContext, trainingDataView);
            // </SnippetBuildTrainModelMain>

            // Evaluate quality of model
            // <SnippetEvaluateModelMain>
            EvaluateModel(mlContext, testDataView, model);
            // </SnippetEvaluateModelMain>

            // Use model to try a single prediction (one row of data)
            // <SnippetUseModelMain>
            UseModelForSinglePrediction(mlContext, model);
            // </SnippetUseModelMain>

            // Save model
            // <SnippetSaveModelMain>
            SaveModel(mlContext, model);
            // </SnippetSaveModelMain>
        }

        // Load data
        public static (IDataView training, IDataView test) LoadData(MLContext mlContext)
        {
            // Load training & test datasets using datapaths
            // <SnippetLoadData>
            var trainingDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "recommendation-ratings-train.csv");
            var testDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "recommendation-ratings-test.csv");

            IDataView trainingDataView = mlContext.Data.LoadFromTextFile<MovieRating>(trainingDataPath, hasHeader: true, separatorChar: ',');
            IDataView testDataView = mlContext.Data.LoadFromTextFile<MovieRating>(testDataPath, hasHeader: true, separatorChar: ',');

            return (trainingDataView, testDataView);
            // </SnippetLoadData>
        }

        // Build and train model
        public static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainingDataView)
        {
            // Add data transformations
            // <SnippetDataTransformations>
            IEstimator<ITransformer> estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName: "userId")
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "movieIdEncoded", inputColumnName: "movieId"));
            // </SnippetDataTransformations>

            // Set algorithm options and append algorithm
            // <SnippetAddAlgorithm>
            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "userIdEncoded",
                MatrixRowIndexColumnName = "movieIdEncoded", 
                LabelColumnName = "Label",
                NumberOfIterations = 20,
                ApproximationRank = 100
            };
            
            var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));
            // </SnippetAddAlgorithm>

            // <SnippetFitModel>
            Console.WriteLine("=============== Training the model ===============");
            ITransformer model = trainerEstimator.Fit(trainingDataView);

            return model;
            // </SnippetFitModel>
        }

        // Evaluate model
        public static void EvaluateModel(MLContext mlContext, IDataView testDataView, ITransformer model)
        {
            // Evaluate model on test data & print evaluation metrics
            // <SnippetTransform>
            Console.WriteLine("=============== Evaluating the model ===============");
            var prediction = model.Transform(testDataView);
            // </SnippetTransform>

            // <SnippetEvaluate>
            var metrics = mlContext.Regression.Evaluate(prediction, label: DefaultColumnNames.Label, score: DefaultColumnNames.Score);
            // </SnippetEvaluate>

            // <SnippetPrintMetrics>
            Console.WriteLine("Rms: " + metrics.Rms.ToString());
            Console.WriteLine("RSquared: " + metrics.RSquared.ToString());
            // </SnippetPrintMetrics>
        }

        // Use model for single prediction
        public static void UseModelForSinglePrediction(MLContext mlContext, ITransformer model)
        {
            // <SnippetPredictionEngine>
            Console.WriteLine("=============== Making a prediction ===============");
            var predictionEngine = model.CreatePredictionEngine<MovieRating, MovieRatingPrediction>(mlContext);
            // </SnippetPredictionEngine>

            // Create test input & make single prediction
            // <SnippetMakeSinglePrediction>
            var testInput = new MovieRating { userId = 6, movieId = 10 };

            var movieRatingPrediction = predictionEngine.Predict(testInput);
            // </SnippetMakeSinglePrediction>

            // <SnippetPrintResults>
            if (Math.Round(movieRatingPrediction.Score, 1) > 3.5)
            {
                Console.WriteLine("Movie " + testInput.movieId + " is recommended for user " + testInput.userId);
            }
            else
            {
                Console.WriteLine("Movie " + testInput.movieId + " is not recommended for user " + testInput.userId);
            }
            // </SnippetPrintResults>
        }

        //Save model
        public static void SaveModel(MLContext mlContext, ITransformer model)
        {
            // Save the trained model to .zip file
            // <SnippetSaveModel>
            using (var fs = new FileStream("moviePredictionModel.zip",
                FileMode.Create, FileAccess.Write, FileShare.Write))

                mlContext.Model.Save(model, fs);
            // </SnippetSaveModel>
        }

    }

}
