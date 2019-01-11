// <SnippetUsingsForPaths>
using System;
using System.IO;
// </SnippetUsingsForPaths>

// <SnippetMLUsings>
using Microsoft.ML;
using Microsoft.ML.Data;
// </SnippetMLUsings>

namespace IrisFlowerClustering
{
    class Program
    {
        // <SnippetPaths>
        static readonly string _dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "iris.data");
        static readonly string _modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "IrisClusteringModel.zip");
        // </SnippetPaths>

        static void Main(string[] args)
        {
            // <SnippetCreateContext>
            var mlContext = new MLContext(seed: 0);
            // </SnippetCreateContext>

            // <SnippetSetupTextLoader>
            TextLoader textLoader = mlContext.Data.CreateTextReader(new TextLoader.Arguments()
            {
                Separator = ",",
                HasHeader = false,
                Column = new[]
                            {
                                new TextLoader.Column("SepalLength", DataKind.R4, 0),
                                new TextLoader.Column("SepalWidth", DataKind.R4, 1),
                                new TextLoader.Column("PetalLength", DataKind.R4, 2),
                                new TextLoader.Column("PetalWidth", DataKind.R4, 3)
                            }
            });
            // </SnippetSetupTextLoader>

            // <SnippetCreateDataView>
            IDataView dataView = textLoader.Read(_dataPath);
            // </SnippetCreateDataView>

            // <SnippetCreatePipeline>
            string featuresColumnName = "Features";
            var pipeline = mlContext.Transforms
                .Concatenate(featuresColumnName, "SepalLength", "SepalWidth", "PetalLength", "PetalWidth")
                .Append(mlContext.Clustering.Trainers.KMeans(featuresColumnName, clustersCount: 3));
            // </SnippetCreatePipeline>

            // <SnippetTrainModel>
            var model = pipeline.Fit(dataView);
            // </SnippetTrainModel>

            // <SnippetSaveModel>
            using (var fileStream = new FileStream(_modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                mlContext.Model.Save(model, fileStream);
            }
            // </SnippetSaveModel>

            // <SnippetPredictor>
            var predictor = model.CreatePredictionEngine<IrisData, ClusterPrediction>(mlContext);
            // </SnippetPredictor>

            // <SnippetPredictionExample>
            var prediction = predictor.Predict(TestIrisData.Setosa);
            Console.WriteLine($"Cluster: {prediction.PredictedClusterId}");
            Console.WriteLine($"Distances: {string.Join(" ", prediction.Distances)}");
            // </SnippetPredictionExample>
        }
    }
}
