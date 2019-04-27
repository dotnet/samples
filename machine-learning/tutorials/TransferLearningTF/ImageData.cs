// <SnippetAddUsings>
using Microsoft.ML.Data;
// </SnippetAddUsings>
namespace TransferLearningTF
{
    // <SnippetDeclareTypes>
    public class ImageData
    {
        [LoadColumn(0)]
        public string ImagePath;

        [LoadColumn(1)]
        public string Label;
    }
    // </SnippetDeclareTypes>
}
