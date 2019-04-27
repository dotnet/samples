// <SnippetAddUsings>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Data.IO;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms.Image;
// </SnippetAddUsings>

namespace TransferLearningTF
{
    class Program
    {
        // <SnippetDeclareGlobalVariables>
        static readonly string _assetsPath = Path.Combine(Environment.CurrentDirectory, "assets");
        static readonly string _trainTagsTsv = Path.Combine(_assetsPath, "inputs-train", "data", "tags.tsv");
        static readonly string _predictImageListTsv = Path.Combine(_assetsPath, "inputs-predict", "data", "image_list.tsv");
        static readonly string _trainImagesFolder = Path.Combine(_assetsPath, "inputs-train", "data");
        static readonly string _predictImagesFolder = Path.Combine(_assetsPath, "inputs-predict", "data");
        static readonly string _predictSingleImage = Path.Combine(_assetsPath, "inputs-predict-single", "data", "toaster3.jpg");
        static readonly string _inceptionPb = Path.Combine(_assetsPath, "inputs-train", "inception", "tensorflow_inception_graph.pb");
        static readonly string _inputImageClassifierZip = Path.Combine(_assetsPath, "inputs-predict", "imageClassifier.zip");
        static readonly string _outputImageClassifierZip = Path.Combine(_assetsPath, "outputs", "imageClassifier.zip");
        private static string LabelTokey = nameof(LabelTokey);
        private static string PredictedLabelValue = nameof(PredictedLabelValue);
        // </SnippetDeclareGlobalVariables>

        static void Main(string[] args)
        {
            // Create MLContext to be shared across the model creation workflow objects 
            // <SnippetCreateMLContext>
            MLContext mlContext = new MLContext(seed: 1);
            // </SnippetCreateMLContext>

            // <SnippetCallReuseAndTuneInceptionModel>
            var model = ReuseAndTuneInceptionModel(mlContext, _trainTagsTsv, _trainImagesFolder, _inceptionPb, _outputImageClassifierZip);
            // </SnippetCallReuseAndTuneInceptionModel>

            // <SnippetCallClassifyImages>
            ClassifyImages(mlContext, _predictImageListTsv, _predictImagesFolder, _outputImageClassifierZip, model);
            // </SnippetCallClassifyImages>

            // <SnippetCallClassifySingleImage>
            ClassifySingleImage(mlContext, _predictSingleImage, _outputImageClassifierZip, model);
            // </SnippetCallClassifySingleImage>
        }

        // <SnippetInceptionSettings>
        private struct InceptionSettings
        {
            public const int ImageHeight = 224;
            public const int ImageWidth = 224;
            public const float Mean = 117;
            public const float Scale = 1;
            public const bool ChannelsLast = true;
        }
        // </SnippetInceptionSettings>

        // Build and train model
        public static ITransformer ReuseAndTuneInceptionModel(MLContext mlContext, string dataLocation, string imagesFolder, string inputModelLocation, string outputModelLocation)
        {

            // <SnippetLoadData>
            var data = mlContext.Data.LoadFromTextFile<ImageData>(path: dataLocation, hasHeader: false);
            // </SnippetLoadData>

            // <SnippetMapValueToKey1>
            var estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: LabelTokey, inputColumnName: "Label")
                            // </SnippetMapValueToKey1>
                            // The image transforms transform the images into the model's expected format.
                            // <SnippetImageTransforms>
                            .Append(mlContext.Transforms.LoadImages(outputColumnName: "input", imageFolder: _trainImagesFolder, inputColumnName: nameof(ImageData.ImagePath)))
                            .Append(mlContext.Transforms.ResizeImages(outputColumnName: "input", imageWidth: InceptionSettings.ImageWidth, imageHeight: InceptionSettings.ImageHeight, inputColumnName: "input"))
                            .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input", interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean))
                            // </SnippetImageTransforms>
                            // The ScoreTensorFlowModel transform scores the TensorFlow model and allows communication 
                            // <SnippetScoreTensorFlowModel>
                            .Append(mlContext.Model.LoadTensorFlowModel(inputModelLocation).
                                ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true))
                            // </SnippetScoreTensorFlowModel>
                            // <SnippetAddTrainer> 
                            .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: LabelTokey, featureColumnName: "softmax2_pre_activation"))
                            // </SnippetAddTrainer>
                            // <SnippetMapValueToKey2>
                            .Append(mlContext.Transforms.Conversion.MapKeyToValue(PredictedLabelValue, "PredictedLabel"))
                            .AppendCacheCheckpoint(mlContext);
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

            // Create enumerables for both the ImageData and ImagePrediction DataViews 
            // for displaying results
            // <SnippetEnumerateDataViews>
            var imageData = mlContext.Data.CreateEnumerable<ImageData>(data, false, true);
            var imagePredictionData = mlContext.Data.CreateEnumerable<ImagePrediction>(predictions, false, true);
            // </SnippetEnumerateDataViews>

            // <SnippetCallDisplayResults1>
            DisplayResults(imagePredictionData);
            // </SnippetCallDisplayResults1>

            // Get some performance metrics on the model using training data
            Console.WriteLine("=============== Classification metrics ===============");

            // <SnippetEvaluate>           
            var multiclassContext = mlContext.MulticlassClassification;
            var metrics = multiclassContext.Evaluate(predictions, labelColumnName: LabelTokey, predictedLabelColumnName: "PredictedLabel");
            // </SnippetEvaluate>

            //<SnippetDisplayMetrics>
            Console.WriteLine($"LogLoss is: {metrics.LogLoss}");
            Console.WriteLine($"PerClassLogLoss is: {String.Join(" , ", metrics.PerClassLogLoss.Select(c => c.ToString()))}");
            //</SnippetDisplayMetrics>

            // <SnippetReturnModel>
            return model;
            // </SnippetReturnModel>
        }

        public static void ClassifyImages(MLContext mlContext, string dataLocation, string imagesFolder, string outputModelLocation, ITransformer model)
        {

            // Read the image_list.tsv file and add the filepath to the image file name 
            // before loading into ImageData 
            // <SnippetReadFromTSV> 
            var imageData = ReadFromTsv(dataLocation, imagesFolder);
            var imageDataView = mlContext.Data.LoadFromEnumerable<ImageData>(imageData);
            // </SnippetReadFromTSV>  

            // <SnippetPredict>  
            var predictions = model.Transform(imageDataView);
            var imagePredictionData = mlContext.Data.CreateEnumerable<ImagePrediction>(predictions, false, true);
            // </SnippetPredict> 

            Console.WriteLine("=============== Making classifications ===============");

            // <SnippetCallDisplayResults2>
            DisplayResults(imagePredictionData);
            // </SnippetCallDisplayResults2> 
        }

        public static void ClassifySingleImage(MLContext mlContext, string imagePath, string outputModelLocation, ITransformer model)
        {
            // load the fully qualified image file name into ImageData 
            // <SnippetLoadImageData> 
            var imageData = new ImageData()
            {
                ImagePath = imagePath
            };
            // </SnippetLoadImageData>  

            // <SnippetPredictSingle>  
            // Make prediction function (input = ImageData, output = ImagePrediction)
            var predictor = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(model);
            var prediction = predictor.Predict(imageData);
            // </SnippetPredictSingle> 

            Console.WriteLine("=============== Making single image classification ===============");
            // <SnippetDisplayPrediction>
            Console.WriteLine($"Image: {Path.GetFileName(imageData.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
            // </SnippetDisplayPrediction> 

        }

        private static void DisplayResults(IEnumerable<ImagePrediction> imagePredictionData)
        {
            // <SnippetDisplayPredictions>
            foreach (ImagePrediction prediction in imagePredictionData)
            {
                Console.WriteLine($"Image: {Path.GetFileName(prediction.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
            }
            // </SnippetDisplayPredictions>
        }

        public static IEnumerable<ImageData> ReadFromTsv(string file, string folder)
        {
            //Need to parse through the tags.tsv file to combine the file path to the 
            // image name for the ImagePath property so that the image file can be found.

            // <SnippetReadFromTsv>
            return File.ReadAllLines(file)
             .Select(line => line.Split('\t'))
             .Select(line => new ImageData()
             {
                 ImagePath = Path.Combine(folder, line[0])
             });
            // </SnippetReadFromTsv>
        }
    }

}
