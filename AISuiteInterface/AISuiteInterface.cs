using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AISuite
{
	public class AISuiteInterface
	{
		const int PROCESS_WM_READ = 0x0010;
		const int VOLTS_ADDRESS = 0x09E89800;
		const int TEMPS_ADDRESS = 0x09E8980C;
		const int FANS_ADDRESS = 0x09E89818;
		const int BLOCK_SIZE = 68;
		const int VALUE_OFFSET = 4;
		const int NAME_OFFSET = 36;

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll")]
		public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(IntPtr hObject);

		private static byte[] ReadBytes(int address, int length)
		{
			Process process = Process.GetProcessesByName("AI Suite II")[0];
			IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);

			int bytesRead = 0;
			byte[] buffer = new byte[length];

			ReadProcessMemory((int)processHandle, address, buffer, buffer.Length, ref bytesRead);

			CloseHandle(processHandle);
			return buffer;
		}

		private static Sensor[] GetSensorBlock(int addressPointer)
		{
			byte[] buffer = ReadBytes(addressPointer, 8);
			int count = BitConverter.ToInt32(buffer, 0);
			int address = BitConverter.ToInt32(buffer, 4);

			Sensor[] sensors = new Sensor[count];

			for (int i = 0; i < count; i++, address += BLOCK_SIZE)
			{
				buffer = ReadBytes(address, BLOCK_SIZE);
				sensors[i] = new Sensor();

				String name = Encoding.ASCII.GetString(buffer, NAME_OFFSET, BLOCK_SIZE - NAME_OFFSET);
				name = name.Remove(name.IndexOf('\0'));
				sensors[i].name = name;
				sensors[i].value = BitConverter.ToInt32(buffer, VALUE_OFFSET);
			}

			return sensors;
		}

		public static AISuiteData GetData()
		{
			AISuiteData data = new AISuiteData
			{
				volts = GetSensorBlock(VOLTS_ADDRESS),
				temps = GetSensorBlock(TEMPS_ADDRESS),
				fans = GetSensorBlock(FANS_ADDRESS)
			};

			return data;
		}

		public async Task<object> Invoke(dynamic input)
		{
			return GetData();
		}
	}
}
