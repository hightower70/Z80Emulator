using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80EmuLib;

namespace Excerciser
{
	/// <summary>
	/// Ports provider for Fuse tests
	/// </summary>
	class TestPorts : IPort
	{
		private Z80Emu m_cpu;

		public void SetCPU(Z80Emu in_cpu)
		{
			m_cpu = in_cpu;
		}

		public byte Read(ushort in_address)
		{
			return (byte)(in_address >> 8);
		}

		public void Write(ushort in_address, byte in_value)
		{

		}
	}

}
