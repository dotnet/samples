// <SnippetAddUsings>
using System;
using System.IO;
using Microsoft.ML;
// </SnippetAddUsings>

namespace ShampooSalesAnomalyDetection
{
    class Program
    {
        // <SnippetDeclareGlobalVariables>
        static readonly string _dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "sales-of-shampoo-over-a-three-ye.csv");
        //assign the Number of records in dataset file to constant variable
        const int _docsize = 36;
        // </SnippetDeclareGlobalVariables>
        static void Main(string[] args)
        {
            // Create MLContext to be shared across the model creation workflow objects 
            // <SnippetCreateMLContext>
            MLContext mlContext = new MLContext();
            // </SnippetCreateMLContext>

            //STEP 1: Common data loading configuration
            // <SnippetLoadData>
            IDataView dataView = mlContext.Data.LoadFromTextFile<ShampooSalesData>(path: _dataPath, hasHeader: true, separatorChar: ',');
            // </SnippetLoadData>

            // Spike detects pattern temporary changes
            // <SnippetCallDetectSpike>
            DetectSpike(mlContext, _docsize, dataView);
            // </SnippetCallDetectSpike>

            // Changepoint detects pattern persistent changes 
            // <SnippetCallDetectChangepoint>
            DetectChangepoint(mlContext, _docsize, dataView);
            // </SnippetCallDetectChangepoint>
        }
        static void DetectSpike(MLContext mlContext, int size, IDataView dataView)
        {
            Console.WriteLine("Detect temporary changes in pattern");

            // STEP 2: Set the training algorithm   
            // <SnippetAddTrainer1> 
            var iidSpikeEstimator = mlContext.Transforms.DetectIidSpike(outputColumnName: nameof(ShampooSalesPrediction.Prediction), inputColumnName: nameof(ShampooSalesData.numSales), confidence: 95, pvalueHistoryLength: size / 4);
            // </SnippetAddTrainer1> 

            // STEP 3:Train the model by fitting the dataview
            // Create and train the model based on the dataset that has been loaded, transformed.
            Console.WriteLine("=============== Training the model ===============");
            // <SnippetTrainModel1>
            ITransformer trainedModel = iidSpikeEstimator.Fit(dataView);
            // </SnippetTrainModel1>

            Console.WriteLine("=============== End of training process ===============");
            // <SnippetDisplayHeader1>
            Console.WriteLine("Alert\tScore\tP-Value");
            // </SnippetDisplayHeader1>
            
            //Apply data transformation to create predictions.
            // <SnippetTransformData>
            IDataView transformedData = trainedModel.Transform(dataView);
            // </SnippetTransformData>

            var predictions = mlContext.Data.CreateEnumerable<ShampooSalesPrediction>(transformedData, reuseRowObject: false);
            // <SnippetDisplayResults>
            foreach (var p in predictions)
            {
                var results = $"{p.Prediction[0]}\t{p.Prediction[1]:f2}\t{p.Prediction[2]:F2}";

                if (p.Prediction[0] == 1)
                {
                    results += " <-- Spike detected";
                }

                Console.WriteLine(results);
            }
            Console.WriteLine("");
            // </SnippetDisplayResults>
        }

        static void DetectChangepoint(MLContext mlContext, int size, IDataView dataView)
        {
            Console.WriteLine("Detect Persistent changes in pattern");

            //STEP 2: Set the training algorithm 
            // <SnippetAddTrainer2> 
            var iidChangePointEstimator = mlContext.Transforms.DetectIidChangePoint(outputColumnName: nameof(ShampooSalesPrediction.Prediction), inputColumnName: nameof(ShampooSalesData.numSales), confidence: 95, changeHistoryLength: size / 4);
            // </SnippetAddTrainer2> 
            //STEP 3:Train the model by fitting the dataview
            Console.WriteLine("=============== Training the model Using Change Point Detection Algorithm===============");
            // </SnippetTrainModel2>
            ITransformer trainedModel = iidChangePointEstimator.Fit(dataView);
            // </SnippetTrainModel2>
            Console.WriteLine("=============== End of training process ===============");

            // <SnippetDisplayHeader2>
            Console.WriteLine("Alert\tScore\tP-Value\tMartingale value");
            // </SnippetDisplayHeader2>
            //Apply data transformation to create predictions.
            // <SnippetTransformData2>
            IDataView transformedData = trainedModel.Transform(dataView);
            // </SnippetTransformData2>

            var predictions = mlContext.Data.CreateEnumerable<ShampooSalesPrediction>(transformedData, reuseRowObject: false);
            // <SnippetDisplayResults2>
            foreach (var p in predictions)
            {
                var results = $"{p.Prediction[0]}\t{p.Prediction[1]:f2}\t{p.Prediction[2]:F2}\t{p.Prediction[3]:F2}";

                if (p.Prediction[0] == 1)
                {
                    results += " <-- alert is on, predicted changepoint";
                }
                Console.WriteLine(results);
            }
            Console.WriteLine("");
            // </SnippetDisplayResults2>
        }
    }
}
