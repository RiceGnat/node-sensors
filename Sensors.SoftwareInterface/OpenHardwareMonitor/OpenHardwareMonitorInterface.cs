using System;
using System.Collections.Generic;
using System.Management;
using System.Threading.Tasks;

namespace Sensors.SoftwareInterface.OpenHardwareMonitor
{
	public class OpenHardwareMonitorInterface
	{
		public static SensorData GetData()
		{
			ManagementScope scope = new ManagementScope("root\\OpenHardwareMonitor");
			SelectQuery q = new SelectQuery("Hardware");
			ManagementObjectSearcher mgmtObjSearcher = new ManagementObjectSearcher(scope, q);

			Dictionary<string, ManagementObject> hardware = new Dictionary<string, ManagementObject>();
			foreach (ManagementObject h in mgmtObjSearcher.Get())
			{
				hardware[h["Identifier"].ToString()] = h;
			}

			q = new SelectQuery("Sensor");
			mgmtObjSearcher = new ManagementObjectSearcher(scope, q);
			List<Sensor> sensors = new List<Sensor>();
			foreach (ManagementObject s in mgmtObjSearcher.Get())
			{
				if (Enum.TryParse(s["SensorType"].ToString(), out SensorType type))
				sensors.Add(new SpeedFan.SpeedFanSensor
				{
					name = s["Name"].ToString(),
					type = type,
					value = (float)s["Value"],
					chip = hardware[s["Parent"].ToString()]["Name"].ToString()
				});
			}

			return new SensorData
			{
				source = "Open Hardware Monitor",
				sensors = sensors.ToArray()
			};
		}

		public async Task<object> Invoke(dynamic input)
		{
			return GetData();
		}
	}
}
