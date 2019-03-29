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
        static readonly string _predictImageListTsv = Path.Combine(_assetsPath, "inputs-predict", "data", "image_list.tsv");
        static readonly string _trainImagesFolder = Path.Combine(_assetsPath, "inputs-train", "data");
        static readonly string _predictImagesFolder = Path.Combine(_assetsPath, "inputs-predict", "data");
        static readonly string _predictSingleImage = Path.Combine(_assetsPath, "inputs-predict-single", "data", "toaster3.jpg");
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

            // <SnippetCallReuseAndTuneInceptionModel>
            ReuseAndTuneInceptionModel(mlContext, _trainTagsTsv, _trainImagesFolder, _inceptionPb, _outputImageClassifierZip);
            // </CallSnippetReuseAndTuneInceptionModel>

            // <SnippetCallClassifyImages>
            ClassifyImages(mlContext, _predictImageListTsv, _predictImagesFolder, _outputImageClassifierZip);
            // </SnippetCallClassifyImages>

            // <SnippetCallClassifySingleImage>
            ClassifySingleImage(mlContext, _predictSingleImage, _outputImageClassifierZip);
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
        public static void ReuseAndTuneInceptionModel(MLContext mlContext, string dataLocation, string imagesFolder, string inputModelLocation, string outputModelLocation)
        {

            // <SnippetLoadData>
            var data = mlContext.Data.ReadFromTextFile<ImageData>(path: dataLocation, hasHeader: false);
            // </SnippetLoadData>

            // <SnippetMapValueToKey1>
            var estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: LabelTokey, inputColumnName: DefaultColumnNames.Label)
                            // </SnippetMapValueToKey1>
                            // The image transforms transform the images into the model's expected format.
                            // <SnippetImageTransforms>
                            .Append(mlContext.Transforms.LoadImages(_trainImagesFolder, (ImageReal, nameof(ImageData.ImagePath))))
                            .Append(mlContext.Transforms.Resize(outputColumnName: ImageReal, imageWidth: InceptionSettings.ImageWidth, imageHeight: InceptionSettings.ImageHeight, inputColumnName: ImageReal))
                            .Append(mlContext.Transforms.ExtractPixels(new ImagePixelExtractorTransformer.ColumnInfo(name: "input", inputColumnName: ImageReal, interleave: InceptionSettings.ChannelsLast, offset: InceptionSettings.Mean)))
                            // </SnippetImageTransforms>
                            // The ScoreTensorFlowModel transform scores the TensorFlow model and allows communication 
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

            // Create enumerables for both the ImageData and ImagePrediction DataViews 
            // for displaying results
            // <SnippetEnumerateDataViews>
            var imageData = mlContext.CreateEnumerable<ImageData>(data, false, true);
            var imagePredictionData = mlContext.CreateEnumerable<ImagePrediction>(predictions, false, true);
            // </SnippetEnumerateDataViews>

            // Read the tags.tsv file and add the filepath to the image file name 
            // before loading into ImageData 
            // <SnippetCallPairAndDisplayResults1>
            PairAndDisplayResults(imageData, imagePredictionData);
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

            // Read the image_list.tsv file and add the filepath to the image file name 
            // before loading into ImageData 
            // <SnippetReadFromTSV> 
            var imageData = ReadFromTsv(dataLocation, imagesFolder);
            var imageDataView = mlContext.Data.ReadFromEnumerable<ImageData>(imageData);
            // </SnippetReadFromTSV>  
            
            // <SnippetPredict>  
            var predictions = loadedModel.Transform(imageDataView);
            var imagePredictionData = mlContext.CreateEnumerable<ImagePrediction>(predictions, false,true);
            // </SnippetPredict> 

            Console.WriteLine("=============== Making classifications ===============");
            // <SnippetCallPairAndDisplayResults2>
            PairAndDisplayResults(imageData, imagePredictionData);
            // </SnippetCallPairAndDisplayResults2> 

        }

        public static void ClassifySingleImage(MLContext mlContext, string imagePath, string outputModelLocation)
        {
            Console.WriteLine($"=============== Loading model ===============");
            Console.WriteLine($"Model loaded: {outputModelLocation}");
            // Load the model
            // <SnippetLoadModel2>
            ITransformer loadedModel;
            using (var fileStream = new FileStream(outputModelLocation, FileMode.Open))
                loadedModel = mlContext.Model.Load(fileStream);
            // </SnippetLoadModel2>

            // load the fully qualified image file name into ImageData 
            // <SnippetLoadImageData> 
            var imageData = new ImageData()
            {
                ImagePath = imagePath
            }; 
            // </SnippetReadFromTSV2>  

            // <SnippetPredictSingle>  
            // Make prediction function (input = ImageNetData, output = ImageNetPrediction)
            var predictor = loadedModel.CreatePredictionEngine<ImageData, ImagePrediction>(mlContext);
            var prediction = predictor.Predict(imageData);
            // </SnippetPredictSingle> 

            Console.WriteLine("=============== Making single image classification ===============");
            // <SnippetDisplayPrediction>
            Console.WriteLine($"Image: {Path.GetFileName(imageData.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
            // </SnippetDisplayPrediction> 

        }

        private static void PairAndDisplayResults(IEnumerable<ImageData> imageNetData, IEnumerable<ImagePrediction> imageNetPredictionData)
        {
            // Builds pairs of (image, prediction) to sync up for display
            // <SnippetBuildImagePredictionPairs>
            IEnumerable<(ImageData image, ImagePrediction prediction)> imagesAndPredictions = imageNetData.Zip(imageNetPredictionData, (image, prediction) => (image, prediction));
            // </SnippetBuildImagePredictionPairs>

            // <SnippetDisplayPredictions>
            foreach ((ImageData image, ImagePrediction prediction) item in imagesAndPredictions)
            {
                Console.WriteLine($"Image: {Path.GetFileName(item.image.ImagePath)} predicted as: {item.prediction.PredictedLabelValue} with score: {item.prediction.Score.Max()} ");
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
