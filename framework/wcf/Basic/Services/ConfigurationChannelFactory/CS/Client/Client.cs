
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Configuration;

namespace Microsoft.Samples.ConfigurationChannelFactory
{
    public class Client
    {
        public static void Main()
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = "Test.config";
            Configuration newConfiguration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            ConfigurationChannelFactory<ICalculatorChannel> factory1 = new ConfigurationChannelFactory<ICalculatorChannel>("endpoint1", newConfiguration, new EndpointAddress("http://localhost:8000/servicemodelsamples/service"));
            ICalculatorChannel client1 = factory1.CreateChannel();

            ConfigurationChannelFactory<ICalculatorChannel> factory2 = new ConfigurationChannelFactory<ICalculatorChannel>("endpoint2", newConfiguration, new EndpointAddress("net.tcp://localhost:8080/servicemodelsamples/service"));
            ICalculatorChannel client2 = factory2.CreateChannel();

            // Call the Add service operation.
            double value1 = 100.00D;
            double value2 = 15.99D;
            double result = client1.Add(value1, value2);
            Console.WriteLine("(HTTP)Add({0},{1}) = {2}", value1, value2, result);
            result = client2.Add(value1, value2);
            Console.WriteLine("(TCP) Add({0},{1}) = {2}", value1, value2, result);

            // Call the Subtract service operation.
            value1 = 145.00D;
            value2 = 76.54D;
            result = client1.Subtract(value1, value2);
            Console.WriteLine("(HTTP)Subtract({0},{1}) = {2}", value1, value2, result);
            result = client2.Subtract(value1, value2);
            Console.WriteLine("(TCP) Subtract({0},{1}) = {2}", value1, value2, result);

            // Call the Multiply service operation.
            value1 = 9.00D;
            value2 = 81.25D;
            result = client1.Multiply(value1, value2);
            Console.WriteLine("(HTTP)Multiply({0},{1}) = {2}", value1, value2, result);
            result = client2.Multiply(value1, value2);
            Console.WriteLine("(TCP) Multiply({0},{1}) = {2}", value1, value2, result);

            // Call the Divide service operation.
            value1 = 22.00D;
            value2 = 7.00D;
            result = client1.Divide(value1, value2);
            Console.WriteLine("(HTTP)Divide({0},{1}) = {2}", value1, value2, result);
            result = client2.Divide(value1, value2);
            Console.WriteLine("(TCP) Divide({0},{1}) = {2}", value1, value2, result);

            // Closing the connection gracefully and cleaning up resources
            client1.Close();
            client2.Close();

            Console.WriteLine("Press <ENTER> to exit.");
            Console.ReadLine();
        }
    }
}
