using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedFan;

namespace SpeedFanInterfaceDriver
{
	class Program
	{
		static void Main(string[] args)
		{
			SpeedFanSharedMem data = SpeedFanInterface.GetData();
			Console.WriteLine(String.Join<Int32>(", ", data.temps));
			Console.ReadKey();
		}
	}
}
