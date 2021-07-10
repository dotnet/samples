
//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Samples.KAA.Common;
using Microsoft.Samples.KAA.Types;

namespace Microsoft.Samples.KAA
{

    [ServiceContract(Namespace = "http://Microsoft.Samples.KAA")]
    [KnownAssembly("Types")]
    public interface IDataContractCalculator
    {
        [OperationContract]
        ComplexNumber Add(ComplexNumber n1, ComplexNumber n2);

        [OperationContract]
        ComplexNumber Subtract(ComplexNumber n1, ComplexNumber n2);

        [OperationContract]
        ComplexNumber Multiply(ComplexNumber n1, ComplexNumber n2);

        [OperationContract]
        ComplexNumber Divide(ComplexNumber n1, ComplexNumber n2);

        [OperationContract]
        List<ComplexNumber> CombineLists(List<ComplexNumber> list1, List<ComplexNumber> list2);
    }

    // Client implementation code
    class Client
    {
        static void Main()
        {
            // Create a channel
            EndpointAddress address = new EndpointAddress("http://localhost/servicemodelsamples/service.svc");
            BasicHttpBinding binding = new BasicHttpBinding();
            ChannelFactory<IDataContractCalculator> factory = new ChannelFactory<IDataContractCalculator>(binding, address);
            IDataContractCalculator channel = factory.CreateChannel();

            // Call the Add service operation
            ComplexNumber value1 = new ComplexNumber(1, 2);
            ComplexNumber value2 = new ComplexNumberWithMagnitude(3, 4);
            ComplexNumber result = channel.Add(value1, value2);
            Console.WriteLine("Add({0} + {1}i, {2} + {3}i) = {4} + {5}i",
                value1.Real, value1.Imaginary, value2.Real, value2.Imaginary, result.Real, result.Imaginary);
            if (result is ComplexNumberWithMagnitude)
            {
                Console.WriteLine("Magnitude: {0}", ((ComplexNumberWithMagnitude)result).Magnitude);
            }
            else
            {
                Console.WriteLine("No magnitude was sent from the service");
            }
            Console.WriteLine();

            // Call the Subtract service operation
            value1 = new ComplexNumber(1, 2);
            value2 = new ComplexNumber(3, 4);
            result = channel.Subtract(value1, value2);
            Console.WriteLine("Subtract({0} + {1}i, {2} + {3}i) = {4} + {5}i",
                value1.Real, value1.Imaginary, value2.Real, value2.Imaginary, result.Real, result.Imaginary);
            if (result is ComplexNumberWithMagnitude)
            {
                Console.WriteLine("Magnitude: {0}", ((ComplexNumberWithMagnitude)result).Magnitude);
            }
            else
            {
                Console.WriteLine("No magnitude was sent from the service");
            }
            Console.WriteLine();

            // Call the Multiply service operation
            value1 = new ComplexNumber(2, 3);
            value2 = new ComplexNumber(4, 7);
            result = channel.Multiply(value1, value2);
            Console.WriteLine("Multiply({0} + {1}i, {2} + {3}i) = {4} + {5}i",
                value1.Real, value1.Imaginary, value2.Real, value2.Imaginary, result.Real, result.Imaginary);
            if (result is ComplexNumberWithMagnitude)
            {
                Console.WriteLine("Magnitude: {0}", ((ComplexNumberWithMagnitude)result).Magnitude);
            }
            else
            {
                Console.WriteLine("No magnitude was sent from the service");
            }
            Console.WriteLine();

            // Call the Divide service operation
            value1 = new ComplexNumber(3, 7);
            value2 = new ComplexNumber(5, -2);
            result = channel.Divide(value1, value2);
            Console.WriteLine("Divide({0} + {1}i, {2} + {3}i) = {4} + {5}i",
                value1.Real, value1.Imaginary, value2.Real, value2.Imaginary, result.Real, result.Imaginary);
            if (result is ComplexNumberWithMagnitude)
            {
                Console.WriteLine("Magnitude: {0}", ((ComplexNumberWithMagnitude)result).Magnitude);
            }
            else
            {
                Console.WriteLine("No magnitude was sent from the service");
            }
            Console.WriteLine();

            // Call the CombineLists service operation
            List<ComplexNumber> list1 = new List<ComplexNumber>();
            List<ComplexNumber> list2 = new List<ComplexNumber>();
            list1.Add(new ComplexNumber(1, 1));
            list1.Add(new ComplexNumber(2, 2));
            list1.Add(new ComplexNumberWithMagnitude(3, 3));
            list1.Add(new ComplexNumberWithMagnitude(4, 4));
            List<ComplexNumber> listResult = channel.CombineLists(list1, list2);
            Console.WriteLine("Lists combined:");
            foreach (ComplexNumber n in listResult)
            {
                Console.WriteLine("{0} + {1}i", n.Real, n.Imaginary);
            }
            Console.WriteLine();

            // Close the channel
            ((IChannel)channel).Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}
