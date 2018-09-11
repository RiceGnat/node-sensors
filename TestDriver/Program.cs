using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sensors.SoftwareInterface.SpeedFan;
using Sensors.SoftwareInterface.AISuite;
using Sensors.SoftwareInterface.OpenHardwareMonitor;

namespace Sensors.SoftwareInterface.TestDriver
{
	class Program
	{
		static void Main(string[] args)
		{
			PrintSensorData(SpeedFanInterface.GetData());

			Console.WriteLine();
			PrintSensorData(AISuite2Interface.GetData());

			Console.WriteLine();
			PrintSensorData(OpenHardwareMonitorInterface.GetData());

			Console.ReadKey();
		}

		private static void PrintSensorData(SensorData data)
		{
			Console.WriteLine($"{data.source} data");
			for (int i = 0; i < data.sensors.Length; i++)
			{
				Console.WriteLine($"({data.sensors[i].type}) {data.sensors[i].name} : {data.sensors[i].value}");
			}
		}
	}
}
