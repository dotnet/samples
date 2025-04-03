//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;

namespace Microsoft.Samples.PortSharing
{
   // Define a service contract
   [ServiceContract(Namespace = "http://Microsoft.Samples.PortSharing")]
   public interface ICalculator
   {
      [OperationContract]
      double Add(double n1, double n2);

      [OperationContract]
      double Subtract(double n1, double n2);

      [OperationContract]
      double Multiply(double n1, double n2);

      [OperationContract]
      double Divide(double n1, double n2);
   }

   class client
   {
      static void Main(string[] args)
      {
         Console.Write("Enter the service number to test: ");
         ushort salt = ushort.Parse(Console.ReadLine());
         string address = String.Format("net.tcp://localhost:9000/calculator/{0}", salt);
         ChannelFactory<ICalculator> factory = new ChannelFactory<ICalculator>(new NetTcpBinding(), new EndpointAddress(address));
         ICalculator proxy = factory.CreateChannel();
         
         // Call the Add service operation.
         double value1 = 100.00D; double value2 = 15.99D;
         double result = proxy.Add(value1, value2);
         Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);
         
         // Call the Subtract service operation.
         value1 = 145.00D;
         value2 = 76.54D;
         result = proxy.Subtract(value1, value2);
         Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);
         
         // Call the Multiply service operation.
         value1 = 9.00D;
         value2 = 81.25D;
         result = proxy.Multiply(value1, value2);
         Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);
         
         // Call the Divide service operation.
         value1 = 22.00D;
         value2 = 7.00D;
         result = proxy.Divide(value1, value2);
         Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);
         
         Console.WriteLine();
         Console.WriteLine("Press <ENTER> to terminate client.");
         Console.ReadLine();
         factory.Close();
      }
   }
}
