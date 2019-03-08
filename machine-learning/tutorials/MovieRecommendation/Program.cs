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
        public static (IDataView training, IDataView test) LoadData(MLContext mlcontext)
        {
            // Load training & test datasets using datapaths
            // <SnippetLoadData>
            var trainingDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "recommendation-ratings-train.csv");
            var testDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "recommendation-ratings-test.csv");

            IDataView trainingDataView = mlcontext.Data.LoadFromTextFile<MovieRating>(trainingDataPath, hasHeader: true, separatorChar: ',');
            IDataView testDataView = mlcontext.Data.LoadFromTextFile<MovieRating>(testDataPath, hasHeader: true, separatorChar: ',');

            return (trainingDataView, testDataView);
            // </SnippetLoadData>
        }

        // Build and train model
        public static ITransformer BuildAndTrainModel(MLContext mlcontext, IDataView trainingDataView)
        {
            // Add data transformations
            // <SnippetDataTransformations>
            IEstimator<ITransformer> est = mlcontext.Transforms.Conversion.MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName: "userId")
                .Append(mlcontext.Transforms.Conversion.MapValueToKey(outputColumnName: "movieIdEncoded", inputColumnName: "movieId"));
            // </SnippetDataTransformations>

            // Set algorithm options, add algorithm, and train model
            // <SnippetTrainModel>
            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "userIdEncoded",
                MatrixRowIndexColumnName = "movieIdEncoded", 
                LabelColumnName = "Label",
                NumberOfIterations = 20,
                ApproximationRank = 100
            };

            est = est.Append(mlcontext.Recommendation().Trainers.MatrixFactorization(options));

            Console.WriteLine("=============== Training the model ===============");
            ITransformer model = est.Fit(trainingDataView);

            return model;
            // </SnippetTrainModel>
        }

        // Evaluate model
        public static void EvaluateModel(MLContext mlcontext, IDataView testDataView, ITransformer model)
        {
            // Evaluate model on test data & print evaluation metrics
            // <SnippetEvaluateModel>
            Console.WriteLine("=============== Evaluating the model ===============");
            var prediction = model.Transform(testDataView);
            var metrics = mlcontext.Regression.Evaluate(prediction, label: DefaultColumnNames.Label, score: DefaultColumnNames.Score);

            Console.WriteLine("Rms: " + metrics.Rms.ToString());
            Console.WriteLine("RSquared: " + metrics.RSquared.ToString());
            // </SnippetEvaluateModel>
        }

        // Use model for single prediction
        public static void UseModelForSinglePrediction(MLContext mlcontext, ITransformer model)
        {
            // <SnippetPredictionEngine>
            Console.WriteLine("=============== Making a prediction ===============");
            var predictionengine = model.CreatePredictionEngine<MovieRating, MovieRatingPrediction>(mlcontext);
            // </SnippetPredictionEngine>

            // Create test input & make single prediction
            // <SnippetMakeSinglePrediction>
            var testInput = new MovieRating { userId = 6, movieId = 10 };

            var movieRatingPrediction = predictionengine.Predict(testInput);
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
        public static void SaveModel(MLContext mlcontext, ITransformer model)
        {
            // Save the trained model to .zip file
            // <SnippetSaveModel>
            using (var fs = new FileStream("moviePredictionModel.zip",
                FileMode.Create, FileAccess.Write, FileShare.Write))

                mlcontext.Model.Save(model, fs);
            // </SnippetSaveModel>
        }

    }

}
