// <SnippetAddUsings>
using Microsoft.ML.Data;
// </SnippetAddUsings>

namespace SentimentAnalysis
{
    // <SnippetDeclareTypes>
    public class SentimentData
    {
        [LoadColumn(0)]
        public string SentimentText;

        [LoadColumn(1), ColumnName("Label")]
        public bool Sentiment;
    }

    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

       // [ColumnName("Probability")]
        public float Probability { get; set; }

      //  [ColumnName("Score")]
        public float Score { get; set; }
    }
    // </SnippetDeclareTypes>
}
