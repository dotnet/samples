// <SnippetUsings>
using Microsoft.ML.Runtime.Api;
// </SnippetUsings>

namespace IrisFlowerClustering
{
    // <SnippetClassDefinitions>
    public class IrisData
    {
        [Column("0")]
        public float SepalLength;

        [Column("1")]
        public float SepalWidth;

        [Column("2")]
        public float PetalLength;

        [Column("3")]
        public float PetalWidth;
    }

    public class ClusterPrediction
    {
        [ColumnName("PredictedLabel")]
        public uint PredictedClusterId;

        [ColumnName("Score")]
        public float[] Distances;
    }
    // </SnippetClassDefinitions>
}
