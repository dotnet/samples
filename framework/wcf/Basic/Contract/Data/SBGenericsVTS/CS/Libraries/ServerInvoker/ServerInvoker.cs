
//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Microsoft.Samples.SBGenericsVTS.SharedCode;

namespace Microsoft.Samples.SBGenericsVTS.Invoker
{
    public class ServerInvoker
    {
        public static void Invoke<T>(string url, List<T> list)
        {
            try
            {
                IService<T> obj = (IService<T>)Activator.GetObject(typeof(IService<T>), url);
                if (obj == null)
                {
                    Console.WriteLine("Could not locate remote object.");
                }
                else
                {
                    List<T> listResult = (List<T>)obj.Reverse(list);

                    foreach (T t in listResult)
                    {
                        Console.WriteLine(t.ToString());
                    }
                }
                Console.WriteLine("");
            }
            catch (RemotingException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
