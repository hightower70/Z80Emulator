using System;
using System.Diagnostics;
using Z80EmuLib;

namespace Excerciser
{
	class ZEXTest
	{
		public void RunTest(string in_file_name)
		{
			TestMemory mem = new TestMemory();

			// load test
			mem.LoadFile(in_file_name, 0x0100);

			// prepare memory for CP/M compatibility
			mem[5] = 0xc9;  // RET
			mem[6] = 0;     // Address for Stack determination (0xc000)
			mem[7] = 0xc0;


			Stopwatch timer = new Stopwatch();
			timer.Start();

			Z80Emu Z80_CPU = new Z80Emu(mem, null, null, true);

			Z80_CPU.Reset();

			Z80_CPU.Registers.PC = 0x0100;
			Z80_CPU.Registers.SP = 0xc000;
			Z80_CPU.TotalTState = 0;

			while (Z80_CPU.Registers.PC != 0)
			{
				// handle BDOS call
				if (Z80_CPU.Registers.PC == 0x0005)
				{
					switch (Z80_CPU.Registers.C)
					{
						case 2:
							Console.Write((char)(Z80_CPU.Registers.E & 0xff));
							break;

						case 9:
							{
								ushort i = Z80_CPU.Registers.DE;

								while (mem[i] != '$')
								{
									Console.Write((char)mem[i]);
									i++;
								}
							}
							break;
					}
				}

				Z80_CPU.Step();
			}

			timer.Stop();
			Console.WriteLine(string.Format("Ellapsed time: {0} ({1} ms) CPU speed: {2}MHz", timer.Elapsed, timer.ElapsedMilliseconds, Z80_CPU.TotalTState / (ulong)timer.ElapsedMilliseconds / 1000));
			Console.WriteLine();
		}
	}
}
