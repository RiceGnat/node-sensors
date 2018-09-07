using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedFan;
using AISuite;

namespace TestDriver
{
	class Program
	{
		static void Main(string[] args)
		{
			SpeedFanSharedMem sfData = SpeedFanInterface.GetData();
			Console.WriteLine(String.Join<Int32>(", ", sfData.temps));

			AISuiteData aiData = AISuite2Interface.GetData();

			for (int i = 0; i < aiData.volts.Length; i++)
			{
				Console.WriteLine(aiData.volts[i].name + " : " + aiData.volts[i].value);
			}
			for (int i = 0; i < aiData.temps.Length; i++)
			{
				Console.WriteLine(aiData.temps[i].name + " : " + aiData.temps[i].value);
			}
			for (int i = 0; i < aiData.fans.Length; i++)
			{
				Console.WriteLine(aiData.fans[i].name + " : " + aiData.fans[i].value);
			}

			Console.ReadKey();
		}
	}
}
