using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using _6502Emulator.Instructions;
using _6502Emulator.FancyWrappers;
using System.Drawing;

namespace _6502Emulator
{
    public class Chip
    {
        private PropertyObservationWrapper<byte> XRegister;
        private PropertyObservationWrapper<byte> YRegister;
        private PropertyObservationWrapper<byte> AccumulatorRegister;

        private PropertyObservationWrapper<byte> StackPointer;

        public PropertyObservationWrapper<short> ProgramCounter;

        public Dictionary<FlagType, PropertyObservationWrapper<bool>> Flags = new Dictionary<FlagType, PropertyObservationWrapper<bool>>()
        {
            [FlagType.C] = new PropertyObservationWrapper<bool>(false, new FlagData("Carry flag", FlagType.C)),
            [FlagType.Z] = new PropertyObservationWrapper<bool>(false, new FlagData("Zero flag", FlagType.Z)),
            [FlagType.V] = new PropertyObservationWrapper<bool>(false, new FlagData("Overflow flag", FlagType.V)),
            [FlagType.S] = new PropertyObservationWrapper<bool>(false, new FlagData("Sign flag", FlagType.S)),
            [FlagType.D] = new PropertyObservationWrapper<bool>(false, new FlagData("Decimal flag (base 10 vs base 16 math)", FlagType.D)),
            [FlagType.B] = new PropertyObservationWrapper<bool>(false, new FlagData("Break flag", FlagType.B)),
        };

        private List<BaseInstruction> instructions;
        public int NumberOfInstructions => instructions.Count;

        public Dictionary<short, int> OffsetToIndexMap { get; set; } = new Dictionary<short, int>();
        public Dictionary<int, short> IndexToOffsetMap { get; set; } = new Dictionary<int, short>();

        public bool Finished { get; private set; }
        public Chip(Action<PropertyObservationWrapper<short>, PropertyChangedEventArgs> registerActionShort,
                    Action<PropertyObservationWrapper<byte>, PropertyChangedEventArgs> registerActionByte,
                    Action<PropertyObservationWrapper<bool>, PropertyChangedEventArgs> flagChangedAction,
                    List<BaseInstruction> instructions)
        {
            for (int i = 0; i < instructions.Count; i++)
            {
                OffsetToIndexMap.Add((short)(instructions[i].ByteOffset + Helper.InitialOffset), i);
                IndexToOffsetMap.Add(i, (short)(instructions[i].ByteOffset + Helper.InitialOffset));
            }


            foreach (var kvp in Flags)
            {
                kvp.Value.PropertyChanged += flagChangedAction;
            }

            this.instructions = instructions;

            XRegister = new PropertyObservationWrapper<byte>(0,  "X-Register");
            XRegister.PropertyChanged += registerActionByte;

            YRegister = new PropertyObservationWrapper<byte>(0, "Y-Register");
            YRegister.PropertyChanged += registerActionByte;

            AccumulatorRegister = new PropertyObservationWrapper<byte>(0, "Accumulator Register");
            AccumulatorRegister.PropertyChanged += registerActionByte;

            StackPointer = new PropertyObservationWrapper<byte>(0, "Stack Pointer");
            StackPointer.PropertyChanged += registerActionByte;

            ProgramCounter = new PropertyObservationWrapper<short>(Helper.InitialOffset, "Program Counter");
            ProgramCounter.PropertyChanged += registerActionShort;

        }
        public void EmulateSingleInstruction()
        {
            if (Finished) return;

            var instruction = instructions[OffsetToIndexMap[ProgramCounter.Value]];

            switch (instruction.Type)
            {
                case InstructionType.ADC:

                    switch (instruction.Mode)
                    {
                        case AddressingModes.Immediate:
                            {
                                byte byteValueToAddToAccumulator = instruction.Parameters[0];

                                byte carryFlag = Flags[FlagType.C].Value ? (byte)1 : (byte)0;

                                byte result = (byte)(AccumulatorRegister.Value + byteValueToAddToAccumulator + carryFlag);

                                bool isSeventhBitCarry1 = byteValueToAddToAccumulator >> 7 == 1 && AccumulatorRegister.Value >> 7 == 1;

                                Flags[FlagType.Z].Value = result == 0;
                                Flags[FlagType.S].Value = (result >> 7) == 1;
                                Flags[FlagType.C].Value = isSeventhBitCarry1;
                                Flags[FlagType.V].Value = ((sbyte)AccumulatorRegister.Value).WillAdditionOverflow((sbyte)byteValueToAddToAccumulator + carryFlag);

                                AccumulatorRegister.Value = result;
                            }
                            break;

                        case AddressingModes.ZeroPage:
                            {
                                var address = instruction.Parameters[0];

                                byte byteValueToAddToAccumulator = Computer.Ram[address].Value;

                                byte carryFlag = Flags[FlagType.C].Value ? (byte)1 : (byte)0;

                                byte result = (byte)(AccumulatorRegister.Value + byteValueToAddToAccumulator + carryFlag);

                                bool isSeventhBitCarry1 = byteValueToAddToAccumulator >> 7 == 1 && AccumulatorRegister.Value >> 7 == 1;

                                Flags[FlagType.Z].Value = result == 0;
                                Flags[FlagType.S].Value = (result >> 7) == 1;
                                Flags[FlagType.C].Value = isSeventhBitCarry1;
                                Flags[FlagType.V].Value = ((sbyte)AccumulatorRegister.Value).WillAdditionOverflow((sbyte)byteValueToAddToAccumulator + carryFlag);

                                AccumulatorRegister.Value = result;
                            }
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

                    switch (instruction.Mode)
                    {
                        case AddressingModes.Relative:

                            if (Flags[FlagType.S].Value == true)
                            {
                                ProgramCounter.Value += (short)((sbyte)instruction.Parameters[0]);
                            }
                            break;
                    }

                    break;

                case InstructionType.BNE:

                    switch (instruction.Mode)
                    {
                        case AddressingModes.Relative:

                            if(Flags[FlagType.Z].Value == false)
                            {
                                ProgramCounter.Value += (short)((sbyte)instruction.Parameters[0]);
                            }

                            break;
                    }
                    

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

                    switch (instruction.Mode)
                    {
                        case AddressingModes.Immediate:

                            //A, X, or Y <Memory  --->N = 1, Z = 0, C = 0
                            //A, X, or Y = Memory---> N = 0, Z = 1, C = 1
                            //A, X, or Y > Memory---> N = 0, Z = 0, C = 1

                            var compareTo = instruction.Parameters[0];

                            Flags[FlagType.S].Value = (XRegister.Value < compareTo);
                            Flags[FlagType.Z].Value = (XRegister.Value == compareTo);
                            Flags[FlagType.C].Value = (XRegister.Value >= compareTo);

                            break;
                    }

                    break;

                case InstructionType.CPY:
                    break;
                case InstructionType.DEC:
                    break;

                case InstructionType.DEX:
                
                    switch(instruction.Mode)
                    {
                        case AddressingModes.Implied:

                            Flags[FlagType.S].Value = ((sbyte)XRegister.Value).WillSubtractionUnderflow(1);
                            Flags[FlagType.Z].Value = XRegister.Value - 1 == 0;

                            XRegister.Value -= 1;

                            break;
                    }
                    
                    break;
                
                case InstructionType.DEY:
                    break;
                case InstructionType.EOR:
                    break;
                case InstructionType.INC:
                    break;

                //FINISHED
                case InstructionType.INX:

                    switch (instruction.Mode)
                    {
                        case AddressingModes.Implied:

                            var result = XRegister.Value + 1;

                            Flags[FlagType.V].Value = ((sbyte)XRegister.Value).WillAdditionOverflow(1);
                            Flags[FlagType.S].Value = (result >> 7) == 1;

                            XRegister.Value += 1;

                            break;
                    }

                    break;

                //FINISHED
                case InstructionType.INY:

                    switch (instruction.Mode)
                    {
                        case AddressingModes.Implied:

                            var result = YRegister.Value + 1;
                            Flags[FlagType.Z].Value = ((sbyte)YRegister.Value).WillAdditionOverflow(1);
                            Flags[FlagType.S].Value = (result >> 7) == 1;

                            YRegister.Value += 1;

                            break;
                    }

                    break;

                case InstructionType.JMP:
                    break;

                case InstructionType.JSR:

                    switch (instruction.Mode)
                    {
                        case AddressingModes.JumpLabel:

                            var lowByte = instruction.Parameters[0];
                            var highByte = instruction.Parameters[1];

                            short address = (short)((highByte << 8) + lowByte);

                            break;
                    }

                    break;

                case InstructionType.LDA:

                    switch (instruction.Mode)
                    {
                        case AddressingModes.Immediate:
                            {
                                var value = instruction.Parameters[0];
                                AccumulatorRegister.Value = value;

                                Flags[FlagType.V].Value = ((sbyte)AccumulatorRegister.Value).WillAdditionOverflow(0);
                                Flags[FlagType.S].Value = (AccumulatorRegister.Value >> 7) == 1;
                            }
                            break;

                        case AddressingModes.ZeroPage:
                            {
                                var address = instruction.Parameters[0];

                                AccumulatorRegister.Value = Computer.Ram[address].Value;

                                Flags[FlagType.V].Value = ((sbyte)AccumulatorRegister.Value).WillAdditionOverflow(0);
                                Flags[FlagType.S].Value = (AccumulatorRegister.Value >> 7) == 1;
                            }
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

                case InstructionType.LDX:

                    switch (instruction.Mode)
                    {
                        case AddressingModes.Immediate:
                            {
                                var value = instruction.Parameters[0];
                                XRegister.Value = value;

                                Flags[FlagType.V].Value = ((sbyte)XRegister.Value).WillAdditionOverflow(0);
                                Flags[FlagType.S].Value = (XRegister.Value >> 7) == 1;
                            }
                            break;
                    }

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

                    switch (instruction.Mode)
                    {
                        case AddressingModes.ZeroPage:
                            {
                                var address = instruction.Parameters[0];

                                Computer.Ram[address].Value = AccumulatorRegister.Value;
                            }
                            break;

                        case AddressingModes.ZeroPageX:
                            break;

                        case AddressingModes.Absolute:
                            {
                                var lowByte = instruction.Parameters[0];
                                var highByte = instruction.Parameters[1];

                                short address = (short)((highByte << 8) + lowByte);

                                if (MMIO.AddressToIndex.ContainsKey(address))
                                {
                                    var encodedData = AccumulatorRegister.Value;
                                   
                                    var red = (encodedData >> 5) * 32;
                                    var green = ((encodedData & 28) >> 2) * 32;
                                    var blue = (encodedData & 3) * 64;
                                    
                                    var col = Color.FromArgb(255, red, green, blue);

                                    var index = MMIO.AddressToIndex[address];
                                    var twoDIndex = index.OneToTwoD(MMIO.Bitmap.Width);

                                    MMIO.Bitmap.SetPixel(twoDIndex.X, twoDIndex.Y, col);
                                }   
                            }
                            break;

                        case AddressingModes.AbsoluteX:
                            {
                                var lowByte = instruction.Parameters[0];
                                var highByte = instruction.Parameters[1];

                                short address = (short)((highByte << 8) + lowByte);

                                var offset = XRegister.Value;

                                short final = (short)(address + offset);

                                if (MMIO.AddressToIndex.ContainsKey(final))
                                {
                                    var encodedData = AccumulatorRegister.Value;

                                    var red = (encodedData >> 5) * 32;
                                    var green = ((encodedData & 28) >> 2) * 32;
                                    var blue = (encodedData & 3) * 64;

                                    var col = Color.FromArgb(255, red, green, blue);

                                    var index = MMIO.AddressToIndex[final];
                                    var twoDIndex = index.OneToTwoD(MMIO.Bitmap.Width);

                                    MMIO.Bitmap.SetPixel(twoDIndex.X, twoDIndex.Y, col);
                                }

                                Computer.Ram[address + offset].Value = AccumulatorRegister.Value;
                            }
                            break;

                        case AddressingModes.AbsoluteY:
                            break;

                        case AddressingModes.IndirectX:
                            break;

                        case AddressingModes.IndirectY:
                            break;
                    }

                    break;

                case InstructionType.STX:

                    switch (instruction.Mode)
                    {
                        case AddressingModes.ZeroPage:
                            {
                                var address = instruction.Parameters[0];
                                Computer.Ram[address].Value = XRegister.Value;

                                ProgramCounter.Value += 3;
                            }
                            break;

                        case AddressingModes.ZeroPageY:



                            break;

                        case AddressingModes.Absolute:
                            {
                                var lowByte = instruction.Parameters[0];
                                var highByte = instruction.Parameters[1];

                                short address = (short)((highByte << 8) + lowByte);

                                if (MMIO.AddressToIndex.ContainsKey(address))
                                {
                                    var encodedData = XRegister.Value;

                                    var red = (encodedData >> 5) * 32;
                                    var green = ((encodedData & 28) >> 2) * 32;
                                    var blue = (encodedData & 3) * 64;

                                    var col = Color.FromArgb(255, red, green, blue);

                                    var index = MMIO.AddressToIndex[address];
                                    var twoDIndex = index.OneToTwoD(MMIO.Bitmap.Width);

                                    MMIO.Bitmap.SetPixel(twoDIndex.X, twoDIndex.Y, col);
                                }
                            }
                            break;
                    }

                    break;

                case InstructionType.STY:
                    break;

                case InstructionType.TAX:

                    switch (instruction.Mode)
                    {
                        case AddressingModes.Implied:

                            XRegister.Value = AccumulatorRegister.Value;

                            break;
                    }

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

            //If there is a brach or jump we need to return early with a different index

            var newIndex = OffsetToIndexMap[ProgramCounter.Value] + 1;

            if (IndexToOffsetMap.ContainsKey(newIndex) == false)
            {
                Finished = true;
                return;
            }

            var newval = IndexToOffsetMap[newIndex];

            ProgramCounter.Value = newval;
        }
    }
}
