// <Snippet1>
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.ML.Runtime.Learners;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Core.Data;
using Microsoft.ML;
using System.Linq;
// </Snippet1>

namespace SentimentAnalysis
{
    class Program
    {
        // <Snippet2>
        static readonly string _trainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "wikipedia-detox-250-line-data.tsv");
        static readonly string _testDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "wikipedia-detox-250-line-test.tsv");
        static readonly string _allDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "wikipedia-detox-250-all.tsv");
        static readonly string _modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "Model.zip");
        static TextLoader _reader;
        // </Snippet2>

        static void Main(string[] args)
        {
            // Create ML.NET context/local environment - allows you to add steps in order to keep everything together 
            // during the learning process.  
            //Create ML Context with seed for repeatable/deterministic results
            // <Snippet3>
            LocalEnvironment mlContext = new LocalEnvironment(seed: 0);
            // </Snippet3>
            // The TextLoader loads a dataset with comments and corresponding postive or negative sentiment. 
            // When you create a loader, you specify the schema by passing a class to the loader containing
            // all the column names and their types. This is used to create the model, and train it. 
            // Initialize our TextLoader
            // <Snippet4>
            _reader = new TextLoader(mlContext,
                            new TextLoader.Arguments()
                            {
                                Separator = "tab",
                                HasHeader = true,
                                Column = new[]
                                {
                                                    new TextLoader.Column("Label", DataKind.Bool, 0),
                                                    new TextLoader.Column("SentimentText", DataKind.Text, 1)
                                }
                            });
            // </Snippet4>
            // <Snippet5>
            var model = Train(mlContext, _trainDataPath);
            // </Snippet5>

            // <Snippet12>
               Evaluate(mlContext, model);
            // </Snippet12>

            Predict(mlContext, model);

            IterateModel(mlContext);

            // <Snippet17>
            PredictWithModelLoadedFromFile(mlContext);
            // </Snippet17>
            Console.WriteLine();
            Console.WriteLine("=============== End of process ===============");
        }

        public static ITransformer Train(LocalEnvironment mlContext, string dataPath)
        {

            // <Snippet5>

            // </Snippet5>
            // <Snippet6>

            IDataView dataView =_reader.Read(new MultiFileSource(dataPath));
            // </Snippet6>

            // Create a flexible pipeline (composed by a chain of estimators) for creating/training the model.
            // This is used to format and clean the data.  

            // <Snippet7>
            // Convert the text column to numeric vectors (Features column) 
            var pipeline = new TextTransform(mlContext, "SentimentText", "Features") 
            //</Snippet7>
            //</Snippet7>
            // Adds a LinearClassificationTrainer, the decision tree learner for this project            
            // <Snippet8>

                .Append(new LinearClassificationTrainer(mlContext, new LinearClassificationTrainer.Arguments(),
                                                         "Features",
                                                         "Label"));                                                                          
            // </Snippet8>


            // <Snippet9>
            Console.WriteLine("=============== Create and Train the Model ===============");
            // Create and train the model based on the dataset that has been loaded, transformed.
            var model = pipeline.Fit(dataView);

            Console.WriteLine("=============== End of training ===============");
            Console.WriteLine();

            // </Snippet9>
            // Returns the model we trained to use for evaluation.
            // <Snippet11>
            return model;
            // </Snippet11>
        }

        public static void Evaluate(LocalEnvironment mlContext, ITransformer model)
        {
            // Evaluate the model and show accuracy stats
            // <Snippet13>

            // Load evaluation/test data
            IDataView dataView = _reader.Read(new MultiFileSource(_testDataPath));
            // </Snippet13>

            // <Snippet14>
            Console.WriteLine("=============== Evaluating Model accuracy with Test data===============");
            var predictions = model.Transform(dataView);
            // </Snippet14>

            // BinaryClassificationContext.Evaluate returns a BinaryClassificationEvaluator.CalibratedResult
            // that contains the computed overall metrics.
            // <Snippet15>
            var binClassificationCtx = new BinaryClassificationContext(mlContext);
            var metrics = binClassificationCtx.Evaluate(predictions, "Label");
            // </Snippet15>

            // The Accuracy metric gets the accuracy of a classifier, which is the proportion 
            // of correct predictions in the test set.

            // The Auc metric gets the area under the ROC curve.
            // The area under the ROC curve is equal to the probability that the classifier ranks
            // a randomly chosen positive instance higher than a randomly chosen negative one
            // (assuming 'positive' ranks higher than 'negative').

            // The F1Score metric gets the classifier's F1 score.
            // The F1 score is the harmonic mean of precision and recall:
            //  2 * precision * recall / (precision + recall).

            // <Snippet16>
            Console.WriteLine();
            Console.WriteLine("Model quality metrics evaluation");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.Auc:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            Console.WriteLine("=============== End of model evaluation ===============");
        }

        private static void Predict(LocalEnvironment mlContext, ITransformer model)
        {
            var predictionFunction = model.MakePredictionFunction<SentimentData, SentimentPrediction>(mlContext);

            SentimentData sampleStatement = new SentimentData
            {
                SentimentText = "This is a very rude movie"
            };
            var resultprediction = predictionFunction.Predict(sampleStatement);

            Console.WriteLine();
            Console.WriteLine("=============== Prediction Test of model with a single sample and test dataset ===============");

            Console.WriteLine();
            Console.WriteLine($"Sentiment: {sampleStatement.SentimentText} | Prediction: {(Convert.ToBoolean(resultprediction.Prediction) ? "Toxic" : "Not Toxic")} sentiment | Probability: {resultprediction.Probability} ");
            Console.WriteLine("=============== End of Predictions ===============");
            Console.WriteLine();
        }

        private static void IterateModel(LocalEnvironment mlContext)
        {
            // <Snippet9>
            Console.WriteLine("=============== New iteration of Model ===============");
            
            // Create and train the model based on the dataset with combined training and test data.
            var newModel = Train(mlContext, _allDataPath);
            Console.WriteLine();

            // Save the new model to .ZIP file
            SaveModelAsFile(mlContext, newModel);
            // <Snippet11>

            // </Snippet16>
        }

        public static void PredictWithModelLoadedFromFile(LocalEnvironment mlContext)

        {
            // Adds some comments to test the trained model's predictions.
            // <Snippet18>

            IEnumerable<SentimentData> sentiments = new[]
            {
                new SentimentData
                {
                    SentimentText = "This is a very rude movie"
                },
                new SentimentData
                {
                    SentimentText = "I love this article"
                }
            };
            // </Snippet18>

            ITransformer loadedModel;
            using (var stream = new FileStream(_modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                loadedModel = TransformerChain.LoadFrom(mlContext, stream);
            }

            // <Snippet19>
            // Create prediction engine
            var sentimentStreamingDataView = mlContext.CreateStreamingDataView(sentiments);
            var predictions = loadedModel.Transform(sentimentStreamingDataView);
            
            // Use the model to predict whether comment data is toxic (1) or nice (0).
            var predictedResults = predictions.AsEnumerable<SentimentPrediction>(mlContext, reuseRowObject: false);
            // </Snippet19>

            // <Snippet20>
            Console.WriteLine();

            Console.WriteLine("=============== Prediction Test of loaded model with a multiple samples ===============");
            // </Snippet20>
            Console.WriteLine();
            // Builds pairs of (sentiment, prediction)
            // <Snippet21>
            var sentimentsAndPredictions = sentiments.Zip(predictedResults, (sentiment, prediction) => (sentiment, prediction));
            // </Snippet21>
            //}
            // <Snippet22>
            foreach (var item in sentimentsAndPredictions)
            {
                Console.WriteLine($"Sentiment: {item.sentiment.SentimentText} | Prediction: {(Convert.ToBoolean(item.prediction.Prediction) ? "Toxic" : "Nice")} | Probability: {item.prediction.Probability} ");
            }
            Console.WriteLine("=============== End of predictions ===============");

            // </Snippet22>          
        }

        // Saves the model we trained to a zip file.
        // <Snippet10>
        private static void SaveModelAsFile(LocalEnvironment mlContext, ITransformer model)
        {
            using (var fs = new FileStream(_modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
                model.SaveTo(mlContext, fs);

            Console.WriteLine("The model is saved to {0}", _modelPath);
        }
        // </Snippet10>
    }
}
