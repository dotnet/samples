// <Snippet1>
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.ML.Runtime.Learners;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Core.Data;
using Microsoft.ML;

// </Snippet1>

namespace SentimentAnalysis
{
    class Program
    {
        // <Snippet2>
        static readonly string _dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "wikipedia-detox-250-line-data.tsv");
        static readonly string _testDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "wikipedia-detox-250-line-test.tsv");
        static readonly string _modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "Model.zip");
        static LocalEnvironment _mlContext;
        // </Snippet2>

        //static async Task Main(string[] args)
        static void Main(string[] args)
        {
            // <Snippet3>
             var model = Train();
            // </Snippet3>

            // <Snippet12>
            var sent = Evaluate(model);
            // </Snippet12>

            // <Snippet17>
            PredictWithModelLoadedFromFile(model, sent);
            // </Snippet17>
        }

        public static ITransformer Train()
        {
            // Create ML.NET context/local environment - allows you to add steps in order to keep everything together 
            // during the learning process.  
            // <Snippet5>
            _mlContext = new LocalEnvironment();
            // </Snippet5>

            // The TextLoader loads a dataset with comments and corresponding postive or negative sentiment. 
            // When you create a loader, you specify the schema by passing a class to the loader containing
            // all the column names and their types. This is used to create the model, and train it. 
            // <Snippet6>
            var reader = new TextLoader(_mlContext,
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

            IDataView trainingDataView = reader.Read(new MultiFileSource(_dataPath));
            // </Snippet6>

            // Create a flexible pipeline (composed by a chain of estimators) for creating/training the model.
            // This is used to format and clean the data.  

            // <Snippet7>
            // Convert the text column to numeric vectors (Features column) 
            var pipeline = new TextTransform(_mlContext, "SentimentText", "Features") 
            //</Snippet7>
            // Adds a LinearClassificationTrainer, the decision tree learner for this project            
            // <Snippet8>
                .Append(new LinearClassificationTrainer(_mlContext, new LinearClassificationTrainer.Arguments(),
                                                         "Features",
                                                         "Label"));                                                                            //</Snippet7>
                                                                                                                                               // </Snippet8>


            // <Snippet9>
            Console.WriteLine("=============== Create and Train the Model ===============");
            // Create and train the model based on the dataset that has been loaded, transformed.
            var model = pipeline.Fit(trainingDataView);

            Console.WriteLine("=============== End of training ===============");
            Console.WriteLine();
            // </Snippet9>

            // Returns the model we trained to use for evaluation.
            // <Snippet11>
            return model;
            // </Snippet11>
        }

        public static SentimentData Evaluate(ITransformer model)
        {
            // Evaluate the model and show accuracy stats
            // <Snippet13>
            var reader = new TextLoader(_mlContext,
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

            // Load evaluation/test data
            IDataView testDataView = reader.Read(new MultiFileSource(_testDataPath));
            // </Snippet13>

            // <Snippet14>
            Console.WriteLine("=============== Evaluating Model accuracy with Test data===============");
            var predictions = model.Transform(testDataView);
            // </Snippet14>

            // BinaryClassificationContext.Evaluate returns a BinaryClassificationEvaluator.CalibratedResult
            // that contains the computed overall metrics.
            // <Snippet15>
            var binClassificationCtx = new BinaryClassificationContext(_mlContext);
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

            var predictionFunct = model.MakePredictionFunction<SentimentData, SentimentPrediction>(_mlContext);

            SentimentData sampleStatement = new SentimentData
            {
                SentimentText = "This is a very rude movie"
            };

            var resultprediction = predictionFunct.Predict(sampleStatement);

            Console.WriteLine();
            Console.WriteLine("=============== Test of model with a sample ===============");

            Console.WriteLine($"Text: {sampleStatement.SentimentText} | Prediction: {(Convert.ToBoolean(resultprediction.Prediction) ? "Toxic" : "Nice")} sentiment | Probability: {resultprediction.Probability} ");

            // </Snippet16>
            return sampleStatement;
        }

        public static void PredictWithModelLoadedFromFile(ITransformer loadedModel, SentimentData sentimentData)
        {
            // Adds some comments to test the trained model's predictions.
            // <Snippet18>

            ///ISSUE: How can I load in multiple rows?
            IEnumerable<SentimentData> sentiments = new[]
            {
                new SentimentData
                {
                    SentimentText = "Please refrain from adding nonsense to Wikipedia."
                },
                new SentimentData
                {
                    SentimentText = "He is the best, and the article should say that."
                }
            };
            // </Snippet18>
            //ISSUE: Load model -  currently throws "Corrupt Model"

            //ITransformer loadedModel;
            //using (var stream = new FileStream(_modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            //{
            //    loadedModel = TransformerChain.LoadFrom(_mlContext, stream);
            //}

            // <Snippet19>
            // Create prediction engine
           // var predictions = loadedModel.Transform(sentimentData);
            var engine = loadedModel.MakePredictionFunction<SentimentData, SentimentPrediction>(_mlContext);

            // Use the model to predict whether comment data is toxic (1) or not (0).
            var predictionFromLoaded = engine.Predict(sentimentData);
            // </Snippet19>

            // <Snippet20>
            Console.WriteLine();
            Console.WriteLine("Sentiment Predictions - Loaded Model");
            Console.WriteLine("---------------------");
            // </Snippet20>

            // Builds pairs of (sentiment, prediction)
            // <Snippet21>
            //var sentimentsAndPredictions = sentiments.Zip(predictions, (sentiment, prediction) => (sentiment, prediction));
            Console.WriteLine($"Text: {sentimentData.SentimentText} | Prediction: {(Convert.ToBoolean(predictionFromLoaded.Prediction) ? "Toxic" : "Nice")} sentiment | Probability: {predictionFromLoaded.Probability} ");
            // </Snippet21>
            Console.WriteLine("=============== End of process, hit any key to finish ===============");
        }
        //    // <Snippet22>
        //    foreach (var item in sentimentsAndPredictions)
        //    {
        //        Console.WriteLine($"Sentiment: {item.sentiment.SentimentText} | Prediction: {(item.prediction.Sentiment ? "Negative" : "Positive")}");
        //    }
        //    Console.WriteLine();
        //    // </Snippet22>          
        //}

        // Saves the model we trained to a zip file.
        // <Snippet10>
        private static void SaveModelAsFile(LocalEnvironment _localEnvironment, TransformerChain<BinaryPredictionTransformer<Microsoft.ML.Runtime.Internal.Internallearn.IPredictorWithFeatureWeights<float>>> model)
        {
            using (var fs = new FileStream(_modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
                model.SaveTo(_localEnvironment, fs);

            Console.WriteLine("The model is saved to {0}", _modelPath);
        }
        // </Snippet10>
    }
}
