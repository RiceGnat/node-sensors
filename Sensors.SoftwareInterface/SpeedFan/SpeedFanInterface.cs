using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sensors.SoftwareInterface.SpeedFan
{
	public class SpeedFanInterface
	{
		internal const int PROCESS_ALL_ACCESS = 0x1F0FFF;
		internal const int FILE_MAP_READ = 0x0004;

		public static SensorData GetData()
		{
			// Read sensor values from shared memory
			StringBuilder sharedMemFile = new StringBuilder("SFSharedMemory_ALM");
			IntPtr handle = Kernel32.OpenFileMapping(FILE_MAP_READ, false, sharedMemFile);
			SpeedFanSharedMem sm;
			IntPtr mem = Kernel32.MapViewOfFile(handle, FILE_MAP_READ, 0, 0, Marshal.SizeOf(typeof(SpeedFanSharedMem)));
			if (mem == IntPtr.Zero)
			{
				throw new Exception("Unable to read shared memory. SpeedFan might not be running.");
			}

			sm = (SpeedFanSharedMem)Marshal.PtrToStructure(mem, typeof(SpeedFanSharedMem));
			Kernel32.UnmapViewOfFile(handle);
			Kernel32.CloseHandle(handle);

			SensorData data = new SensorData();

			// Try to get sensor names from SpeedFan config file
			try
			{
				// Get SpeedFan process
				Process process = Helper.GetProcess("speedfan");

				// Get config file path
				string sfDir = Path.GetDirectoryName(process.MainModule.FileName);
				string cfgPath = Path.Combine(sfDir, "speedfansens.cfg");

				string text = File.ReadAllText(cfgPath);

				// Get sensor chips
				Dictionary<string, string> sources = new Dictionary<string, string>();
				Regex rx = new Regex(@"xxx Sensor UniqueID=(.*)\n((?:.+=.+\n)+)xxx end");
				Regex nameRx = new Regex(@"name=(.*)", RegexOptions.IgnoreCase);
				foreach (Match m in rx.Matches(text))
				{
					sources[m.Groups[1].Value.TrimEnd()] = nameRx.Match(m.Groups[2].Value).Groups[1].Value.TrimEnd();
				}

				// Map sensor names to values
				Dictionary<string, List<Sensor>> sensors = new Dictionary<string, List<Sensor>>();
				rx = new Regex(@"xxx (Fan|Temp|Volt) \d+ from (.*)\n((?:.+=.+\n)+)xxx end", RegexOptions.IgnoreCase);
				foreach (Match m in rx.Matches(text))
				{
					string type = m.Groups[1].Value.TrimEnd().ToLowerInvariant();
					string source = m.Groups[2].Value.TrimEnd();
					if (!sensors.ContainsKey(type)) sensors[type] = new List<Sensor>();

					int value = 0;
					switch (type)
					{
						case "temp":
							value = sm.temps[sensors[type].Count];
							break;
						case "fan":
							value = sm.fans[sensors[type].Count];
							break;
						case "volt":
							value = sm.volts[sensors[type].Count];
							break;
					}

					SpeedFanSensor s = new SpeedFanSensor
					{
						name = nameRx.Match(m.Groups[3].Value).Groups[1].Value.TrimEnd(),
						value = value,
						chip = $"{sources[source]} ({source})"
					};

					sensors[type].Add(s);
				}

				data.temps = sensors["temp"].ToArray();
				data.fans = sensors["fan"].ToArray();
				data.volts = sensors["volt"].ToArray();
			}
			catch (Exception ex)
			{
				throw new Exception("Couldn't get sensor names from SpeedFan config. The calling process probably doesn't have sufficient privileges.", ex);
			}

			return data;
		}

		public async Task<object> Invoke(dynamic input)
		{
			return GetData();
		}
	}
}
