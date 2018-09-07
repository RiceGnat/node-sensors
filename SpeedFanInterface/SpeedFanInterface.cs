using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpeedFan
{
	public class SpeedFanInterface
	{
		internal const int PROCESS_ALL_ACCESS = 0x1F0FFF;
		internal const int FILE_MAP_READ = 0x0004;

		[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr OpenFileMapping(int dwDesiredAccess,
				bool bInheritHandle, StringBuilder lpName);

		[DllImport("Kernel32.dll")]
		internal static extern IntPtr MapViewOfFile(IntPtr hFileMapping,
				int dwDesiredAccess, int dwFileOffsetHigh, int dwFileOffsetLow,
				int dwNumberOfBytesToMap);

		[DllImport("Kernel32.dll")]
		internal static extern bool UnmapViewOfFile(IntPtr map);

		[DllImport("kernel32.dll")]
		internal static extern bool CloseHandle(IntPtr hObject);

		private static string[] GetNames()
		{
			// Get SpeedFan process
			Process[] processes = Process.GetProcessesByName("speedfan");
		}

		public static SpeedFanSharedMem GetData()
		{
			StringBuilder sharedMemFile = new StringBuilder("SFSharedMemory_ALM");
			IntPtr handle = OpenFileMapping(FILE_MAP_READ, false, sharedMemFile);
			SpeedFanSharedMem sm;
			IntPtr mem = MapViewOfFile(handle, FILE_MAP_READ, 0, 0, Marshal.SizeOf((Type)typeof(SpeedFanSharedMem)));
			if (mem == IntPtr.Zero)
			{
				throw new Exception("Unable to read shared memory.");
			}

			sm = (SpeedFanSharedMem)Marshal.PtrToStructure(mem, typeof(SpeedFanSharedMem));
			UnmapViewOfFile(handle);
			CloseHandle(handle);

			return sm;
		}

		public async Task<object> Invoke(dynamic input)
		{
			return GetData();
		}
	}
}
