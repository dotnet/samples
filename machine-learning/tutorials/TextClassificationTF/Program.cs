// <SnippetAddUsings>
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;
// </SnippetAddUsings>

namespace TextClassificationTF
{
    class Program
    {
        // <SnippetDeclareGlobalVariables>
        public const int FeatureLength = 600;
        static readonly string _modelPath = Path.Combine(Environment.CurrentDirectory, "sentiment_model");
        // </SnippetDeclareGlobalVariables>

        static void Main(string[] args)
        {
            // Create MLContext to be shared across the model creation workflow objects 
            // <SnippetCreateMLContext>
            MLContext mlContext = new MLContext();
            // </SnippetCreateMLContext>

            // <SnippetCreateEmptyDataView>
            IEnumerable<IMDBSentiment> emptyData = new List<IMDBSentiment>();
            var dataView = mlContext.Data.LoadFromEnumerable(emptyData);
            // </SnippetCreateEmptyDataView>

            // Dictionary to convert words into integer indexes.
            // <SnippetCreateLookupMap>
            var lookupMap = mlContext.Data.LoadFromTextFile(Path.Combine(_modelPath, "imdb_word_index.csv"),
                columns: new[]
                   {
                        new TextLoader.Column("Words", DataKind.String, 0),
                        new TextLoader.Column("Ids", DataKind.Int32, 1),
                   },
                separatorChar: ','
               );
            // </SnippetCreateLookupMap>

            // Load the TensorFlow model.
            // <SnippetLoadTensorFlowModel>
            var tensorFlowModel = mlContext.Model.LoadTensorFlowModel(_modelPath);
            // </SnippetLoadTensorFlowModel>

            // <SnippetGetModelSchema>
            var schema = tensorFlowModel.GetModelSchema();
            Console.WriteLine(" =============== TensorFlow Model Schema =============== ");
            var featuresType = (VectorDataViewType)schema["Features"].Type;
            Console.WriteLine($"Name: Features, Type: {featuresType.ItemType.RawType}, Shape: (-1, {featuresType.Dimensions[0]})");
            var predictionType = (VectorDataViewType)schema["Prediction/Softmax"].Type;
            Console.WriteLine($"Name: Prediction/Softmax, Type: {featuresType.ItemType.RawType}, Shape: (-1, {featuresType.Dimensions[0]})");
            // </SnippetGetModelSchema>

            // The model expects the input feature vector to be a fixed length vector.
            // This action resizes the integer vector to a fixed length vector. If there
            // are less than 600 words in the sentence, the remaining indices will be filled
            // with zeros. If there are more than 600 words in the sentence, then the
            // array is truncated at 600.
            // <SnippetResizeFeatures>
            Action<IMDBSentiment, IntermediateFeatures> ResizeFeaturesAction = (s, f) =>
            {
                f.Sentiment_Text = s.SentimentText;
                var features = s.VariableLengthFeatures;
                Array.Resize(ref features, FeatureLength);
                f.Features = features;
            };
            // </SnippetResizeFeatures>

            // <SnippetTokenizeIntoWords>
            var pipeline =
                // Split the text into individual words
                mlContext.Transforms.Text.TokenizeIntoWords("TokenizedWords", "SentimentText")
                // </SnippetTokenizeIntoWords>

                // <SnippetMapValue>
                // Map each word to an integer value. The array of integer makes up the input features.
                .Append(mlContext.Transforms.Conversion.MapValue("VariableLengthFeatures", lookupMap,
                    lookupMap.Schema["Words"], lookupMap.Schema["Ids"], "TokenizedWords"))
                // </SnippetMapValue>

                // <SnippetCustomMapping>
                // Resize variable length vector to fixed length vector.
                .Append(mlContext.Transforms.CustomMapping(ResizeFeaturesAction, "Resize"))
                // </SnippetCustomMapping>

                // <SnippetScoreTensorFlowModel>
                // Passes the data to TensorFlow for scoring
                .Append(tensorFlowModel.ScoreTensorFlowModel("Prediction/Softmax", "Features"))
                // </SnippetScoreTensorFlowModel>

                // <SnippetCopyColumns>
                // Retrieves the 'Prediction' from TensorFlow and and copies to a column
                .Append(mlContext.Transforms.CopyColumns("Prediction", "Prediction/Softmax"));
            // </SnippetCopyColumns>

            // Train the model
            Console.WriteLine("=============== Training classification model ===============");

            // <SnippetCreateModel>
            // Create and train the model based on the dataset that has been loaded, transformed.
            ITransformer model = pipeline.Fit(dataView);
            // </SnippetCreateModel>

            // <SnippetCallPredictSentiment>
            PredictSentiment(mlContext, model);
            // </SnippetCallPredictSentiment>
        }

        public static void PredictSentiment(MLContext mlContext, ITransformer model)
        {
            // <SnippetCreatePredictionEngine>
            var engine = mlContext.Model.CreatePredictionEngine<IMDBSentiment, IMDBSentimentPrediction>(model);
            // </SnippetCreatePredictionEngine>

            // <SnippetCreateTestData>
            var data = new[] { new IMDBSentiment() {
                SentimentText = "this film is really good"
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

            [VectorType(FeatureLength)]
            public int[] Features { get; set; }
        }
        // </SnippetDeclareIntermediateFeatures>
    }

}
