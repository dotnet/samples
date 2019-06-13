// <SnippetAddUsings>
using Microsoft.ML.Data;
// </SnippetAddUsings>

namespace TextClassificationTF
{
    // <SnippetDeclareTypes>
    /// <summary>
    /// Class to hold original sentiment data.
    /// </summary>
    public class IMDBSentiment
    {
        public string Sentiment_Text { get; set; }

        /// <summary>
        /// This is a variable length vector designated by VectorType attribute.
        /// Variable length vectors are produced by applying operations such as 'TokenizeWords' on strings
        /// resulting in vectors of tokens of variable lengths.
        /// </summary>
        [VectorType]
        public int[] VariableLengthFeatures { get; set; }
    }  

    /// <summary>
    /// Class to contain the output values from the transformation.
    /// </summary>
    public class IMDBSentimentPrediction : IMDBSentiment
    {
        [VectorType(2)]
        public float[] Prediction { get; set; }
    }
    // <SnippetDeclareTypes>
}
