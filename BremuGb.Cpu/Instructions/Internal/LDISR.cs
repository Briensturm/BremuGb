using BremuGb.Memory;
using BremuGb.Common.Constants;

namespace BremuGb.Cpu.Instructions
{
    public class LDISR : InstructionBase
    {
        private readonly byte _readyInterrupts;
        protected override int InstructionLength => 5;

        public LDISR(byte readyInterrupts)
        {
            _readyInterrupts = readyInterrupts;
        }

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch(_remainingCycles)
            {
                case 5:
                    //Console.WriteLine("Loading interrupt service routine...");
                    break;
                case 3:
                    mainMemory.WriteByte(--cpuState.StackPointer, (byte)(cpuState.ProgramCounter >> 8));
                    break;
                case 2:
                    mainMemory.WriteByte(--cpuState.StackPointer, (byte)(cpuState.ProgramCounter & 0x00FF));
                    break;
                case 1:
                    var interruptFlags = mainMemory.ReadByte(MiscRegisters.InterruptFlags);

                    //vblank interrupt
                    if ((_readyInterrupts & 0x01) == 0x01)
                    {
                        //Console.WriteLine("Loading vblank isr...");

                        cpuState.ProgramCounter = Interrupts.VblankInterrupt;

                        mainMemory.WriteByte(MiscRegisters.InterruptFlags, (byte)(interruptFlags & 0xFE));
                    }

                    //lcd stat interrupt
                    else if ((_readyInterrupts & 0x02) == 0x02)
                    {
                        //Console.WriteLine("Loading lcd stat isr...");

                        cpuState.ProgramCounter = Interrupts.LcdInterrupt;

                        //clear interrupt flag
                        mainMemory.WriteByte(MiscRegisters.InterruptFlags, (byte)(interruptFlags & 0xFD));
                    }
                    
                    //timer interrupt
                    else if ((_readyInterrupts & 0x04) == 0x04)
                    {
                        //Console.WriteLine("Loading timer isr...");

                        cpuState.ProgramCounter = Interrupts.TimerInterrupt;

                        //clear interrupt flag
                        mainMemory.WriteByte(MiscRegisters.InterruptFlags, (byte)(interruptFlags & 0xFB));
                    }

                    //serial interrupt
                    else if ((_readyInterrupts & 0x08) == 0x08)
                    {
                        cpuState.ProgramCounter = Interrupts.SerialInterrupt;

                        //clear interrupt flag
                        mainMemory.WriteByte(MiscRegisters.InterruptFlags, (byte)(interruptFlags & 0xF7));
                    }

                    //joypad interrupt
                    else if ((_readyInterrupts & 0x10) == 0x10)
                    {
                        cpuState.ProgramCounter = Interrupts.JoypadInterrupt;

                        //clear interrupt flag
                        mainMemory.WriteByte(MiscRegisters.InterruptFlags, (byte)(interruptFlags & 0x0F));
                    }

                    cpuState.InterruptMasterEnable = false;
                    break;
            }            

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}
