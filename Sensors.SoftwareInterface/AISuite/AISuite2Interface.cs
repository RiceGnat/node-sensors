using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sensors.SoftwareInterface.AISuite
{
	public class AISuite2Interface
	{
		const string PROCESS_NAME = "AI Suite II";
		const int PROCESS_ALL_ACCESS = 0x001F0FFF;
		const int LIST_MODULES_32BIT = 0x01;
		const int VOLTS_ADDRESS_OFFSET = 0xB9800;
		const int TEMPS_ADDRESS_OFFSET = 0xB980C;
		const int FANS_ADDRESS_OFFSET = 0xB9818;
		const int BLOCK_SIZE = 68;
		const int VALUE_OFFSET = 4;
		const int NAME_OFFSET = 36;

		private static byte[] ReadBytes(IntPtr processHandle, IntPtr address, int length)
		{
			int bytesRead = 0;
			byte[] buffer = new byte[length];

			Kernel32.ReadProcessMemory(processHandle, address, buffer, buffer.Length, ref bytesRead);
			return buffer;
		}

		private static Sensor[] GetSensorBlock(IntPtr processHandle, IntPtr addressPointer, SensorType type)
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
				sensors[i].value = BitConverter.ToInt32(buffer, VALUE_OFFSET) / (type == SensorType.Temperature ? 10f : (type == SensorType.Voltage ? 1000f : 1));
				sensors[i].type = type;
			}

			return sensors;
		}

		public static SensorData GetData()
		{
			// Get AI Suite process
			Process process = Helper.GetProcess(PROCESS_NAME);
			IntPtr processHandle = Kernel32.OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);

			// Get process modules
			IntPtr baseAddress = IntPtr.Zero;
			IntPtr[] hMods = new IntPtr[1024];
			GCHandle gch = GCHandle.Alloc(hMods, GCHandleType.Pinned);
			try
			{
				IntPtr pModules = gch.AddrOfPinnedObject();
				uint uiSize = (uint)(Marshal.SizeOf(typeof(IntPtr)) * (hMods.Length));
				uint cbNeeded = 0;
				if (!PSAPI.EnumProcessModulesEx(processHandle, pModules, uiSize, out cbNeeded, LIST_MODULES_32BIT))
					throw new Exception("Couldn't access process modules. The calling process probably doesn't have sufficient privileges.");

				// Look for sensor module
				Int32 uiTotalNumberofModules = (Int32)(cbNeeded / Marshal.SizeOf(typeof(IntPtr)));
				for (int i = 0; i < uiTotalNumberofModules; i++)
				{
					StringBuilder lpBaseName = new StringBuilder(1024);
					PSAPI.GetModuleFileNameEx(processHandle, hMods[i], lpBaseName, lpBaseName.Capacity);

					if (Path.GetFileName(lpBaseName.ToString()) == "Sensor.dll")
					{
						//Find sensor module base address
						PSAPI.GetModuleInformation(processHandle, hMods[i], out PSAPI.MODULEINFO info, (uint)Marshal.SizeOf(typeof(PSAPI.MODULEINFO)));
						baseAddress = info.lpBaseOfDll;
						break;
					}
				}
			}
			finally
			{
				gch.Free();
			}

			if (baseAddress == IntPtr.Zero)
				throw new Exception("Couldn't locate AI Suite sensor module. The calling process probably doesn't have sufficient privileges.");

			// Get sensors
			List<Sensor> sensors = new List<Sensor>();
			sensors.AddRange(GetSensorBlock(processHandle, IntPtr.Add(baseAddress, VOLTS_ADDRESS_OFFSET), SensorType.Voltage));
			sensors.AddRange(GetSensorBlock(processHandle, IntPtr.Add(baseAddress, TEMPS_ADDRESS_OFFSET), SensorType.Temperature));
			sensors.AddRange(GetSensorBlock(processHandle, IntPtr.Add(baseAddress, FANS_ADDRESS_OFFSET), SensorType.Fan));

			// Clean up and return data
			Kernel32.CloseHandle(processHandle);
			return new SensorData
			{
				source = "AI Suite II",
				sensors = sensors.ToArray()
			};
		}

		public async Task<object> Invoke(dynamic input)
		{
			return GetData();
		}
	}
}
