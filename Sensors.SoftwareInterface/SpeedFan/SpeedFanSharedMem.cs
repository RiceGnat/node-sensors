using System;
using System.Runtime.InteropServices;

namespace Sensors.SoftwareInterface.SpeedFan
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class SpeedFanSharedMem
	{
		ushort version;
		ushort flags;
		Int32 size;
		Int32 handle;
		ushort numTemps;
		ushort numFans;
		ushort numVolts;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public Int32[] temps;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public Int32[] fans;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public Int32[] volts;

		public SpeedFanSharedMem()
		{
			temps = new Int32[32];
			fans = new Int32[32];
			volts = new Int32[32];
		}
	}
}
