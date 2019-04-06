namespace Excerciser
{
	class Program
	{
		static void Main(string[] args)
		{
			FuseTest fuse_test = new FuseTest();

			fuse_test.RunTest(@"..\..\..\Test\tests.in", @"..\..\..\Test\tests.expected");

			ZEXTest zex_test = new ZEXTest();

			zex_test.RunTest(@"..\..\..\Test\zexdoc.com");

			zex_test.RunTest(@"..\..\..\Test\zexall.com");
		}
	}
}















#if false
Z80Cpu cpu = new Z80Cpu(mem, new Port());
			cpu.Registers.PC = 0x0100;
			cpu.Registers.SP = 0xc000;

			while (cpu.Registers.PC != 0)
			{
				if (cpu.Registers.PC == 0xc000)
				{
					switch (cpu.Registers.BC & 0xff)
					{
						case 2:
							Console.Write((char)(cpu.Registers.DE & 0xff));
							break;

						case 9:
							{
								ushort i = cpu.Registers.DE;

								while (mem.Read(i) != '$')
								{
									Console.Write((char)mem.Read(i));
									i++;
								}
							}
							break;
					}
				}

				cpu.ExecuteCpuCycle();
			}
		}
	}
}

/*
uP proc = new uP(mem, null);


proc.Status.PC = 0x0100;
proc.Status.SP = 0xc000;

mem.WriteByte(5, 0xcd);
mem.WriteByte(6, 0);
mem.WriteByte(7, 0xc0);
mem.WriteByte(8, 0xc9);

mem.WriteByte(0xc000, 0xc9);

while(proc.Status.PC != 0)
{
if (proc.Status.PC == 0xc000)
{
switch (proc.Status.BC & 0xff)
{
case 2:
Console.Write((char)(proc.Status.DE & 0xff));
break;

case 9:
{
 ushort i = proc.Status.DE;

 while (mem[i] != '$')
 {
	 Console.Write((char)mem[i]);
	 i++;
 }
}
break;
}
}


//proc.event_next_event = proc.tstates + 1;
proc.StatementsToFetch = 1;
proc.Execute();
}
*/


/*

Z80 Z80_CPU = new Z80(mem, null, null, true);
Z80_CPU.Reset();

Z80_CPU.Registers.PC = 0x0100;
Z80_CPU.Registers.SP = 0xc000;

mem.Write(Z80_CPU, 6, 0);
mem.Write(Z80_CPU, 7, 0xc0);


while (Z80_CPU.Registers.PC != 0)
{
Z80_CPU.Step();
}

	 */
#endif