// <Snippet1>
using Microsoft.ML.Runtime.Api;
// </Snippet1>

namespace SentimentAnalysis
{
    // <Snippet2>
    public class SentimentData
    {
        [Column(ordinal: "0", name: "Label")]
        public float Sentiment;
        [Column(ordinal: "1")]
        public string SentimentText;
    }

    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Sentiment;
    }
    // </Snippet2>
}