using _6502Emulator.Instructions;
using _6502Emulator.Instructions.Arthimetic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _6502Emulator
{
    public static class Helper
    {
        public static Dictionary<int, char> IntToHexMap = new Dictionary<int, char>()
        {
            [0] = '0',
            [1] = '1',
            [2] = '2',
            [3] = '3',
            [4] = '4',
            [5] = '5',
            [6] = '6',
            [7] = '7',
            [8] = '8',
            [9] = '9',
            [10] = 'A',
            [11] = 'B',
            [12] = 'C',
            [13] = 'D',
            [14] = 'E',
            [15] = 'F'
        };

        public static Dictionary<char, int> HexToIntMap = new Dictionary<char, int>()
        {
            ['0'] = 0,
            ['1'] = 1,
            ['2'] = 2,
            ['3'] = 3,
            ['4'] = 4,
            ['5'] = 5,
            ['6'] = 6,
            ['7'] = 7,
            ['8'] = 8,
            ['9'] = 9,
            ['A'] = 10,
            ['B'] = 11,
            ['C'] = 12,
            ['D'] = 13,
            ['E'] = 14,
            ['F'] = 15,
            ['a'] = 10,
            ['b'] = 11,
            ['c'] = 12,
            ['d'] = 13,
            ['e'] = 14,
            ['f'] = 15
        };

        public static Dictionary<InstructionType, Dictionary<AddressingModes, byte>> InstructionTypeToOpCode = new Dictionary<InstructionType, Dictionary<AddressingModes, byte>>()
        {
            #region BitManipulation
            [InstructionType.ASL] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Accumulator] = 0x0A,
                [AddressingModes.ZeroPage] = 0x06,
                [AddressingModes.ZeroPageX] = 0x16,
                [AddressingModes.Absolute] = 0x0E,
                [AddressingModes.AbsoluteX] = 0x1E,
            },
            [InstructionType.LSR] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Accumulator] = 0x4A,
                [AddressingModes.ZeroPage] = 0x46,
                [AddressingModes.ZeroPageX] = 0x56,
                [AddressingModes.Absolute] = 0x4E,
                [AddressingModes.AbsoluteX] = 0x5E,
            },
            [InstructionType.ROL] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Accumulator] = 0x2A,
                [AddressingModes.ZeroPage] = 0x26,
                [AddressingModes.ZeroPageX] = 0x36,
                [AddressingModes.Absolute] = 0x2E,
                [AddressingModes.AbsoluteX] = 0x3E,
            },
            [InstructionType.ROR] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Accumulator] = 0x6A,
                [AddressingModes.ZeroPage] = 0x66,
                [AddressingModes.ZeroPageX] = 0x76,
                [AddressingModes.Absolute] = 0x6E,
                [AddressingModes.AbsoluteX] = 0x7E,
            },
            #endregion
            #region Arthimetic
            [InstructionType.ADC] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Immediate] = 0x69,
                [AddressingModes.ZeroPage] = 0x65,
                [AddressingModes.ZeroPageX] = 0x75,
                [AddressingModes.Absolute] = 0x6D,
                [AddressingModes.AbsoluteX] = 0x7D,
                [AddressingModes.AbsoluteY] = 0x79,
                [AddressingModes.IndirectX] = 0x61,
                [AddressingModes.IndirectY] = 0x71,
            },
            [InstructionType.DEC] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.ZeroPage] = 0xC6,
                [AddressingModes.ZeroPageX] = 0xD6,
                [AddressingModes.Absolute] = 0xCE,
                [AddressingModes.AbsoluteX] = 0xDE
            },
            [InstructionType.INC] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.ZeroPage] = 0xE6,
                [AddressingModes.ZeroPageX] = 0xF6,
                [AddressingModes.Absolute] = 0xEE,
                [AddressingModes.AbsoluteX] = 0xFE
            },
            [InstructionType.SBC] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Immediate] = 0xE9,
                [AddressingModes.ZeroPage] = 0xE5,
                [AddressingModes.ZeroPageX] = 0xF5,
                [AddressingModes.Absolute] = 0xED,
                [AddressingModes.AbsoluteX] = 0xFD,
                [AddressingModes.AbsoluteY] = 0xF9,
                [AddressingModes.IndirectX] = 0xE1,
                [AddressingModes.IndirectY] = 0xF1,
            },
            #endregion
            #region Logic
            [InstructionType.AND] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Immediate] = 0x29,
                [AddressingModes.ZeroPage] = 0x25,
                [AddressingModes.ZeroPageX] = 0x35,
                [AddressingModes.Absolute] = 0x2D,
                [AddressingModes.AbsoluteX] = 0x3D,
                [AddressingModes.AbsoluteY] = 0x39,
                [AddressingModes.IndirectX] = 0x21,
                [AddressingModes.IndirectY] = 0x31,
            },
            [InstructionType.EOR] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Immediate] = 0x49,
                [AddressingModes.ZeroPage] = 0x45,
                [AddressingModes.ZeroPageX] = 0x55,
                [AddressingModes.Absolute] = 0x4D,
                [AddressingModes.AbsoluteX] = 0x5D,
                [AddressingModes.AbsoluteY] = 0x59,
                [AddressingModes.IndirectX] = 0x41,
                [AddressingModes.IndirectY] = 0x51,
            },
            [InstructionType.ORA] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Immediate] = 0x09,
                [AddressingModes.ZeroPage] = 0x05,
                [AddressingModes.ZeroPageX] = 0x15,
                [AddressingModes.Absolute] = 0x0D,
                [AddressingModes.AbsoluteX] = 0x1D,
                [AddressingModes.AbsoluteY] = 0x19,
                [AddressingModes.IndirectX] = 0x01,
                [AddressingModes.IndirectY] = 0x11,
            },
            #endregion
            #region Comparison
            [InstructionType.BIT] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.ZeroPage] = 0x24,
                [AddressingModes.Absolute] = 0x2C
            },
            [InstructionType.CMP] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Immediate] = 0xC9,
                [AddressingModes.ZeroPage] = 0xC5,
                [AddressingModes.ZeroPageX] = 0xD5,
                [AddressingModes.Absolute] = 0xCD,
                [AddressingModes.AbsoluteX] = 0xDD,
                [AddressingModes.AbsoluteY] = 0xD9,
                [AddressingModes.IndirectX] = 0xC1,
                [AddressingModes.IndirectY] = 0xD1,
            },
            [InstructionType.CPX] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Immediate] = 0xE0,
                [AddressingModes.ZeroPage] = 0xE4,
                [AddressingModes.Absolute] = 0xEC
            },
            [InstructionType.CPY] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Immediate] = 0xC0,
                [AddressingModes.ZeroPage] = 0xC4,
                [AddressingModes.Absolute] = 0xCC
            },
            #endregion
            #region Branching
            [InstructionType.BCC] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Relative] = 0x90,
            },
            [InstructionType.BCS] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Relative] = 0xB0,
            },
            [InstructionType.BEQ] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Relative] = 0xF0,
            },
            [InstructionType.BMI] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Relative] = 0x30,
            },
            [InstructionType.BNE] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Relative] = 0xD0
            },
            [InstructionType.BPL] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Relative] = 0x10
            },
            [InstructionType.BVC] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Relative] = 0x50
            },
            [InstructionType.BVS] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Relative] = 0x70
            },
            #endregion
            #region Memory
            [InstructionType.LDA] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Immediate] = 0xA9,
                [AddressingModes.ZeroPage] = 0xA5,
                [AddressingModes.ZeroPageX] = 0xB5,
                [AddressingModes.Absolute] = 0xAD,
                [AddressingModes.AbsoluteX] = 0xBD,
                [AddressingModes.AbsoluteY] = 0xB9,
                [AddressingModes.IndirectX] = 0xA1,
                [AddressingModes.IndirectY] = 0xB1,
            },
            [InstructionType.LDX] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Immediate] = 0xA2,
                [AddressingModes.ZeroPage] = 0xA6,
                [AddressingModes.ZeroPageY] = 0xB6,
                [AddressingModes.Absolute] = 0xAE,
                [AddressingModes.AbsoluteY] = 0xBE,
            },
            [InstructionType.LDY] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Immediate] = 0xA0,
                [AddressingModes.ZeroPage] = 0xA4,
                [AddressingModes.ZeroPageX] = 0xB4,
                [AddressingModes.Absolute] = 0xAC,
                [AddressingModes.AbsoluteX] = 0xBC,
            },
            [InstructionType.STA] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.ZeroPage] = 0x85,
                [AddressingModes.ZeroPageX] = 0x95,
                [AddressingModes.Absolute] = 0x8D,
                [AddressingModes.AbsoluteX] = 0x9D,
                [AddressingModes.AbsoluteY] = 0x99,
                [AddressingModes.IndirectX] = 0x81,
                [AddressingModes.IndirectY] = 0x91,
            },
            [InstructionType.STX] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.ZeroPage] = 0x86,
                [AddressingModes.ZeroPageY] = 0x96,
                [AddressingModes.Absolute] = 0x8E,
            },
            [InstructionType.STY] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.ZeroPage] = 0x84,
                [AddressingModes.ZeroPageX] = 0x94,
                [AddressingModes.Absolute] = 0x8C
            },
            #endregion
            #region StatusAndSystem
            [InstructionType.CLC] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0x18
            },
            [InstructionType.SEC] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0x38
            },
            [InstructionType.CLI] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0x58
            },
            [InstructionType.SEI] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0x78
            },
            [InstructionType.CLV] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0xB8
            },
            [InstructionType.CLD] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0xD8
            },
            [InstructionType.SED] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0xF8
            },
            [InstructionType.BRK] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0x00
            },
            [InstructionType.NOP] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0xEA
            },
            #endregion
            #region Register
            [InstructionType.TAX] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0xAA
            },
            [InstructionType.TXA] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0x8A
            },
            [InstructionType.DEX] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0xCA
            },
            [InstructionType.INX] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0xE8
            },
            [InstructionType.TAY] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0xA8
            },
            [InstructionType.TYA] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0x98
            },
            [InstructionType.DEY] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0x88
            },
            [InstructionType.INY] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0xC8
            },
            #endregion
            #region Other
            [InstructionType.JMP] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Absolute] = 0x4C,
                [AddressingModes.Indirect] = 0x6C,
                [AddressingModes.JumpLabel] = 0x4C,
            },
            [InstructionType.JSR] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Absolute] = 0x20,
                [AddressingModes.JumpLabel] = 0x20
            },
            [InstructionType.RTI] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0x40
            },
            [InstructionType.RTS] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0x60
            },
            #endregion
            #region Stack
            [InstructionType.TXS] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0x9A
            },
            [InstructionType.TSX] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0xBA
            },
            [InstructionType.PHA] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0x48
            },
            [InstructionType.PLA] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0x68
            },
            [InstructionType.PHP] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0x08
            },
            [InstructionType.PLP] = new Dictionary<AddressingModes, byte>()
            {
                [AddressingModes.Implied] = 0x28
            },
            #endregion
        };
        public static string GetAddresses(List<BaseInstruction> instructions)
        {
            StringBuilder result = new StringBuilder();

            foreach (var instruction in instructions)
            {
                //append op code and get all parameters

                var byteOffsetString = Convert.ToString(instruction.ByteOffset + InitialOffset, 16).PadLeft(4, '0');

                var text = $"${byteOffsetString}";

                result.Append(text);
                result.Append("\n");
            }


            return result.ToString();
        }
        public static List<string> GetAddressesList(List<BaseInstruction> instructions)
        {
            List<string> result = new List<string>();
            foreach (var instruction in instructions)
            {
                var byteOffsetString = Convert.ToString(instruction.ByteOffset + InitialOffset, 16).PadLeft(4, '0');

                var text = $"${byteOffsetString}";
                result.Add(text);
            }

            return result;
        }
        public static string HexDumpStr(List<BaseInstruction> instructions)
        {
            StringBuilder result = new StringBuilder();
            foreach (var instruction in instructions)
            {
                result.Append(Convert.ToString(instruction.OpCode, 16).PadLeft(2, '0'));
                foreach (var item in instruction.Parameters)
                {
                    result.Append(" ");
                    result.Append(Convert.ToString(item, 16).PadLeft(2, '0'));
                }
                result.Append("\n");
            }

            return result.ToString().Trim();
        }
        public static List<string> HexDumpList(List<BaseInstruction> instructions)
        {
            List<string> hexdump = new List<string>();
            foreach (var instruction in instructions)
            {
                StringBuilder line = new StringBuilder();

                line.Append(Convert.ToString(instruction.OpCode, 16).PadLeft(2, '0'));
                foreach (var item in instruction.Parameters)
                {
                    line.Append(" ");
                    line.Append(Convert.ToString(item, 16).PadLeft(2, '0'));
                }

                hexdump.Add(line.ToString());
            }

            return hexdump;
        }
        public static string DissassemblyStr(List<string> dissassembly)
        {
            StringBuilder final = new StringBuilder();
       
            foreach(var item in dissassembly)
            {
                final.Append(item);
                final.Append("\n");
            }
            return final.ToString();
        }

        public static int MemorySize = 65536;

        public static short InitialOffset = 0x600;

        public static Font Font;

        public static Dictionary<InstructionType, Func<short, BaseInstruction>> InstructionTypeToInstruction = new Dictionary<InstructionType, Func<short, BaseInstruction>>()
        {
            [InstructionType.ADC] = new Func<short, BaseInstruction>((byteOffset) => new ADC() { ByteOffset = byteOffset }),
            [InstructionType.AND] = new Func<short, BaseInstruction>((byteOffset) => new AND() { ByteOffset = byteOffset }),
            [InstructionType.BIT] = new Func<short, BaseInstruction>((byteOffset) => new BIT() { ByteOffset = byteOffset }),
            [InstructionType.ASL] = new Func<short, BaseInstruction>((byteOffset) => new ASL() { ByteOffset = byteOffset }),
            [InstructionType.BCC] = new Func<short, BaseInstruction>((byteOffset) => new BCC() { ByteOffset = byteOffset }),
            [InstructionType.BCS] = new Func<short, BaseInstruction>((byteOffset) => new BCS() { ByteOffset = byteOffset }),
            [InstructionType.BEQ] = new Func<short, BaseInstruction>((byteOffset) => new BEQ() { ByteOffset = byteOffset }),
            [InstructionType.BMI] = new Func<short, BaseInstruction>((byteOffset) => new BMI() { ByteOffset = byteOffset }),
            [InstructionType.BNE] = new Func<short, BaseInstruction>((byteOffset) => new BNE() { ByteOffset = byteOffset }),
            [InstructionType.BPL] = new Func<short, BaseInstruction>((byteOffset) => new BPL() { ByteOffset = byteOffset }),
            [InstructionType.BVC] = new Func<short, BaseInstruction>((byteOffset) => new BVC() { ByteOffset = byteOffset }),
            [InstructionType.BVS] = new Func<short, BaseInstruction>((byteOffset) => new BVS() { ByteOffset = byteOffset }),
            [InstructionType.BRK] = new Func<short, BaseInstruction>((byteOffset) => new BRK() { ByteOffset = byteOffset }),
            [InstructionType.CMP] = new Func<short, BaseInstruction>((byteOffset) => new CMP() { ByteOffset = byteOffset }),
            [InstructionType.CPX] = new Func<short, BaseInstruction>((byteOffset) => new CPX() { ByteOffset = byteOffset }),
            [InstructionType.CPY] = new Func<short, BaseInstruction>((byteOffset) => new CPY() { ByteOffset = byteOffset }),
            [InstructionType.DEC] = new Func<short, BaseInstruction>((byteOffset) => new DEC() { ByteOffset = byteOffset }),
            [InstructionType.EOR] = new Func<short, BaseInstruction>((byteOffset) => new EOR() { ByteOffset = byteOffset }),
            [InstructionType.CLC] = new Func<short, BaseInstruction>((byteOffset) => new CLC() { ByteOffset = byteOffset }),
            [InstructionType.SEC] = new Func<short, BaseInstruction>((byteOffset) => new SEC() { ByteOffset = byteOffset }),
            [InstructionType.CLI] = new Func<short, BaseInstruction>((byteOffset) => new CLI() { ByteOffset = byteOffset }),
            [InstructionType.SEI] = new Func<short, BaseInstruction>((byteOffset) => new SEI() { ByteOffset = byteOffset }),
            [InstructionType.CLV] = new Func<short, BaseInstruction>((byteOffset) => new CLV() { ByteOffset = byteOffset }),
            [InstructionType.CLD] = new Func<short, BaseInstruction>((byteOffset) => new CLD() { ByteOffset = byteOffset }),
            [InstructionType.SED] = new Func<short, BaseInstruction>((byteOffset) => new SED() { ByteOffset = byteOffset }),
            [InstructionType.INC] = new Func<short, BaseInstruction>((byteOffset) => new INC() { ByteOffset = byteOffset }),
            [InstructionType.JMP] = new Func<short, BaseInstruction>((byteOffset) => new JMP() { ByteOffset = byteOffset }),
            [InstructionType.JSR] = new Func<short, BaseInstruction>((byteOffset) => new JSR() { ByteOffset = byteOffset }),
            [InstructionType.LDA] = new Func<short, BaseInstruction>((byteOffset) => new LDA() { ByteOffset = byteOffset }),
            [InstructionType.LDX] = new Func<short, BaseInstruction>((byteOffset) => new LDX() { ByteOffset = byteOffset }),
            [InstructionType.LDY] = new Func<short, BaseInstruction>((byteOffset) => new LDY() { ByteOffset = byteOffset }),
            [InstructionType.LSR] = new Func<short, BaseInstruction>((byteOffset) => new LSR() { ByteOffset = byteOffset }),
            [InstructionType.NOP] = new Func<short, BaseInstruction>((byteOffset) => new NOP() { ByteOffset = byteOffset }),
            [InstructionType.ORA] = new Func<short, BaseInstruction>((byteOffset) => new ORA() { ByteOffset = byteOffset }),
            [InstructionType.TAX] = new Func<short, BaseInstruction>((byteOffset) => new TAX() { ByteOffset = byteOffset }),
            [InstructionType.TXA] = new Func<short, BaseInstruction>((byteOffset) => new TXA() { ByteOffset = byteOffset }),
            [InstructionType.DEX] = new Func<short, BaseInstruction>((byteOffset) => new DEX() { ByteOffset = byteOffset }),
            [InstructionType.INX] = new Func<short, BaseInstruction>((byteOffset) => new INX() { ByteOffset = byteOffset }),
            [InstructionType.TAY] = new Func<short, BaseInstruction>((byteOffset) => new TAY() { ByteOffset = byteOffset }),
            [InstructionType.TYA] = new Func<short, BaseInstruction>((byteOffset) => new TYA() { ByteOffset = byteOffset }),
            [InstructionType.DEY] = new Func<short, BaseInstruction>((byteOffset) => new DEY() { ByteOffset = byteOffset }),
            [InstructionType.INY] = new Func<short, BaseInstruction>((byteOffset) => new INY() { ByteOffset = byteOffset }),
            [InstructionType.ROL] = new Func<short, BaseInstruction>((byteOffset) => new ROL() { ByteOffset = byteOffset }),
            [InstructionType.ROR] = new Func<short, BaseInstruction>((byteOffset) => new ROR() { ByteOffset = byteOffset }),
            [InstructionType.RTI] = new Func<short, BaseInstruction>((byteOffset) => new RTI() { ByteOffset = byteOffset }),
            [InstructionType.RTS] = new Func<short, BaseInstruction>((byteOffset) => new RTS() { ByteOffset = byteOffset }),
            [InstructionType.SBC] = new Func<short, BaseInstruction>((byteOffset) => new SBC() { ByteOffset = byteOffset }),
            [InstructionType.STA] = new Func<short, BaseInstruction>((byteOffset) => new STA() { ByteOffset = byteOffset }),
            [InstructionType.TXS] = new Func<short, BaseInstruction>((byteOffset) => new TXS() { ByteOffset = byteOffset }),
            [InstructionType.TSX] = new Func<short, BaseInstruction>((byteOffset) => new TSX() { ByteOffset = byteOffset }),
            [InstructionType.PHA] = new Func<short, BaseInstruction>((byteOffset) => new PHA() { ByteOffset = byteOffset }),
            [InstructionType.PLA] = new Func<short, BaseInstruction>((byteOffset) => new PLA() { ByteOffset = byteOffset }),
            [InstructionType.PHP] = new Func<short, BaseInstruction>((byteOffset) => new PHP() { ByteOffset = byteOffset }),
            [InstructionType.PLP] = new Func<short, BaseInstruction>((byteOffset) => new PLP() { ByteOffset = byteOffset }),
            [InstructionType.STX] = new Func<short, BaseInstruction>((byteOffset) => new STX() { ByteOffset = byteOffset }),
            [InstructionType.STY] = new Func<short, BaseInstruction>((byteOffset) => new STY() { ByteOffset = byteOffset }),
        };

        public static Dictionary<InstructionType, string> InstructionDescriptions = new Dictionary<InstructionType, string>();
    }
}
