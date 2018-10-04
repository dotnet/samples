// <Snippet2>
using System;
using System.IO;
// </Snippet2>
// <Snippet12>
using System.Threading.Tasks;
// </Snippet12>
// <Snippet3>
using Microsoft.ML.Legacy;
using Microsoft.ML.Legacy.Data;
using Microsoft.ML.Legacy.Trainers;
using Microsoft.ML.Legacy.Transforms;
// </Snippet3>

namespace IrisClustering
{
    public static class Program
    {
        // <Snippet1>
        static readonly string _dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "iris.data");
        static readonly string _modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "IrisClusteringModel.zip");
        // </Snippet1>

        // <Snippet11>
        private static async Task Main(string[] args)
        // </Snippet11>
        {
            // <Snippet4>
            PredictionModel<IrisData, ClusterPrediction> model = Train();
            // </Snippet4>

            // <Snippet10>
            await model.WriteAsync(_modelPath);
            // </Snippet10>

            // <Snippet13>
            var prediction = model.Predict(TestIrisData.Setosa);
            Console.WriteLine($"Cluster: {prediction.PredictedClusterId}");
            Console.WriteLine($"Distances: {string.Join(" ", prediction.Distances)}");
            // </Snippet13>
        }

        private static PredictionModel<IrisData, ClusterPrediction> Train()
        {
            // <Snippet5>
            var pipeline = new LearningPipeline();
            // </Snippet5>

            // <Snippet6>
            pipeline.Add(new TextLoader(_dataPath).CreateFrom<IrisData>(separator: ','));
            // </Snippet6>

            // <Snippet7>
            pipeline.Add(new ColumnConcatenator(
                    "Features",
                    "SepalLength",
                    "SepalWidth",
                    "PetalLength",
                    "PetalWidth"));
            // </Snippet7>

            // <Snippet8>
            pipeline.Add(new KMeansPlusPlusClusterer() { K = 3 });
            // </Snippet8>

            // <Snippet9>
            var model = pipeline.Train<IrisData, ClusterPrediction>();
            return model;
            // </Snippet9>
        }
    }
}
