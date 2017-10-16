using System;

namespace AISuite
{
	public class Sensor
	{
		public string name;
		public Int32 value;
	}

	public class AISuiteData
	{
		public Sensor[] volts;
		public Sensor[] temps;
		public Sensor[] fans;
	}
}
