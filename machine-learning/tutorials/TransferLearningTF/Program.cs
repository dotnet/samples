// <SnippetAddUsings>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.DataView;
using Microsoft.ML;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Data;
using Microsoft.ML.ImageAnalytics;
using Microsoft.ML.Trainers;
// </SnippetAddUsings>

namespace TransferLearningTF
{
    class Program
    {
        // <SnippetDeclareGlobalVariables>
        static readonly string _assetsPath = Path.Combine(Environment.CurrentDirectory, "assets");
        static readonly string _trainTagsTsv = Path.Combine(_assetsPath, "inputs-train", "data", "tags.tsv");
        static readonly string _predictTagsTsv = Path.Combine(_assetsPath, "inputs-predict", "data", "tags.tsv");
        static readonly string _trainImagesFolder = Path.Combine(_assetsPath, "inputs-train", "data");
        static readonly string _predictImagesFolder = Path.Combine(_assetsPath, "inputs-predict", "data");
        static readonly string _inceptionPb = Path.Combine(_assetsPath, "inputs-train", "inception", "tensorflow_inception_graph.pb");
        static readonly string _inputImageClassifierZip = Path.Combine(_assetsPath, "inputs-predict", "imageClassifier.zip");
        static readonly string _outputImageClassifierZip = Path.Combine(_assetsPath, "outputs", "imageClassifier.zip");
        private static string LabelTokey = nameof(LabelTokey);
        private static string ImageReal = nameof(ImageReal);
        private static string PredictedLabelValue = nameof(PredictedLabelValue);
        // </SnippetDeclareGlobalVariables>

        static void Main(string[] args)
        {
            // Create MLContext to be shared across the model creation workflow objects 
            // <SnippetCreateMLContext>
            MLContext mlContext = new MLContext(seed:1);
            // </SnippetCreateMLContext>

            // <SnippetCallBuildAndTrain>
            BuildAndTrainModel(mlContext, _trainTagsTsv, _trainImagesFolder, _inceptionPb, _outputImageClassifierZip);
            // </SnippetCallBuildAndTrain>

            // <SnippetCallClassifyImages>
            ClassifyImages(mlContext, _predictTagsTsv, _predictImagesFolder, _outputImageClassifierZip);
            // </SnippetCallClassifyImages>
        }

        // <SnippetImageNetSettings>
        private struct ImageNetSettings
        {
            public const int ImageHeight = 224;
            public const int ImageWidth = 224;
            public const float Mean = 117;
            public const float Scale = 1;
            public const bool ChannelsLast = true;
        }
        // </SnippetImageNetSettings>

        // Build and train model
        public static void BuildAndTrainModel(MLContext mlContext, string dataLocation, string imagesFolder, string inputModelLocation, string outputModelLocation)
        {

            Console.WriteLine("Read model");
            Console.WriteLine($"Model location: {inputModelLocation}");
            Console.WriteLine($"Images folder: {_trainImagesFolder}");
            Console.WriteLine($"Training file: {dataLocation}");
            Console.WriteLine($"Default parameters: image size=({ImageNetSettings.ImageWidth},{ImageNetSettings.ImageHeight}), image mean: {ImageNetSettings.Mean}");

            // <SnippetLoadData>
            var data = mlContext.Data.ReadFromTextFile<ImageNetData>(path: dataLocation, hasHeader: true);
            // </SnippetLoadData>

            // <SnippetMapValueToKey1>
            var estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: LabelTokey, inputColumnName: DefaultColumnNames.Label)
                            // </SnippetMapValueToKey1>
                            // <SnippetImageTransforms>
                            .Append(mlContext.Transforms.LoadImages(_trainImagesFolder, (ImageReal, nameof(ImageNetData.ImagePath))))
                            .Append(mlContext.Transforms.Resize(outputColumnName: ImageReal, imageWidth: ImageNetSettings.ImageWidth, imageHeight: ImageNetSettings.ImageHeight, inputColumnName: ImageReal))
                            .Append(mlContext.Transforms.ExtractPixels(new ImagePixelExtractorTransformer.ColumnInfo(name: "input", inputColumnName: ImageReal, interleave: ImageNetSettings.ChannelsLast, offset: ImageNetSettings.Mean)))
                            // </SnippetImageTransforms>
                            // <SnippetScoreTensorFlowModel>
                            .Append(mlContext.Transforms.ScoreTensorFlowModel(modelLocation: inputModelLocation, outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }))
                            // </SnippetScoreTensorFlowModel>
                            // <SnippetAddTrainer> 
                            .Append(mlContext.MulticlassClassification.Trainers.LogisticRegression(labelColumn: LabelTokey, featureColumn: "softmax2_pre_activation"))
                            // </SnippetAddTrainer>
                            // <SnippetMapValueToKey2>
                            .Append(mlContext.Transforms.Conversion.MapKeyToValue((PredictedLabelValue, DefaultColumnNames.PredictedLabel)));
            // </SnippetMapValueToKey2>

            // Train the model
            Console.WriteLine("=============== Training classification model ===============");
            // Create and train the model based on the dataset that has been loaded, transformed.
            // <SnippetTrainModel>
            ITransformer model = estimator.Fit(data);
            // </SnippetTrainModel>

            // Process the training data through the model
            // This is an optional step, but it's useful for debugging issues
            // <SnippetTransformData>
            var predictions = model.Transform(data);
            // </SnippetTransformData>

            // <SnippetEnumerateDataViews>
            var imageNetData = mlContext.CreateEnumerable<ImageNetData>(data, false, true);
            var imageNetPredictionData = mlContext.CreateEnumerable<ImageNetPrediction>(predictions, false, true);
            // </SnippetEnumerateDataViews>

            // <SnippetCallPairAndDisplayResults1>
            PairAndDisplayResults(imageNetData, imageNetPredictionData);
            // </SnippetCallPairAndDisplayResults1>

            // Get some performance metrics on the model using training data
            Console.WriteLine("=============== Classification metrics ===============");

            // <SnippetEvaluate>           
            var regressionContext = new MulticlassClassificationCatalog(mlContext);
            var metrics = regressionContext.Evaluate(predictions, label: LabelTokey, predictedLabel: DefaultColumnNames.PredictedLabel);
            // </SnippetEvaluate>

            //<SnippetDisplayMetrics>
            Console.WriteLine($"LogLoss is: {metrics.LogLoss}");
            Console.WriteLine($"PerClassLogLoss is: {String.Join(" , ", metrics.PerClassLogLoss.Select(c => c.ToString()))}");
            //</SnippetDisplayMetrics>

            // Save the model to assets/outputs
            Console.WriteLine("=============== Save model to local file ===============");

            // <SnippetSaveModel>
            using (var fileStream = new FileStream(outputModelLocation, FileMode.Create))
                mlContext.Model.Save(model, fileStream);
            // </SnippetSaveModel>

            Console.WriteLine($"Model saved: {outputModelLocation}");
        }

        public static void ClassifyImages(MLContext mlContext, string dataLocation, string imagesFolder, string outputModelLocation)
        {
            Console.WriteLine($"=============== Loading model ===============");
            Console.WriteLine($"Model loaded: {outputModelLocation}");

            // Load the model
            // <SnippetLoadModel>
            ITransformer loadedModel;
            using (var fileStream = new FileStream(outputModelLocation, FileMode.Open))
                loadedModel = mlContext.Model.Load(fileStream);
            // </SnippetLoadModel>

            // <SnippetReadFromCSV> 
            var imageNetData = ImageNetData.ReadFromCsv(dataLocation, imagesFolder);
            var imageNetDataView = mlContext.Data.ReadFromEnumerable<ImageNetData>(imageNetData);
            // </SnippetReadFromCSV>  
            
            // <SnippetPredict>  
            var predictions = loadedModel.Transform(imageNetDataView);
            var imageNetPredictionData = mlContext.CreateEnumerable<ImageNetPrediction>(predictions, false,true);
            // </SnippetPredict> 

            Console.WriteLine("=============== Making classifications ===============");
            // <SnippetCallPairAndDisplayResults2>
            PairAndDisplayResults(imageNetData, imageNetPredictionData);
            // </SnippetCallPairAndDisplayResults2> 

        }

        private static void PairAndDisplayResults(IEnumerable<ImageNetData> imageNetData, IEnumerable<ImageNetPrediction> imageNetPredictionData)
        {
            // Builds pairs of (image, prediction)
            // <SnippetBuildImagePredictionPairs>
            IEnumerable<(ImageNetData image, ImageNetPrediction prediction)> imagesAndPredictions = imageNetData.Zip(imageNetPredictionData, (image, prediction) => (image, prediction));
            // </SnippetBuildImagePredictionPairs>

            // <SnippetDisplayPredictions>
            foreach ((ImageNetData image, ImageNetPrediction prediction) item in imagesAndPredictions)
            {
                Console.WriteLine($"Image: {Path.GetFileName(item.image.ImagePath)} predicted as: {item.prediction.PredictedLabelValue} with score: {item.prediction.Score.Max()} ");
            }
            // </SnippetDisplayPredictions>
        }
    }

}
