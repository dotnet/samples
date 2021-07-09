//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;

namespace Microsoft.Samples.GZipEncoder
{
	[ServiceContract]
	public interface ISampleServer
	{
		[OperationContract]
		string Echo(string input);

		[OperationContract]
		string[] BigEcho(string[] input);
	}

	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
	public class SampleServer : ISampleServer
	{
		public string Echo(string input)
		{
			Console.WriteLine("\n\tServer Echo(string input) called:", input);
			Console.WriteLine("\tClient message:\t{0}\n", input);
			return input + " " + input;
		}

		public string[] BigEcho(string[] input)
		{
			Console.WriteLine("\n\tServer BigEcho(string[] input) called:", input);
			Console.WriteLine("\t{0} client messages", input.Length);
			return input;
		}
	}

	static class Program
	{
		static void Main()
		{
            using (ServiceHost sampleServer = new ServiceHost(typeof(SampleServer), new Uri("http://localhost:8000/samples/GZipEncoder")))
			{
				sampleServer.Open();
				Console.WriteLine("\nPress Enter key to Exit.");
				Console.ReadLine();

				sampleServer.Close();
			}
		}
	}
}
