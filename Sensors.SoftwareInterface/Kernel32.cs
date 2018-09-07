using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sensors.SoftwareInterface
{
	internal class Kernel32
	{
		private const string dll = "kernel32.dll";

		[DllImport(dll)]
		public static extern IntPtr OpenProcess(
			int dwDesiredAccess,
			bool bInheritHandle,
			int dwProcessId);

		[DllImport(dll)]
		public static extern bool ReadProcessMemory(
			IntPtr hProcess,
			IntPtr lpBaseAddress,
			byte[] lpBuffer,
			int dwSize,
			ref int lpNumberOfBytesRead);

		[DllImport(dll, CharSet = CharSet.Auto)]
		public static extern IntPtr OpenFileMapping(
			int dwDesiredAccess,
			bool bInheritHandle,
			StringBuilder lpName);

		[DllImport(dll)]
		public static extern IntPtr MapViewOfFile(
			IntPtr hFileMapping,
			int dwDesiredAccess,
			int dwFileOffsetHigh,
			int dwFileOffsetLow,
			int dwNumberOfBytesToMap);

		[DllImport(dll)]
		public static extern bool UnmapViewOfFile(IntPtr map);

		[DllImport(dll)]
		public static extern bool CloseHandle(IntPtr hObject);
	}
}
