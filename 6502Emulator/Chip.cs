using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using _6502Emulator.Instructions;
using _6502Emulator.FancyWrappers;

namespace _6502Emulator
{
    public class Chip
    {
        private FancyRegister<byte> XRegister;
        private FancyRegister<byte> YRegister;
        private FancyRegister<byte> AccumulatorRegister;
                              
        private FancyRegister<byte> StackPointer;
        private FancyRegister<short> ProgramCounter;

        public Dictionary<FlagType, FancyFlag> Flags = new Dictionary<FlagType, FancyFlag>()
        {
            [FlagType.C] = new FancyFlag("Carry flag", FlagType.C),
            [FlagType.Z] = new FancyFlag("Zero flag", FlagType.Z),
            [FlagType.V] = new FancyFlag("Overflow flag", FlagType.V),
            [FlagType.S] = new FancyFlag("Sign flag", FlagType.S),
            [FlagType.D] = new FancyFlag("Decimal flag (base 10 vs base 16 math)", FlagType.D),
            [FlagType.B] = new FancyFlag("Break flag", FlagType.B),
        };

        private List<BaseInstruction> instructions;

        public int NumberOfInstructions => instructions.Count;
        public Chip(Action<FancyRegister<short>, PropertyChangedEventArgs> registerActionShort,
                    Action<FancyRegister<byte>, PropertyChangedEventArgs> registerActionByte,
                    Action<FancyFlag, PropertyChangedEventArgs> flagChangedAction,
                    List<BaseInstruction> instructions)
        {
            foreach(var kvp in Flags)
            {
                kvp.Value.PropertyChanged += flagChangedAction;
            }

            this.instructions = instructions;

            XRegister = new FancyRegister<byte>(0,  "X-Register");
            XRegister.PropertyChanged += registerActionByte;

            YRegister = new FancyRegister<byte>(0, "Y-Register");
            YRegister.PropertyChanged += registerActionByte;

            AccumulatorRegister = new FancyRegister<byte>(0, "Accumulator Register");
            AccumulatorRegister.PropertyChanged += registerActionByte;

            StackPointer = new FancyRegister<byte>(0, "Stack Pointer");
            StackPointer.PropertyChanged += registerActionByte;

            ProgramCounter = new FancyRegister<short>(Helper.InitialOffset, "Program Counter");
            ProgramCounter.PropertyChanged += registerActionShort;

        }

        public void Emulate()
        {
            for(int i = 0; i < instructions.Count; i++)
            {
                EmulateSingleInstruction(i);
            }
        }

        public void EmulateSingleInstruction(int index)
        {
            var instruction = instructions[index];

            switch (instruction.Type)
            {
                case InstructionType.ADC:

                    switch (instruction.Mode)
                    {
                        case AddressingModes.Immediate:

                            byte byteValueToAddToAccumulator = instruction.Parameters[0];

                            byte carryFlag = Flags[FlagType.C].HasValue ? 1 : 0;

                            byte result = (byte)(AccumulatorRegister.Value + byteValueToAddToAccumulator + carryFlag);

                            bool isSeventhBitCarry1 = byteValueToAddToAccumulator >> 7 == 1 && AccumulatorRegister.Value >> 7 == 1;
                            bool isSixthBitCarry1 = byteValueToAddToAccumulator >> 6 == 1 && AccumulatorRegister.Value >> 6 == 1;

                            Flags[FlagType.Z].HasValue = result == 0;
                            Flags[FlagType.S].HasValue = (result >> 7) == 1;
                            Flags[FlagType.C].HasValue = isSeventhBitCarry1;
                            Flags[FlagType.V].HasValue = isSeventhBitCarry1 ^ isSixthBitCarry1;

                            AccumulatorRegister.Value = result;

                            ProgramCounter.Value += 3;

                            break;

                        case AddressingModes.ZeroPage:
                            break;
                        case AddressingModes.ZeroPageX:
                            break;
                        case AddressingModes.Absolute:
                            break;
                        case AddressingModes.AbsoluteX:
                            break;
                        case AddressingModes.AbsoluteY:
                            break;
                        case AddressingModes.IndirectX:
                            break;
                        case AddressingModes.IndirectY:
                            break;
                    }

                    break;




                case InstructionType.AND:
                    break;
                case InstructionType.ASL:
                    break;
                case InstructionType.BCC:
                    break;
                case InstructionType.BCS:
                    break;
                case InstructionType.BEQ:
                    break;
                case InstructionType.BIT:
                    break;
                case InstructionType.BMI:
                    break;
                case InstructionType.BNE:
                    break;
                case InstructionType.BPL:
                    break;
                case InstructionType.BRK:
                    break;
                case InstructionType.BVC:
                    break;
                case InstructionType.BVS:
                    break;
                case InstructionType.CLC:
                    break;
                case InstructionType.CLD:
                    break;
                case InstructionType.CLI:
                    break;
                case InstructionType.CLV:
                    break;
                case InstructionType.CMP:
                    break;
                case InstructionType.CPX:
                    break;
                case InstructionType.CPY:
                    break;
                case InstructionType.DEC:
                    break;
                case InstructionType.DEX:
                    break;
                case InstructionType.DEY:
                    break;
                case InstructionType.EOR:
                    break;
                case InstructionType.INC:
                    break;
                case InstructionType.INX:
                    break;
                case InstructionType.INY:
                    break;
                case InstructionType.JMP:
                    break;
                case InstructionType.JSR:
                    break;
                case InstructionType.LDA:
                    break;
                case InstructionType.LDX:
                    break;
                case InstructionType.LDY:
                    break;
                case InstructionType.LSR:
                    break;
                case InstructionType.NOP:
                    break;
                case InstructionType.ORA:
                    break;
                case InstructionType.PHA:
                    break;
                case InstructionType.PHP:
                    break;
                case InstructionType.PLA:
                    break;
                case InstructionType.PLP:
                    break;
                case InstructionType.ROL:
                    break;
                case InstructionType.ROR:
                    break;
                case InstructionType.RTI:
                    break;
                case InstructionType.RTS:
                    break;
                case InstructionType.SBC:
                    break;
                case InstructionType.SEC:
                    break;
                case InstructionType.SED:
                    break;
                case InstructionType.SEI:
                    break;
                case InstructionType.STA:
                    break;
                case InstructionType.STX:
                    break;
                case InstructionType.STY:
                    break;
                case InstructionType.TAX:
                    break;
                case InstructionType.TAY:
                    break;
                case InstructionType.TSX:
                    break;
                case InstructionType.TXA:
                    break;
                case InstructionType.TXS:
                    break;
                case InstructionType.TYA:
                    break;
            }
        }
    }
}
