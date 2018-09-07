using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sensors.SoftwareInterface
{
	internal class PSAPI
	{
		private const string dll = "psapi.dll";

		[DllImport(dll, SetLastError = true)]
		public static extern bool EnumProcessModulesEx(
			IntPtr hProcess,
			[Out] IntPtr lphModule,
			uint cb,
			[MarshalAs(UnmanagedType.U4)] out UInt32 lpcbNeeded,
			int dwFilterFlag);

		[DllImport(dll)]
		public static extern uint GetModuleFileNameEx(
			IntPtr hProcess,
			IntPtr hModule,
			[Out] StringBuilder lpBaseName,
			[In] [MarshalAs(UnmanagedType.U4)] int nSize);

		[StructLayout(LayoutKind.Sequential)]
		public struct MODULEINFO
		{
			public IntPtr lpBaseOfDll;
			public uint SizeOfImage;
			public IntPtr EntryPoint;
		}

		[DllImport(dll)]
		public static extern bool GetModuleInformation(
			IntPtr hProcess,
			IntPtr hModule,
			out MODULEINFO lpmodinfo,
			uint cb);
	}
}
