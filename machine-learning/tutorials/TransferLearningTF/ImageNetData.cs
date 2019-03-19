// <SnippetAddUsings>
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// </SnippetAddUsings>
namespace TransferLearningTF//.ImageData
{
    // <SnippetDeclareTypes>
    public class ImageNetData
    {
        [LoadColumn(0)]
        public string ImagePath;

        [LoadColumn(1)]
        public string Label;

        public static IEnumerable<ImageNetData> ReadFromCsv(string file, string folder)
        {
            return File.ReadAllLines(file)
             .Select(x => x.Split('\t'))
             .Select(x => new ImageNetData()
             {
                 ImagePath = Path.Combine(folder,x[0]),
                 Label = x[1],
             });
        }
    }

    public class ImageNetDataProbability : ImageNetData
    {
        public float Probability { get; set; }
    }

    public class ImageNetPipeline
    {
        public string ImagePath;
        public string Label;
        public string PredictedLabelValue;
        public float[] Score;
        public float[] softmax2_pre_activation;
    }
    // <SnippetDeclareTypes>
}
