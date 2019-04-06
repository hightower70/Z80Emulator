using System;
using System.IO;
using Z80EmuLib;

namespace Excerciser
{
	/// <summary>
	/// Memory provider for CPU tests
	/// </summary>
	class TestMemory : IMemory
	{
		const int MemSize = 0x10000; // size of the memory in bytes

		private Z80Emu m_cpu;

		public byte this[int Address] { get => m_memory[Address]; set => m_memory[Address] = value; }

		public int Size => MemSize;

		private readonly byte[] m_memory = new byte[MemSize];

		public void SetCPU(Z80Emu in_cpu)
		{
			m_cpu = in_cpu;
		}

		public byte Read(ushort in_address, bool in_m1_state)
		{
			return m_memory[in_address];
		}

		public void Write(ushort in_address, byte in_value)
		{
			m_memory[in_address] = in_value;
		}

		public void LoadFile(string name, ushort in_address)
		{
			byte[] data = File.ReadAllBytes(name);

			Array.Copy(data, 0, m_memory, in_address, data.Length);
		}

	}
}
