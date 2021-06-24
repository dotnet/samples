//  Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.Collections;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Microsoft.Samples.WindowsForms
{
    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.Samples.WindowsForms")]
	public interface IWeatherService
    {
		[OperationContract]
		WeatherData[] GetWeatherData(string[] localities);
    }

    // Service class which implements the service contract.
	public class WeatherService : IWeatherService
    {
		public WeatherData[] GetWeatherData(string[] localities)
		{
			ArrayList weatherArray = new ArrayList();
			foreach (string locality in localities)
			{
				WeatherData data = new WeatherData(locality);
				weatherArray.Add(data);
			}
			return (WeatherData[])weatherArray.ToArray(typeof(WeatherData));
		}
    }

    [DataContract(Namespace = "http://Microsoft.Samples.WindowsForms")]
	public class WeatherData
	{
		public WeatherData(string locality)
		{
			Random rand = new Random();
			int highDelta = rand.Next(3);
			int lowDelta = rand.Next(3);
			switch (locality)
			{
				case "Los Angeles":
					Locality = "Los Angeles";
					HighTemperature = 85 + highDelta;
					LowTemperature = 60 + lowDelta;
					break;
				case "Rio de Janeiro":
					Locality = "Rio de Janeiro";
					HighTemperature = 55 + highDelta;
					LowTemperature = 40 + lowDelta;
					break;
				case "New York":
					Locality = "New York";
					HighTemperature = 80 + highDelta;
					LowTemperature = 55 + lowDelta;
					break;
				case "London":
					Locality = "London";
					HighTemperature = 65 + highDelta;
					LowTemperature = 45 + lowDelta;
					break;
				case "Paris":
					Locality = "Paris";
					HighTemperature = 70 + highDelta;
					LowTemperature = 50 + lowDelta;
					break;
				case "Rome":
					Locality = "Rome";
					HighTemperature = 80 + highDelta;
					LowTemperature = 60 + lowDelta;
					break;
				case "Cairo":
					Locality = "Cairo";
					HighTemperature = 90 + highDelta;
					LowTemperature = 70 + lowDelta;
					break;
				case "Beijing":
					Locality = "Beijing";
					HighTemperature = 85 + highDelta;
					LowTemperature = 60 + lowDelta;
					break;
			}
			if (HighTemperature < LowTemperature)
			{
				int temp = HighTemperature;
				HighTemperature = LowTemperature;
				LowTemperature = temp;
			}
			if (HighTemperature < (LowTemperature - 8))
			{
				HighTemperature += 5;
			}
		}

		private int _highTemp;

		[DataMember]
		public int HighTemperature
		{
			get { return _highTemp; }
			set { _highTemp = value; }
		}

		private int _lowTemp;

		[DataMember]
		public int LowTemperature
		{
			get { return _lowTemp; }
			set { _lowTemp = value; }
		}

		private string _locality = "Los Angeles";

		[DataMember]
		public string Locality
 		{
			get { return _locality; }
			set { _locality = value; }
		}
	}
}
