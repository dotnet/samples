// <SnippetAddUsings>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.DataView;
using Microsoft.ML;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.Online;
using Microsoft.ML.Transforms.Text;
// </SnippetAddUsings>

namespace SentimentAnalysis
{
    class Program
    {
        // <SnippetDeclareGlobalVariables>
        static readonly string _dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "yelp_labelled.txt");
        static readonly string _modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "Model.zip");
        // </SnippetDeclareGlobalVariables>

        static void Main(string[] args)
        {
            // Create ML.NET context/local environment - allows you to add steps in order to keep everything together 
            // during the learning process.  
            //Create ML Context with seed for repeatable/deterministic results
            // <SnippetCreateMLContext>
            MLContext mlContext = new MLContext();
            // </SnippetCreateMLContext>

            // <SnippetCallLoadData>
            (IDataView trainSet, IDataView testSet) splitDataView = LoadData(mlContext);
            // </SnippetCallLoadData>


            // <SnippetCallBuildAndTrainModel>
            ITransformer model = BuildAndTrainModel(mlContext, splitDataView.trainSet);
            // </SnippetCallBuildAndTrainModel>

            // <SnippetCallEvaluate>
            Evaluate(mlContext, model, splitDataView.testSet);
            // </SnippetCallEvaluate>

            // <SnippetCallUseModelWithSingleItem>
            UseModelWithSingleItem(mlContext, model);
            // </SnippetCallUseModelWithSingleItem>

            // <SnippetCallUseLoadedModelWithBatchItems>
            UseLoadedModelWithBatchItems(mlContext);
            // </SnippetCallUseLoadedModelWithBatchItems>

            Console.WriteLine();
            Console.WriteLine("=============== End of process ===============");
        }

        public static (IDataView trainSet, IDataView testSet) LoadData(MLContext mlContext)
        {

            //Note that this case, loading your training data from a file, 
            //is the easiest way to get started, but ML.NET also allows you 
            //to load data from databases or in-memory collections.
            // <SnippetLoadData>
            IDataView dataView = mlContext.Data.ReadFromTextFile<SentimentData>(_dataPath,hasHeader:false);
            // </SnippetLoadData>

            // <SnippetSplitData>
            (IDataView trainSet, IDataView testSet) splitDataView = mlContext.BinaryClassification.TrainTestSplit(dataView, testFraction: 0.2);
            // </SnippetSplitData>

            // <SnippetReturnSplitData>        
            return splitDataView;
            // </SnippetReturnSplitData>           
        }

        public static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        {

            // Create a flexible pipeline (composed by a chain of estimators) for creating/training the model.
            // This is used to format and clean the data.  
            // Convert the text column to numeric vectors (Features column) 
            // <SnippetFeaturizeText>
            var pipeline = mlContext.Transforms.Text.FeaturizeText(outputColumnName: DefaultColumnNames.Features, inputColumnName: nameof(SentimentData.SentimentText))
            //</SnippetFeaturizeText>
            // Adds a FastTreeBinaryClassificationTrainer, the decision tree learner for this project  
            // <SnippetAddTrainer> 
            .Append(mlContext.BinaryClassification.Trainers.FastTree(numLeaves: 50, numTrees: 50, minDatapointsInLeaves: 20));
            // </SnippetAddTrainer>

            // Create and train the model based on the dataset that has been loaded, transformed.
            // <SnippetTrainModel>
            Console.WriteLine("=============== Create and Train the Model ===============");
            var model = pipeline.Fit(splitTrainSet);
            Console.WriteLine("=============== End of training ===============");
            Console.WriteLine();
            // </SnippetTrainModel>

            // Returns the model we trained to use for evaluation.
            // <SnippetReturnModel>
            return model;
            // </SnippetReturnModel>
        }

        public static void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        {
            // Evaluate the model and show accuracy stats

            //Take the data in, make transformations, output the data. 
            // <SnippetTransformData>
            Console.WriteLine("=============== Evaluating Model accuracy with Test data===============");
            IDataView predictions = model.Transform(splitTestSet);
            // </SnippetTransformData>

            // BinaryClassificationContext.Evaluate returns a BinaryClassificationEvaluator.CalibratedResult
            // that contains the computed overall metrics.
            // <SnippetEvaluate>
            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
            // </SnippetEvaluate>

            // The Accuracy metric gets the accuracy of a classifier, which is the proportion 
            // of correct predictions in the test set.

            // The Auc metric gets the area under the ROC curve.
            // The area under the ROC curve is equal to the probability that the classifier ranks
            // a randomly chosen positive instance higher than a randomly chosen negative one
            // (assuming 'positive' ranks higher than 'negative').

            // The F1Score metric gets the classifier's F1 score.
            // The F1 score is the harmonic mean of precision and recall:
            //  2 * precision * recall / (precision + recall).

            // <SnippetDisplayMetrics>
            Console.WriteLine();
            Console.WriteLine("Model quality metrics evaluation");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.Auc:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            Console.WriteLine("=============== End of model evaluation ===============");
            //</SnippetDisplayMetrics>

            // Save the new model to .ZIP file
            // <SnippetCallSaveModel>
            SaveModelAsFile(mlContext, model);
            // </SnippetCallSaveModel>
        }

        private static void UseModelWithSingleItem(MLContext mlContext, ITransformer model)
        {
            // <SnippetCreatePredictionEngine1>
            PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = model.CreatePredictionEngine<SentimentData, SentimentPrediction>(mlContext);
            // </SnippetCreatePredictionEngine1>

            // <SnippetCreateTestIssue1>
            SentimentData sampleStatement = new SentimentData
            {
                SentimentText = "This was a very bad steak"
            };
            // </SnippetCreateTestIssue1>

            // <SnippetPredict>
            var resultprediction = predictionFunction.Predict(sampleStatement);
            // </SnippetPredict>
            // <SnippetOutputPrediction>
            Console.WriteLine();
            Console.WriteLine("=============== Prediction Test of model with a single sample and test dataset ===============");

            Console.WriteLine();
            Console.WriteLine($"Sentiment: {sampleStatement.SentimentText} | Prediction: {(Convert.ToBoolean(resultprediction.Prediction) ? "Positive" : "Negative")} | Probability: {resultprediction.Probability} ");

            Console.WriteLine("=============== End of Predictions ===============");
            Console.WriteLine();
            // </SnippetOutputPrediction>
        }

        public static void UseLoadedModelWithBatchItems(MLContext mlContext)
        {
            // Adds some comments to test the trained model's predictions.
            // <SnippetCreateTestIssues>
            IEnumerable<SentimentData> sentiments = new[]
            {
                new SentimentData
                {
                    SentimentText = "This was a horrible meal"
                },
                new SentimentData
                {
                    SentimentText = "I love this spaghetti."
                }
            };
            // </SnippetCreateTestIssues>

            // <SnippetLoadModel>
            ITransformer loadedModel;
            using (var stream = new FileStream(_modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                loadedModel = mlContext.Model.Load(stream);
            }
            // </SnippetLoadModel>

            // Load test data  
            // <SnippetPrediction>
            IDataView sentimentStreamingDataView = mlContext.Data.ReadFromEnumerable(sentiments);

            IDataView predictions = loadedModel.Transform(sentimentStreamingDataView);

            // Use model to predict whether comment data is Positive (1) or Negative (0).
            IEnumerable<SentimentPrediction> predictedResults = mlContext.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);
            // </SnippetPrediction>

            // <SnippetAddInfoMessage>
            Console.WriteLine();

            Console.WriteLine("=============== Prediction Test of loaded model with a multiple samples ===============");
            // </SnippetAddInfoMessage>

            Console.WriteLine();

            // Builds pairs of (sentiment, prediction)
            // <SnippetBuildSentimentPredictionPairs>
            IEnumerable<(SentimentData sentiment, SentimentPrediction prediction)> sentimentsAndPredictions = sentiments.Zip(predictedResults, (sentiment, prediction) => (sentiment, prediction));
            // </SnippetBuildSentimentPredictionPairs>

            // <SnippetDisplayResults>
            foreach ((SentimentData sentiment, SentimentPrediction prediction) item in sentimentsAndPredictions)
            {
                Console.WriteLine($"Sentiment: {item.sentiment.SentimentText} | Prediction: {(Convert.ToBoolean(item.prediction.Prediction) ? "Positive" : "Negative")} | Probability: {item.prediction.Probability} ");

            }
            Console.WriteLine("=============== End of predictions ===============");

            // </SnippetDisplayResults>          
        }

        // Saves the model we trained to a zip file.

        private static void SaveModelAsFile(MLContext mlContext, ITransformer model)
        {
            // <SnippetSaveModel> 
            using (var fs = new FileStream(_modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
                mlContext.Model.Save(model, fs);
            // </SnippetSaveModel>

            Console.WriteLine("The model is saved to {0}", _modelPath);
        }

    }
}
