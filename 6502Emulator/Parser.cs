using _6502Emulator.Instructions;
using _6502Emulator.Instructions.Arthimetic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _6502Emulator
{
    public class Parser
    {
        Dictionary<string, InstructionType> instructionNameMap = new Dictionary<string, InstructionType>();

        Dictionary<InstructionType, List<AddressingModes>> instructionToPossibleAddressingMode = new Dictionary<InstructionType, List<AddressingModes>>();

        Dictionary<AddressingModes, Func<string, BaseInstruction, bool>> addressModeParsingMethods = new Dictionary<AddressingModes, Func<string, BaseInstruction, bool>>();

        Dictionary<InstructionType, Func<IInstruction>> InstructionTypeToInstruction = new Dictionary<InstructionType, Func<IInstruction>>()
        {
            [InstructionType.ADC] = new Func<IInstruction>(() => new ADC()),
            [InstructionType.AND] = new Func<IInstruction>(() => new AND()),
            [InstructionType.BIT] = new Func<IInstruction>(() => new BIT()),
            [InstructionType.ASL] = new Func<IInstruction>(() => new ASL()),
            [InstructionType.BCC] = new Func<IInstruction>(() => new BCC()),
            [InstructionType.BCS] = new Func<IInstruction>(() => new BCS()),
            [InstructionType.BEQ] = new Func<IInstruction>(() => new BEQ()),
            [InstructionType.BMI] = new Func<IInstruction>(() => new BMI()),
            [InstructionType.BNE] = new Func<IInstruction>(() => new BNE()),
            [InstructionType.BPL] = new Func<IInstruction>(() => new BPL()),
            [InstructionType.BVC] = new Func<IInstruction>(() => new BVC()),
            [InstructionType.BVS] = new Func<IInstruction>(() => new BVS()),
            [InstructionType.BRK] = new Func<IInstruction>(() => new BRK()),
            [InstructionType.CMP] = new Func<IInstruction>(() => new CMP()),
            [InstructionType.CPX] = new Func<IInstruction>(() => new CPX()),
            [InstructionType.CPY] = new Func<IInstruction>(() => new CPY()),
            [InstructionType.DEC] = new Func<IInstruction>(() => new DEC()),
            [InstructionType.EOR] = new Func<IInstruction>(() => new EOR()),
            [InstructionType.CLC] = new Func<IInstruction>(() => new CLC()),
            [InstructionType.SEC] = new Func<IInstruction>(() => new SEC()),
            [InstructionType.CLI] = new Func<IInstruction>(() => new CLI()),
            [InstructionType.SEI] = new Func<IInstruction>(() => new SEI()),
            [InstructionType.CLV] = new Func<IInstruction>(() => new CLV()),
            [InstructionType.CLD] = new Func<IInstruction>(() => new CLD()),
            [InstructionType.SED] = new Func<IInstruction>(() => new SED()),
            [InstructionType.INC] = new Func<IInstruction>(() => new INC()),
            [InstructionType.JMP] = new Func<IInstruction>(() => new JMP()),
            [InstructionType.JSR] = new Func<IInstruction>(() => new JSR()),
            [InstructionType.LDA] = new Func<IInstruction>(() => new LDA()),
            [InstructionType.LDX] = new Func<IInstruction>(() => new LDX()),
            [InstructionType.LDY] = new Func<IInstruction>(() => new LDY()),
            [InstructionType.LSR] = new Func<IInstruction>(() => new LSR()),
            [InstructionType.NOP] = new Func<IInstruction>(() => new NOP()),
            [InstructionType.ORA] = new Func<IInstruction>(() => new ORA()),
            [InstructionType.TAX] = new Func<IInstruction>(() => new TAX()),
            [InstructionType.TXA] = new Func<IInstruction>(() => new TXA()),
            [InstructionType.DEX] = new Func<IInstruction>(() => new DEX()),
            [InstructionType.INX] = new Func<IInstruction>(() => new INX()),
            [InstructionType.TAY] = new Func<IInstruction>(() => new TAY()),
            [InstructionType.TYA] = new Func<IInstruction>(() => new TYA()),
            [InstructionType.DEY] = new Func<IInstruction>(() => new DEY()),
            [InstructionType.INY] = new Func<IInstruction>(() => new INY()),
            [InstructionType.ROL] = new Func<IInstruction>(() => new ROL()),
            [InstructionType.ROR] = new Func<IInstruction>(() => new ROR()),
            [InstructionType.RTI] = new Func<IInstruction>(() => new RTI()),
            [InstructionType.RTS] = new Func<IInstruction>(() => new RTS()),
            [InstructionType.SBC] = new Func<IInstruction>(() => new SBC()),
            [InstructionType.STA] = new Func<IInstruction>(() => new STA()),
            [InstructionType.TXS] = new Func<IInstruction>(() => new TXS()),
            [InstructionType.TSX] = new Func<IInstruction>(() => new TSX()),
            [InstructionType.PHA] = new Func<IInstruction>(() => new PHA()),
            [InstructionType.PLA] = new Func<IInstruction>(() => new PLA()),
            [InstructionType.PHP] = new Func<IInstruction>(() => new PHP()),
            [InstructionType.PLP] = new Func<IInstruction>(() => new PLP()),
            [InstructionType.STX] = new Func<IInstruction>(() => new STX()),
            [InstructionType.STY] = new Func<IInstruction>(() => new STY()),
        };

        Dictionary<AddressingModes, List<Regex>> ParseMethods = new Dictionary<AddressingModes, List<Regex>>()
        {
            [AddressingModes.Immediate] = new List<Regex>()
            {
                new Regex("[#][$](.){1,2}$"),
                new Regex("[#]([^$]){1,3}$")
            },
            [AddressingModes.ZeroPage] = new List<Regex>()
            {
                new Regex("^[$](.){1,2}$")
            },
            [AddressingModes.ZeroPageX] = new List<Regex>()
            {
                new Regex("^[$](.){1,2}[,][X]$")
            },
            [AddressingModes.ZeroPageY] = new List<Regex>()
            {
                new Regex("^[$](.){1,2}[,][Y]")
            },
            [AddressingModes.Absolute] = new List<Regex>()
            {
                new Regex("^[$](.){3,4}$")
            },
            [AddressingModes.AbsoluteX] = new List<Regex>()
            {
                new Regex("^[$](.){3,4}[,][X]$")
            },
            [AddressingModes.AbsoluteY] = new List<Regex>()
            {
                new Regex("^[$](.){3,4}[,][Y]$")
            },
            [AddressingModes.Indirect] = new List<Regex>()
            {
                new Regex("^[(][$](.){1,4}[)]")
            },
            [AddressingModes.IndirectX] = new List<Regex>()
            {
                new Regex("^[(][$](.){1,4}[,][X][)]$")
            },
            [AddressingModes.IndirectY] = new List<Regex>()
            {
                new Regex("^[(][$](.){1,4}[)][,][Y]$")
            },
            [AddressingModes.Accumulator] = new List<Regex>()
            {
                new Regex("[#][$](.){1,2}$"),
                new Regex("[#]([^$]){1,3}$")
            }
        };

        Dictionary<string, int> LabelLocationsMap = new Dictionary<string, int>();
        HashSet<int> LabelLocations = new HashSet<int>();
        public Parser(string sourceCodeFile, string configFile)
        {
            #region Setup
            var props = typeof(InstructionType).GetEnumValues();
            foreach (var item in props)
            {
                instructionNameMap.Add(item.ToString(), (InstructionType)item);
            }

            foreach(var kvp in Helper.InstructionTypeToOpCode)
            {
                var key = kvp.Key;
                var val = kvp.Value;

                List<AddressingModes> addressingModes = new List<AddressingModes>();
                foreach(var innerKvp in val)
                {
                    addressingModes.Add(innerKvp.Key);
                }

                instructionToPossibleAddressingMode.Add(key, addressingModes);
            }

            addressModeParsingMethods = new Dictionary<AddressingModes, Func<string, BaseInstruction, bool>>()
            {
                [AddressingModes.Immediate] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regexMethods = ParseMethods[AddressingModes.Immediate];

                    foreach (var regexExpression in regexMethods)
                    {
                        bool didWork = regexExpression.Match(code).Success;
                        if (!didWork) continue;

                        bool isHex = code.Contains('$');
                        var byteval = ParseStr(code.Substring(1 + (isHex ? 1 : 0), code.Length - (1 + (isHex ? 1 : 0))), isHex);
                        baseIns.Mode = AddressingModes.Immediate;
                        baseIns.Parameters = new List<byte>()
                        {
                            byteval
                        };
                        return true;
                    }
                    return false;
                }),
                [AddressingModes.ZeroPage] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.ZeroPage][0];
                    if (!regex.Match(code).Success) return false;

                    var hexed = code.Substring(1, code.Length - 1);

                    baseIns.Mode = AddressingModes.ZeroPage;
                    baseIns.Parameters = new List<byte>()
                    {
                        ParseStr(hexed, true)
                    };
                    return true;
                }),
                [AddressingModes.ZeroPageX] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.ZeroPageX][0];
                    if (!regex.Match(code).Success) return false;

                    var split = code.Split(',');

                    baseIns.Mode = AddressingModes.ZeroPageX;
                    baseIns.Parameters = new List<byte>()
                    {
                        ParseStr(split[0].Substring(1), true)
                    };

                    return true;
                }),
                [AddressingModes.ZeroPageY] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.ZeroPageY][0];
                    if (!regex.Match(code).Success) return false;

                    var split = code.Split(',');

                    baseIns.Mode = AddressingModes.ZeroPageY;
                    baseIns.Parameters = new List<byte>()
                    {
                        ParseStr(split[0].Substring(1), true)
                    };

                    return true;
                }),
                [AddressingModes.Absolute] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.Absolute][0];
                    if (!regex.Match(code).Success) return false;

                    string hexedValue = code.Substring(1);
                    var decimalRepresentation = hexedValue.ToDecimal();

                    if (decimalRepresentation >= Helper.MemorySize) return false;

                    byte highByte = Convert.ToByte(decimalRepresentation & 255);
                    byte lowByte = Convert.ToByte(decimalRepresentation >> 8);

                    baseIns.Mode = AddressingModes.Absolute;
                    baseIns.Parameters = new List<byte>() { highByte, lowByte };

                    return true;
                }),
                [AddressingModes.AbsoluteX] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.AbsoluteX][0];
                    if (!regex.Match(code).Success) return false;

                    var args = code.Split(',');
                    string hexedValue = args[0].Substring(1);
                    var decimalRepresentation = hexedValue.ToDecimal();

                    if (decimalRepresentation >= Helper.MemorySize) return false;

                    byte highByte = Convert.ToByte(decimalRepresentation & 255);
                    byte lowByte = Convert.ToByte(decimalRepresentation >> 8);

                    baseIns.Mode = AddressingModes.AbsoluteX;
                    baseIns.Parameters = new List<byte>() { highByte, lowByte };

                    return true;
                }),
                [AddressingModes.AbsoluteY] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.AbsoluteY][0];
                    if (!regex.Match(code).Success) return false;

                    var args = code.Split(',');
                    string hexedValue = args[0].Substring(1);
                    var decimalRepresentation = hexedValue.ToDecimal();

                    if (decimalRepresentation >= Helper.MemorySize) return false;

                    byte highByte = Convert.ToByte(decimalRepresentation & 255);
                    byte lowByte = Convert.ToByte(decimalRepresentation >> 8);

                    baseIns.Mode = AddressingModes.AbsoluteY;
                    baseIns.Parameters = new List<byte>() { highByte, lowByte };

                    return true;
                }),
                [AddressingModes.Indirect] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.Indirect][0];
                    if (!regex.Match(code).Success) return false;

                    var hex = code.Substring(2).Trim(')');
                    baseIns.Mode = AddressingModes.Indirect;
                    baseIns.Parameters = new List<byte>()
                    {
                        ParseStr(hex, true)
                    };
                    return true;
                }),
                [AddressingModes.IndirectX] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.IndirectX][0];
                    if (!regex.Match(code).Success) return false;

                    var split = code.Split(',');

                    string hexedValue = split[0].Substring(2).Trim(')');
                    var decimalRepresentation = hexedValue.ToDecimal();

                    if (decimalRepresentation >= Helper.MemorySize) return false;

                    byte highByte = Convert.ToByte(decimalRepresentation & 255);
                    byte lowByte = Convert.ToByte(decimalRepresentation >> 8);

                    baseIns.Mode = AddressingModes.IndirectX;
                    baseIns.Parameters = new List<byte>() { highByte, lowByte };

                    return true;
                }),
                [AddressingModes.IndirectY] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.IndirectY][0];
                    if (!regex.Match(code).Success) return false;

                    var split = code.Split(',');

                    string hexedValue = split[0].Substring(2).Trim(')');
                    var decimalRepresentation = hexedValue.ToDecimal();

                    if (decimalRepresentation >= Helper.MemorySize) return false;

                    byte highByte = Convert.ToByte(decimalRepresentation & 255);
                    byte lowByte = Convert.ToByte(decimalRepresentation >> 8);

                    baseIns.Mode = AddressingModes.IndirectY;
                    baseIns.Parameters = new List<byte>() { highByte, lowByte };

                    return true;
                }),
                [AddressingModes.Accumulator] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regexMethods = ParseMethods[AddressingModes.Accumulator];

                    foreach (var regexExpression in regexMethods)
                    {
                        bool didWork = regexExpression.Match(code).Success;
                        if (!didWork) continue;

                        bool isHex = code.Contains('$');
                        var byteval = ParseStr(code.Substring(1 + (isHex ? 1 : 0), code.Length - (1 + (isHex ? 1 : 0))), isHex);
                        baseIns.Mode = AddressingModes.Accumulator;
                        baseIns.Parameters = new List<byte>()
                        {
                            byteval
                        };
                        return true;
                    }
                    return false;
                }),
                [AddressingModes.Relative] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var labelLocation = LabelLocationsMap[code];
                    byte jumpAheadAmount = 0;
                    
                    if(labelLocation < baseIns.InstructionNumber)
                    {
                        if (255 - baseIns.InstructionNumber - labelLocation >= 255) return false;
                        jumpAheadAmount = (byte)(255 - baseIns.InstructionNumber - labelLocation);
                    }
                    else
                    {
                        if (labelLocation - baseIns.InstructionNumber >= 255) return false;
                        jumpAheadAmount = (byte)(labelLocation - baseIns.InstructionNumber);
                    }

                    baseIns.Mode = AddressingModes.Relative;
                    baseIns.Parameters = new List<byte>()
                    {
                        jumpAheadAmount
                    };
                    return true;
                }),
                [AddressingModes.Implied] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    baseIns.Mode = AddressingModes.Implied;
                    baseIns.Parameters = new List<byte>();
                    return true;
                }),
            };

            #endregion


            string[] lines = System.IO.File.ReadAllLines(sourceCodeFile);
            lines = lines.Select((x) => x.Trim()).Where((x) => !string.IsNullOrEmpty(x)).ToArray();

            Regex labelRegexMatcher = new Regex("^( ){0,}([^ ]){1,10}[:]( ){0,}$");
            //initial pass for labels
            for(int i = 0; i < lines.Length; i++)
            {
                if (!labelRegexMatcher.Match(lines[i]).Success) continue;

                LabelLocations.Add(i + 1);
                LabelLocationsMap.Add(lines[i].Trim(':'), i + 1);
            }


            List<IInstruction> instructions = new List<IInstruction>();

            for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                if (LabelLocations.Contains(lineNumber + 1)) continue;

                var line = lines[lineNumber];

                string instructionName = line.Substring(0, 3);
                InstructionType type = GetInstructionType(instructionName);

                string rest = line.Substring(3, line.Length - 3).Trim().Replace(" ", "");
                var possibleMatches = instructionToPossibleAddressingMode[type];

                var insSize = instructions.Count;
                foreach (var possibleMatch in possibleMatches)
                {
                    BaseInstruction b = new BaseInstruction()
                    {
                        InstructionNumber = lineNumber + 1,
                    };

                    bool successful = addressModeParsingMethods[possibleMatch](rest, b);

                    if (!successful) continue;

                    var specificCreation = InstructionTypeToInstruction[type]();
                    BaseInstructionToSpecificType(b, specificCreation);
                    instructions.Add(specificCreation);

                    break;
                }

                if (instructions.Count == insSize)
                {
                    //bad things happened we were unable to add an instruction
                    throw new Exception("BAD");
                }
            }
            var hexdump = Helper.HexDump(instructions);
            Debug.WriteLine(hexdump);
        }

        private InstructionType GetInstructionType(string instructionName)
        {
            if (!instructionNameMap.ContainsKey(instructionName))
            {
                throw new Exception("Invalid instruction or format");
            }
            return instructionNameMap[instructionName];
        }

        private void BaseInstructionToSpecificType(BaseInstruction b, IInstruction item)
        {
            var props = item.GetType().GetProperties();
            var propsOfB = b.GetType().GetProperties();

            Dictionary<string, PropertyInfo> itemMap = new Dictionary<string, PropertyInfo>();
            Dictionary<string, PropertyInfo> bMap = new Dictionary<string, PropertyInfo>();

            HashSet<string> strings = new HashSet<string>();

            for (int i = 0; i < props.Length; i++)
            {
                strings.Add(props[i].Name);

                itemMap.Add(props[i].Name, props[i]);
                bMap.Add(propsOfB[i].Name, propsOfB[i]);
            }

            foreach (var propName in strings)
            {
                if (propName == "OpCode" || propName == "Type") continue;

                var bVal = bMap[propName].GetValue(b);

                itemMap[propName].SetValue(item, bVal);
            }
        }

        private byte ParseStr(string str, bool ishex)
        {
            if (ishex)
            {
                return Convert.ToByte(str, 16);
            }
            return byte.Parse(str);
        }

    }
}
