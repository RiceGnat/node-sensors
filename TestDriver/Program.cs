using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sensors.SoftwareInterface.SpeedFan;
using Sensors.SoftwareInterface.AISuite;

namespace Sensors.SoftwareInterface.TestDriver
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("SpeedFan data");
			PrintSensorData(SpeedFanInterface.GetData());

			Console.WriteLine();
			Console.WriteLine("AI Suite data");
			PrintSensorData(AISuite2Interface.GetData());

			Console.ReadKey();
		}

		private static void PrintSensorData(SensorData data)
		{
			for (int i = 0; i < data.temps.Length; i++)
			{
				Console.WriteLine($"{data.temps[i].name} : {data.temps[i].value}");
			}
			for (int i = 0; i < data.fans.Length; i++)
			{
				Console.WriteLine($"{data.fans[i].name} : {data.fans[i].value}");
			}
			for (int i = 0; i < data.volts.Length; i++)
			{
				Console.WriteLine($"{data.volts[i].name} : {data.volts[i].value}");
			}
		}
	}
}
