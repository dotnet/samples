
//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.Samples.SBGenericsVTS.SharedCode
{
    [Serializable]
    public class Customer
    {
        public Customer(string name, int age)
        {
            CustomerName = name;
            CustomerAge = age;
        }

        public string CustomerName
        {
            get;
            set;
        }

        public int CustomerAge
        {
            get;
            set;
        }

        public override string ToString()
        {
            return "Customer:\n  Name: " + CustomerName + "\n  Age: " + CustomerAge;
        }
    }

    public interface IService<T>
    {
        List<T> Reverse(List<T> list);
    }

    public class Service<T> : MarshalByRefObject, IService<T>
    {
        public Service()
        {
        }

        public List<T> Reverse(List<T> list)
        {
            list.Reverse();
            return list;
        }
    }
}
