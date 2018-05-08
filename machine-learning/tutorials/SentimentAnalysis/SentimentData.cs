// <Snippet1>
using Microsoft.ML.Runtime.Api;
// </Snippet1>

namespace SentimentAnalysis
{
    // <Snippet2>
    public class SentimentData
    {
        [Column(ordinal: "0")]
        public string SentimentText;
        [Column(ordinal: "1", name: "Label")]
        public float Sentiment;
    }

    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Sentiment;
    }
    // </Snippet2>
}
