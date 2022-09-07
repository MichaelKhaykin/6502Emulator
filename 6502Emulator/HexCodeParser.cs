using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator
{
    public class HexCodeParser
    {
        Dictionary<byte, (InstructionType type, AddressingModes mode)> reversedMap = new Dictionary<byte, (InstructionType type, AddressingModes mode)>();
      
        List<string> hexdump;
        List<string> offsets;
        public HexCodeParser(List<string> hexdump, List<string> offsets)
        {
            Setup();
            this.hexdump = hexdump;
            this.offsets = offsets;
        }

        private void Setup()
        {
            var originalDict = Helper.InstructionTypeToOpCode;

            foreach (var item in originalDict)
            {
                foreach (var kvp in item.Value)
                {
                    //a duplicate key may occur as some instructions have two diffetent 
                    //addressing modes mapped to the same byte code for parsing purposes

                    if (reversedMap.ContainsKey(kvp.Value)) continue;

                    reversedMap.Add(kvp.Value, (item.Key, kvp.Key));
                }
            }
        }

        public List<string> Parse()
        {
            List<string> dissassembly = new List<string>();

            hexdump = hexdump.Select((x) => x.Trim()).ToList();
            for(int i = 0; i < hexdump.Count; i++)
            {
                var line = hexdump[i];
                var split = line.Split(' ');

                split[0].ToDecimal(out int instruction);

                byte instructionByte = (byte)instruction;

                if (!reversedMap.ContainsKey(instructionByte))
                {
                    throw new Exception("something went wrong, cannot figure out what instruction this is");
                }

                StringBuilder builder = new StringBuilder();

                var (instructionType, addressMode) = reversedMap[instructionByte];

                builder.Append(instructionType.ToString());
                builder.Append(" ");

                switch (addressMode)
                {
                    case AddressingModes.Implied:
                        //do nothing, because these instructions don't have parameters
                        break;

                    case AddressingModes.Relative:

                        //parameter[0] needs to be figured out as positive or negative
                        //take byteoffset + parameter[0] + 2

                        offsets[i].Substring(1).ToDecimal(out int byteOffset);

                        split[1].ToDecimal(out int parameter);
                        if (parameter >= 128)
                        {
                            parameter -= 256;
                        }

                        builder.Append($"${(byteOffset + parameter + 2).ToHex().PadLeft(4, '0')}");

                        break;

                    case AddressingModes.Accumulator:

                        if (split.Length == 1)
                        {
                            builder.Append("A");
                        }
                        else
                        {
                            builder.Append($"{split[1]}");
                        }

                        break;

                    case AddressingModes.Immediate:
                        builder.Append($"#${split[1]}");
                        break;

                    case AddressingModes.ZeroPage:
                        builder.Append($"${split[1]}");
                        break;

                    case AddressingModes.ZeroPageX:
                        builder.Append($"${split[1]},X");
                        break;

                    case AddressingModes.ZeroPageY:
                        builder.Append($"${split[1]},Y");
                        break;

                    case AddressingModes.Absolute:
                        builder.Append($"${split[2] + split[1]}");
                        break;

                    case AddressingModes.AbsoluteX:
                        builder.Append($"${split[2] + split[1]},X");
                        break;

                    case AddressingModes.AbsoluteY:
                        builder.Append($"${split[2] + split[1]},Y");
                        break;

                    case AddressingModes.Indirect:
                        builder.Append($"(${split[2] + split[1]})");
                        break;

                    case AddressingModes.IndirectX:
                        builder.Append($"(${split[1]},X)");
                        break;

                    case AddressingModes.IndirectY:
                        builder.Append($"(${split[1]}),Y");
                        break;
                }

                dissassembly.Add(builder.ToString());
            }
            dissassembly = dissassembly.Select((x) => x.ToUpper()).ToList();
            return dissassembly;
        }
    }
}
