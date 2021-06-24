
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.



using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace Microsoft.ServiceModel.Samples
{
    #region Contracts
    [ServiceContract]
    public interface ICalculatorContract
    {
        [OperationContract]
        int Add(int x, int y);
    }

    [ServiceContract]
    public interface IDatagramContract
    {
        [OperationContract(IsOneWay = true)]
        void Hello(string greeting);
    }

    [ServiceContract]
    public interface IStatusContract
    {
        [OperationContract]
        void Start();

        [OperationContract]
        string GetStatus();
    }
    #endregion

    #region Services
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    class CalculatorService : IDatagramContract, ICalculatorContract, IStatusContract
    {
        List<string> serverTraces = new List<string>();

        #region ICalculatorContract implementation
        public int Add(int x, int y)
        {
            serverTraces.Add(string.Format("Operation 'Add' is called: {0} + {1}", x, y));
            return (x + y);
        }
        #endregion

        #region IDatagramContract implementation
        public void Hello(string greeting)
        {
            serverTraces.Add(string.Format("Operation 'Hello' is called: {0}", greeting));
        }
        #endregion

        #region IStatusContract Members

        public void Start()
        {
            serverTraces.Clear();
        }

        public string GetStatus()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < serverTraces.Count; i++)
            {
                if (i == 0)
                {
                    sb.Append(serverTraces[i]);
                }
                else
                {
                    // Adding separators
                    sb.AppendFormat("|{0}", serverTraces[i]);
                }
            }

            return sb.ToString();
        }

        #endregion
    }
    #endregion
}

