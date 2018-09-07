using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AISuite
{
	public class AISuite2Interface
	{
		const string PROCESS_NAME = "AI Suite II";
		const int PROCESS_WM_READ = 0x0010;
		const int VOLTS_ADDRESS_OFFSET = 0xB9800;
		const int TEMPS_ADDRESS_OFFSET = 0xB980C;
		const int FANS_ADDRESS_OFFSET = 0xB9818;
		const int BLOCK_SIZE = 68;
		const int VALUE_OFFSET = 4;
		const int NAME_OFFSET = 36;

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll")]
		public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(IntPtr hObject);

		private static byte[] ReadBytes(IntPtr processHandle, IntPtr address, int length)
		{
			int bytesRead = 0;
			byte[] buffer = new byte[length];

			ReadProcessMemory((int)processHandle, (int)address, buffer, buffer.Length, ref bytesRead);
			return buffer;
		}

		private static Sensor[] GetSensorBlock(IntPtr processHandle, IntPtr addressPointer)
		{
			// Get size and address of block
			byte[] buffer = ReadBytes(processHandle, addressPointer, 8);
			int count = BitConverter.ToInt32(buffer, 0);
			IntPtr address = new IntPtr(BitConverter.ToInt32(buffer, 4));

			// Read sensors
			Sensor[] sensors = new Sensor[count];
			for (int i = 0; i < count; i++, address = IntPtr.Add(address, BLOCK_SIZE))
			{
				buffer = ReadBytes(processHandle, address, BLOCK_SIZE);
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
			// Get AI Suite process
			Process[] processes = Process.GetProcessesByName("AI Suite II");
			if (processes.Length == 0)
				throw new Exception($"Couldn't find the \"{PROCESS_NAME}.exe\" process. Check that AI Suite is running.");
			Process process = processes[0];
			IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);

			// Get process modules
			ProcessModule bClient;
			ProcessModuleCollection bModules;
			try
			{
				bModules = process.Modules;
			}
			catch (System.ComponentModel.Win32Exception ex)
			{
				throw new Exception("Couldn't access process modules. The calling process probably doesn't have sufficient privileges.", ex);
			}

			//Find sensor module base address
			IntPtr baseAddress = IntPtr.Zero;
			for (int i = 0; i < bModules.Count; i++)
			{
				bClient = bModules[i];
				if (bClient.ModuleName == "Sensor.dll")
				{
					baseAddress = bClient.BaseAddress;
					break;
				}
			}
			if (baseAddress == IntPtr.Zero)
				throw new Exception("Couldn't locate AI Suite sensor module");

			// Get sensors
			AISuiteData data = new AISuiteData
			{
				volts = GetSensorBlock(processHandle, IntPtr.Add(baseAddress, VOLTS_ADDRESS_OFFSET)),
				temps = GetSensorBlock(processHandle, IntPtr.Add(baseAddress, TEMPS_ADDRESS_OFFSET)),
				fans = GetSensorBlock(processHandle, IntPtr.Add(baseAddress, FANS_ADDRESS_OFFSET))
			};

			// Clean up and return data
			CloseHandle(processHandle);
			return data;
		}

		public async Task<object> Invoke(dynamic input)
		{
			return GetData();
		}
	}
}
