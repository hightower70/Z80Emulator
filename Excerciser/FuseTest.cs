using System;
using System.Globalization;
using System.IO;
using Z80EmuLib;

namespace Excerciser
{
	class FuseTest
	{
		private string m_test_name;

		private void ReadRegisters(string in_line, Z80Registers out_registers)
		{
			out_registers.AF = ushort.Parse(in_line.Substring(0, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
			out_registers.BC = ushort.Parse(in_line.Substring(5, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
			out_registers.DE = ushort.Parse(in_line.Substring(10, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
			out_registers.HL = ushort.Parse(in_line.Substring(15, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
			out_registers._AF_ = ushort.Parse(in_line.Substring(20, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
			out_registers._BC_ = ushort.Parse(in_line.Substring(25, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
			out_registers._DE_ = ushort.Parse(in_line.Substring(30, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
			out_registers._HL_ = ushort.Parse(in_line.Substring(35, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
			out_registers.IX = ushort.Parse(in_line.Substring(40, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
			out_registers.IY = ushort.Parse(in_line.Substring(45, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
			out_registers.SP = ushort.Parse(in_line.Substring(50, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
			out_registers.PC = ushort.Parse(in_line.Substring(55, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
			out_registers.WZ = ushort.Parse(in_line.Substring(60, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
		}

		public void RunTest(string in_file_name, string expected_file_name)
		{
			TestMemory mem = new TestMemory();
			TestPorts port = new TestPorts();
			Z80Emu cpu = new Z80Emu(mem, port, null, true);
			uint planned_t_state;
			uint ellapsed_t_state;
			int failed_tests = 0;

			Z80Registers expected_registers = new Z80Registers();

			string line;
			using (var expected_file = File.OpenText(expected_file_name))
			{
				using (var in_file = File.OpenText(in_file_name))
				{
					while ((line = in_file.ReadLine()) != null)
					{
						if (string.IsNullOrEmpty(line.Trim()))
							continue;

						// reset CPU
						cpu.Reset();

						// test found read name
						m_test_name = line;

						// read registers
						line = in_file.ReadLine();
						ReadRegisters(line, cpu.Registers);

						// read other registers and tsatate
						line = in_file.ReadLine();
						cpu.Registers.I = byte.Parse(line.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
						cpu.Registers.R = byte.Parse(line.Substring(3, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
						cpu.IFF1 = (byte)(line[6] - '0');
						cpu.IFF2 = (byte)(line[8] - '0');
						cpu.IM = (Z80Emu.IMMode)(line[10] - '0');
						cpu.Halted = line[12] != '0';

						planned_t_state = uint.Parse(line.Substring(13).Trim());

						// read memory
						do
						{
							line = in_file.ReadLine();
							if (line != "-1")
								ParseMemoryData(line, mem);
						} while (line != "-1");

						// start simulation
						Console.Write("Running test {0}", m_test_name);
						ellapsed_t_state = 0;

						while (ellapsed_t_state < planned_t_state || !cpu.InstructionDone)
						{
							ellapsed_t_state += cpu.Step();
						}

						// compare result with the expected

						// compare test names
						if (m_test_name != expected_file.ReadLine())
							Console.WriteLine("Unmatched test name");

						// skip  events
						do
						{
							line = expected_file.ReadLine();
						} while (line[0] == ' ');

						// load registers
						ReadRegisters(line, expected_registers);

						// compare cpu state
						line = expected_file.ReadLine();

						byte temp;
						bool ok = true;

						temp = byte.Parse(line.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
						if (temp != cpu.Registers.I)
						{
							Console.WriteLine(string.Format("\n  Register mismatch I={0:X2} -- expected {1:X2}", cpu.Registers.I, temp));
							ok = false;
						}

						temp = byte.Parse(line.Substring(3, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
						if (temp != cpu.Registers.R)
						{
							Console.WriteLine(string.Format("\n  Register mismatch R={0:X2} -- expected {1:X2}", cpu.Registers.R, temp));
							ok = false;
						}

						// check IFF1
						temp = (byte)(line[6] - '0');
						if (cpu.IFF1 != temp)
						{
							Console.WriteLine(string.Format("\n  IFF1 flag mismatch {0} -- expected {1}", cpu.IFF1, temp));
							ok = false;
						}

						// check IFF2
						temp = (byte)(line[8] - '0');
						if (cpu.IFF2 != temp)
						{
							Console.WriteLine(string.Format("\n  IFF2 flag mismatch {0} -- expected {1}", cpu.IFF2, temp));
							ok = false;
						}

						// check IM
						Z80Emu.IMMode im_mode = (Z80Emu.IMMode)(line[10] - '0');
						if (cpu.IM != im_mode)
						{
							Console.WriteLine(string.Format("\n  IM mode mismatch {0} -- expected {1}", cpu.IM, im_mode));
							ok = false;
						}

						// check halted
						bool halted = line[12] != '0';
						if (cpu.Halted != halted)
						{
							Console.WriteLine(string.Format("\n  Halted mode mismatch {0} -- expected {1}", cpu.Halted, halted));
							ok = false;
						}

						// check T state
						uint expected_t_state = uint.Parse(line.Substring(13).Trim());
						if (expected_t_state != ellapsed_t_state)
						{
							Console.WriteLine(string.Format("\n  T state mismathed T={0:d} -- expected {1:d}", ellapsed_t_state, expected_t_state));
							ok = false;
						}

						// compare registers
						ok = CompareRegisters(ok, (ushort)(cpu.Registers.AF), (ushort)(expected_registers.AF), "AF");
						ok = CompareRegisters(ok, cpu.Registers.BC, expected_registers.BC, "BC");
						ok = CompareRegisters(ok, cpu.Registers.DE, expected_registers.DE, "DE");
						ok = CompareRegisters(ok, cpu.Registers.HL, expected_registers.HL, "HL");
						ok = CompareRegisters(ok, cpu.Registers._AF_, expected_registers._AF_, "_AF_");
						ok = CompareRegisters(ok, cpu.Registers._BC_, expected_registers._BC_, "_BC_");
						ok = CompareRegisters(ok, cpu.Registers._DE_, expected_registers._DE_, "_DE_");
						ok = CompareRegisters(ok, cpu.Registers._HL_, expected_registers._HL_, "_HL_");

						ok = CompareRegisters(ok, cpu.Registers.IX, expected_registers.IX, "IX");
						ok = CompareRegisters(ok, cpu.Registers.IY, expected_registers.IY, "IY");
						ok = CompareRegisters(ok, cpu.Registers.SP, expected_registers.SP, "SP");
						ok = CompareRegisters(ok, cpu.Registers.PC, expected_registers.PC, "PC");
						ok = CompareRegisters(ok, cpu.Registers.WZ, expected_registers.WZ, "WZ");

						do
						{
							line = expected_file.ReadLine();
							if (!string.IsNullOrEmpty(line))
							{
								// check memory
								ok = CheckMemoryData(ok, line, mem);
							}

						} while (!string.IsNullOrEmpty(line));


						if (ok)
							Console.WriteLine(" -- OK");
						else
							failed_tests++;
					}

					Console.WriteLine("Tests failed: " + failed_tests.ToString());
					Console.WriteLine();
				}
			}
		}

		private bool CompareRegisters(bool in_ok, ushort in_sim_reg, ushort in_expected_reg, string in_name)
		{
			if (in_sim_reg != in_expected_reg)
			{
				Console.WriteLine(string.Format("\n  Register mismathed {0}={1:X4} -- expected {2:X4}", in_name, in_sim_reg, in_expected_reg));
				in_ok = false;
			}

			return in_ok;
		}


		private void ParseMemoryData(string in_line, TestMemory in_memory)
		{
			string line = in_line.Trim();

			int address = int.Parse(in_line.Substring(0, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);

			int pos = 5;
			int value = 0;
			while (value >= 0)
			{
				if (line[pos] == '-')
					value = -1;
				else
				{
					value = int.Parse(line.Substring(pos, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
					pos += 3;

					in_memory[address] = (byte)value;
					address++;
				}
			}
		}

		private bool CheckMemoryData(bool in_ok, string in_line, TestMemory in_memory)
		{
			string line = in_line.Trim();

			int address = int.Parse(in_line.Substring(0, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);

			int pos = 5;
			int value = 0;
			while (value >= 0)
			{
				if (line[pos] == '-')
					value = -1;
				else
				{
					value = int.Parse(line.Substring(pos, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
					pos += 3;

					if(in_memory[address] != (byte)value)
					{
						Console.WriteLine(string.Format("\n  Memory mismathed {0:X4}={1:X2} -- expected {2:X2}", address, in_memory[address], value));
						in_ok = false;
					}

					address++;
				}
			}

			return in_ok;
		}
	}
}
