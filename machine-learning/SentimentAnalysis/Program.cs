using System;
using Microsoft.ML.Models;
using Microsoft.ML.Runtime;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;

namespace SentimentAnalysis
{
    class Program
    {
        const string _dataPath = @"..\..\data\sentiment labelled sentences\imdb_labelled.txt";
        const string _testDataPath = @"..\..\data\sentiment labelled sentences\yelp_labelled.txt";
        static void Main(string[] args)
        {
            var model = TrainAndPredict();
            Evaluate(model);
        }
        public static PredictionModel<SentimentData, SentimentPrediction> TrainAndPredict()
        {
            // LearningPipeline allows us to add steps in order to keep everything together 
            // during the learning process.  
            var pipeline = new LearningPipeline();

            // The TextLoader loads a dataset with comments and corresponding postive or negative sentiment. 
            // When you create a loader you specify the schema by passing a class to the loader containing
            // all the column names and their types. This will be used to create the model, and train it. 
            pipeline.Add(new TextLoader<SentimentData>(_dataPath, useHeader: false, separator: "tab"));

            // TextFeaturizer is a transform that will be used to featurize an input column. 
            // This is used to format and clean the data.  
            pipeline.Add(new TextFeaturizer("Features", "SentimentText"));

            //add a FastTreeBinaryClassifier, the decision tree learner for this project, and 
            //three hyperparameters to be used for tuning decision tree performance 
            pipeline.Add(new FastTreeBinaryClassifier() { NumLeaves = 5, NumTrees = 5, MinDocumentsInLeafs = 2 });

            // We train our pipeline based on the dataset that has been loaded, transformed 
            PredictionModel<SentimentData, SentimentPrediction> model = pipeline.Train<SentimentData, SentimentPrediction>();

            //add some comments to test the trained model's predictions
            IEnumerable<SentimentData> sentiments = new[]
             {
                new SentimentData
                {
                    SentimentText = "Contoso's 11 is a wonderful experience",
                    Sentiment = 0
                },
                new SentimentData
                {
                    SentimentText = "The acting in this movie is very bad",
                    Sentiment = 0
                },
                new SentimentData
                {
                    SentimentText = "Joe versus the Volcano Coffee Company is a great film.",
                    Sentiment = 0
                }
            };

            // Now that we have a model, use that to predict the positive 
            // or negative sentiment of the comment data.
            IEnumerable<SentimentPrediction> predictions = model.Predict(sentiments);

            Console.WriteLine();
            Console.WriteLine("Sentiment Predictions");
            Console.WriteLine("---------------------");

            // Build pairs of (sentiment, prediction)
            var sentimentsAndPredictions = sentiments.Zip(predictions, (sentiment, prediction) => new { sentiment, prediction });

            foreach (var item in sentimentsAndPredictions)
            {
                Console.WriteLine($"Sentiment: {item.sentiment.SentimentText} | Prediction: {(item.prediction.Sentiment ? "Positive" : "Negative")}");
            }
            Console.WriteLine();

            //return the model we trained to use for evaluation
            return model;
        }
        public static void Evaluate(PredictionModel<SentimentData, SentimentPrediction> model)
        {
            // Evaluate
            var testData = new TextLoader<SentimentData>(_testDataPath, useHeader: false, separator: "tab");

            // BinaryClassificationEvaluator computes the quality metrics for the PredictionModel
            //using the specified data set.
            var evaluator = new BinaryClassificationEvaluator();

            // BinaryClassificationMetrics contains the overall metrics computed by binary classification evaluators
            BinaryClassificationMetrics metrics = evaluator.Evaluate(model, testData);
            Console.WriteLine();
            Console.WriteLine("PredictionModel quality metrics evaluation");
            Console.WriteLine("------------------------------------------");

            // This metric gets the accuracy of a classifier which is the proportion 
            //of correct predictions in the test set.
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");

            // This metric gets the area under the ROC curve.
            // The area under the ROC curve is equal to the probability that the classifier ranks
            // a randomly chosen positive instance higher than a randomly chosen negative one
            // (assuming 'positive' ranks higher than 'negative').
            Console.WriteLine($"Auc: {metrics.Auc:P2}");

            // This metric gets the classifier's F1 score.
            // The F1 score is the harmonic mean of precision and recall:
            //  2 * precision * recall / (precision + recall).
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
        }
    }
}
