using System;
using System.Diagnostics;

namespace Sensors.SoftwareInterface
{
	internal class Helper
	{
		public static Process GetProcess(string name)
		{
			Process[] processes = Process.GetProcessesByName(name);
			if (processes.Length == 0)
				throw new Exception($"Couldn't find the {name}.exe process.");
			return processes[0];
		}
	}
}
