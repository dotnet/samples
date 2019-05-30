// <SnippetAddUsings>
using System;
using System.IO;
using System.Net;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
// </SnippetAddUsings>

namespace TextClassificationTF
{

    class Program
    {
        // <SnippetDeclareGlobalVariables>
        public const int MaxSentenceLength = 600;
        // </SnippetDeclareGlobalVariables>

        static void Main(string[] args)
        {
            // Create MLContext to be shared across the model creation workflow objects 
            // <SnippetCreateMLContext>
            MLContext mlContext = new MLContext(seed: 1);
            // </SnippetCreateMLContext>

            // <SnippetCalReuseAndTuneSentimentModel>
            var model = ReuseAndTuneSentimentModel(mlContext);
            // </SnippetCallReuseAndTuneSentimentModel>

            // <SnippetCallPredictSentiment>
            PredictSentiment(mlContext, model);
            // </SnippetCallPredictSentiment>
        }

        public static ITransformer ReuseAndTuneSentimentModel(MLContext mlContext)
        {
            // <SnippetDownloadModel>
            string modelLocation = DownloadTensorFlowSentimentModel();
            // <SnippetDownloadModel>

            var trainData = new[] { new IMDBSentiment() {
                Sentiment_Text = "this film was just brilliant casting location scenery story direction " +
                                    "everyone's really suited the part they played and you could just imagine being there robert " +
                                    "is an amazing actor and now the same being director  father came from the same scottish " +
                                    "island as myself so i loved the fact there was a real connection with this film the witty " +
                                    "remarks throughout the film were great it was just brilliant so much that i bought the " +
                                    "film as soon as it was released for  and would recommend it to everyone to watch and the " +
                                    "fly fishing was amazing really cried at the end it was so sad and you know what they say " +
                                    "if you cry at a film it must have been good and this definitely was also  to the two " +
                                    "little boy's that played the  of norman and paul they were just brilliant children are " +
                                    "often left out of the  list i think because the stars that play them all grown up are " +
                                    "such a big profile for the whole film but these children are amazing and should be praised " +
                                    "for what they have done don't you think the whole story was so lovely because it was true " +
                                    "and was someone's life after all that was shared with us all"
            } };

            var dataView = mlContext.Data.LoadFromEnumerable(trainData);

            // This is the dictionary to convert words into the integer indexes.
            var lookupMap = mlContext.Data.LoadFromTextFile(Path.Combine(modelLocation, "imdb_word_index.csv"),
                columns: new[]
                   {
                        new TextLoader.Column("Words", DataKind.String, 0),
                        new TextLoader.Column("Ids", DataKind.Int32, 1),
                   },
                separatorChar: ','
               );

            // Load the TensorFlow model once.
            //      - Use it for quering the schema for input and output in the model
            //      - Use it for prediction in the pipeline.
            // <SnippetLoadTensorFlowModel>
            var tensorFlowModel = mlContext.Model.LoadTensorFlowModel(modelLocation);
            // </SnippetLoadTensorFlowModel>

            // <SnippetGetModelSchema>
            var schema = tensorFlowModel.GetModelSchema();
            // </SnippetGetModelSchema>

            //var featuresType = (VectorDataViewType)schema["Features"].Type;
            //Console.WriteLine("Name: {0}, Type: {1}, Shape: (-1, {2})", "Features", featuresType.ItemType.RawType, featuresType.Dimensions[0]);
            //var predictionType = (VectorDataViewType)schema["Prediction/Softmax"].Type;
            //Console.WriteLine("Name: {0}, Type: {1}, Shape: (-1, {2})", "Prediction/Softmax", predictionType.ItemType.RawType, predictionType.Dimensions[0]);

            // The model expects the input feature vector to be a fixed length vector.
            // This action resizes the integer vector to a fixed length vector 
            // required for CustomMappingEstimator input
            // <SnippetResizeFeatures>
            Action<IMDBSentiment, IntermediateFeatures> ResizeFeaturesAction = (s, f) =>
            {
                f.Sentiment_Text = s.Sentiment_Text;
                var features = s.VariableLengthFeatures;
                Array.Resize(ref features, MaxSentenceLength);
                f.Features = features;
            };
            // </SnippetResizeFeatures>


            // A pipeline converts text into vector of words.
            // 'TokenizeIntoWords' uses spaces to parse the text/string into words
            // Space is also a default value for the 'separators' argument if it is not specified.
            var pipeline = mlContext.Transforms.Text.TokenizeIntoWords("TokenizedWords", "Sentiment_Text")
                // MapValue maps each word to an integer which is an index in the dictionary ('lookupMap'),
                .Append(mlContext.Transforms.Conversion.MapValue("VariableLengthFeatures", lookupMap,
                    lookupMap.Schema["Words"], lookupMap.Schema["Ids"], "TokenizedWords"))
                // CustomMappingEstimator is used to resize variable length vector 
                // to fixed length vector via ResizeFeaturesAction.
                .Append(mlContext.Transforms.CustomMapping(ResizeFeaturesAction, "Resize")) 
                // Passes the data to TensorFlow for scoring
                .Append(tensorFlowModel.ScoreTensorFlowModel("Prediction/Softmax", "Features"))
                // Retreives the 'Prediction' from TensorFlow and and copies to a column 
                .Append(mlContext.Transforms.CopyColumns("Prediction", "Prediction/Softmax"));


            // Train the model
            Console.WriteLine("=============== Training classification model ===============");
            // Create and train the model based on the dataset that has been loaded, transformed.
            // <SnippetTrainModel>
           ITransformer model = pipeline.Fit(dataView);
            // </SnippetTrainModel>

            // <SnippetReturnModel>
            return model;
            // </SnippetReturnModel>
        }

        /// <summary>
        /// Downloads sentiment_model from the dotnet/machinelearning-testdata repo.
        /// </summary>
        /// <remarks>
        /// The model is downloaded from
        /// https://github.com/dotnet/machinelearning-testdata/blob/master/Microsoft.ML.TensorFlow.TestModels/sentiment_model
        /// The model is in 'SavedModel' format. For further explanation on how was the `sentiment_model` created
        /// c.f. https://github.com/dotnet/machinelearning-testdata/blob/master/Microsoft.ML.TensorFlow.TestModels/sentiment_model/README.md
        /// </remarks>
        public static string DownloadTensorFlowSentimentModel()
        {
            // <SnippetDeclareDownloadPath>
            string downloadPath = "https://github.com/dotnet/machinelearning-testdata/raw/master/Microsoft.ML.TensorFlow.TestModels/sentiment_model/";
            // </SnippetDeclareDownloadPath>

            // <SnippetCheckModelDir>
            string modelPath = "sentiment_model";
            if (!Directory.Exists(modelPath))
                Directory.CreateDirectory(modelPath);
            // </SnippetCheckModelDir>

            // <SnippetCheckVariableDir>
            string variablePath = Path.Combine(modelPath, "variables");
            if (!Directory.Exists(variablePath))
                Directory.CreateDirectory(variablePath);
            // </SnippetCheckVariableDir>

            // <SnippetDownloadModel>
            Download(Path.Combine(downloadPath, "saved_model.pb"), Path.Combine(modelPath, "saved_model.pb"));
            Download(Path.Combine(downloadPath, "imdb_word_index.csv"), Path.Combine(modelPath, "imdb_word_index.csv"));
            Download(Path.Combine(downloadPath, "variables", "variables.data-00000-of-00001"), Path.Combine(variablePath, "variables.data-00000-of-00001"));
            Download(Path.Combine(downloadPath, "variables", "variables.index"), Path.Combine(variablePath, "variables.index"));
            // </SnippetDownloadModel>

            // <SnippetReturnModelPath>
            return modelPath;
            // </SnippetReturnModelPath>
        }

        private static string Download(string baseGitPath, string dataFile)
        {
            // <SnippetDownloadDataFile>
            if (File.Exists(dataFile))
                return dataFile;

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri($"{baseGitPath}"), dataFile);
            }

            return dataFile;
            // <//SnippetDownloadDataFile>
        }

        public static void PredictSentiment(MLContext mlContext, ITransformer model)
        {
            // <SnippetCreatePredictionEngine>
            var engine = mlContext.Model.CreatePredictionEngine<IMDBSentiment, IMDBSentimentPrediction>(model);
            // </SnippetCreatePredictionEngine>

            // <SnippetCreateTestData>
            var data = new[] { new IMDBSentiment() {
                Sentiment_Text = "this film is really bad"
            }};
            // </SnippetCreateTestData>

            // Predict with TensorFlow pipeline.
            // <SnippetPredict>  
            var prediction = engine.Predict(data[0]);
            // </SnippetPredict>  

            // <SnippetDisplayPredictions>
            Console.WriteLine("Number of classes: {0}", prediction.Prediction.Length);
            Console.WriteLine("Is sentiment/review positive? {0}", prediction.Prediction[1] > 0.5 ? "Yes." : "No.");
            Console.WriteLine("Prediction Confidence: {0}", prediction.Prediction[1].ToString("0.00"));
            // </SnippetDisplayPredictions>
            /////////////////////////////////// Expected output ///////////////////////////////////
            // 
            // Name: Features, Type: System.Int32, Shape: (-1, 600)
            // Name: Prediction/Softmax, Type: System.Single, Shape: (-1, 2)
            // 
            // Number of classes: 2
            // Is sentiment/review positive ? Yes
            // Prediction Confidence: 0.65
        }
        // <SnippetDeclareIntermediateFeatures>
        /// <summary>
        /// Class to hold intermediate data. Mostly used by CustomMapping Estimator
        /// </summary>
        public class IntermediateFeatures
        {
            public string Sentiment_Text { get; set; }

            [VectorType(MaxSentenceLength)]
            public int[] Features { get; set; }
        }
        // </SnippetDeclareIntermediateFeatures>
    }

}
