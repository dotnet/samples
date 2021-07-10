//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.Transactions;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.ServiceModel.Samples
{
    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
    public interface ICalculator
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Mandatory)]
        double Add(double n1, double n2);
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        double Subtract(double n1, double n2);
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.NotAllowed)]
        double Multiply(double n1, double n2);
        [OperationContract]
        double Divide(double n1, double n2); 
    }

    // Service class which implements the service contract.
    [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.Serializable)]
    public class CalculatorService : ICalculator
    {

        [OperationBehavior(TransactionScopeRequired = true)]
        public double Add(double n1, double n2)
        {
            RecordToLog(String.Format(CultureInfo.CurrentCulture, "Adding {0} to {1}", n1, n2));
            return n1 + n2;
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public double Subtract(double n1, double n2)
        {
            RecordToLog(String.Format(CultureInfo.CurrentCulture, "Subtracting {0} from {1}", n2, n1));
            return n1 - n2;
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public double Multiply(double n1, double n2)
        {
            RecordToLog(String.Format(CultureInfo.CurrentCulture, "Multiplying {0} by {1}", n1, n2));
            return n1 * n2;
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public double Divide(double n1, double n2)
        {
            RecordToLog(String.Format(CultureInfo.CurrentCulture, "Dividing {0} by {1}", n1, n2));
            return n1 / n2;
        }

        private static void RecordToLog(string recordText)
        {
            // Record the operations performed
            if (ConfigurationManager.AppSettings["usingSql"] == "true")
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["connectionString"]))
                {
                    conn.Open();

                    SqlCommand cmdLog = new SqlCommand("INSERT into Log (Entry) Values (@Entry)", conn);
                    cmdLog.Parameters.AddWithValue("@Entry", recordText);
                    cmdLog.ExecuteNonQuery();
                    cmdLog.Dispose();

                    Console.WriteLine("  Writing row to database: {0}", recordText);

                    conn.Close();
                }
            }
            else
                Console.WriteLine("  Noting row: {0}", recordText);
        }
    }
}
