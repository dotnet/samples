// <SnippetAddUsings>
using System;
using System.IO;
using Microsoft.ML;
// </SnippetAddUsings>

namespace ProductSalesAnomalyDetection
{
    class Program
    {
        // <SnippetDeclareGlobalVariables>
        static readonly string _dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "product-sales.csv");
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
            IDataView dataView = mlContext.Data.LoadFromTextFile<ProductSalesData>(path: _dataPath, hasHeader: true, separatorChar: ',');
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
        static void DetectSpike(MLContext mlContext, int docSize, IDataView productSales)
        {
            Console.WriteLine("Detect temporary changes in pattern");

            // STEP 2: Set the training algorithm   
            // <SnippetAddSpikeTrainer> 
            var iidSpikeEstimator = mlContext.Transforms.DetectIidSpike(outputColumnName: nameof(ProductSalesPrediction.Prediction), inputColumnName: nameof(ProductSalesData.numSales), confidence: 95, pvalueHistoryLength: docSize / 4);
            // </SnippetAddSpikeTrainer> 

            // STEP 3:Train the model by fitting the dataview
            // Create and train the model based on the dataset that has been loaded, transformed.
            Console.WriteLine("=============== Training the model ===============");
            // <SnippetTrainModel1>
            ITransformer trainedModel = iidSpikeEstimator.Fit(productSales);
            // </SnippetTrainModel1>

            Console.WriteLine("=============== End of training process ===============");
             //Apply data transformation to create predictions.
            // <SnippetTransformData1>
            IDataView transformedData = trainedModel.Transform(productSales);
            // </SnippetTransformData1>
 
            // <SnippetCreateEnumerable1>
            var predictions = mlContext.Data.CreateEnumerable<ProductSalesPrediction>(transformedData, reuseRowObject: false);
            // </SnippetCreateEnumerable1>
            
            // <SnippetDisplayHeader1>
            Console.WriteLine("Alert\tScore\tP-Value");
            // </SnippetDisplayHeader1>

            // <SnippetDisplayResults1>
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
            // </SnippetDisplayResults1>
        }

        static void DetectChangepoint(MLContext mlContext, int docSize, IDataView productSales)
        {
            Console.WriteLine("Detect Persistent changes in pattern");

            //STEP 2: Set the training algorithm 
            // <SnippetAddChangePointTrainer> 
            var iidChangePointEstimator = mlContext.Transforms.DetectIidChangePoint(outputColumnName: nameof(ProductSalesPrediction.Prediction), inputColumnName: nameof(ProductSalesData.numSales), confidence: 95, changeHistoryLength: docSize / 4);
            // </SnippetAddChangePointTrainer> 

            //STEP 3:Train the model by fitting the dataview
            Console.WriteLine("=============== Training the model Using Change Point Detection Algorithm===============");
            // </SnippetTrainModel2>
            var trainedModel = iidChangePointEstimator.Fit(productSales);
            // </SnippetTrainModel2>
            Console.WriteLine("=============== End of training process ===============");

            //Apply data transformation to create predictions.
            // <SnippetTransformData2>
            IDataView transformedData = trainedModel.Transform(productSales);
            // </SnippetTransformData2>

            // <SnippetCreateEnumerable2>
            var predictions = mlContext.Data.CreateEnumerable<ProductSalesPrediction>(transformedData, reuseRowObject: false);
            // </SnippetCreateEnumerable2>
            
            // <SnippetDisplayHeader2>
            Console.WriteLine("Alert\tScore\tP-Value\tMartingale value");
            // </SnippetDisplayHeader2>

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
