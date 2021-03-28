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
    public class AssemblyParser
    {
        Dictionary<string, InstructionType> instructionNameMap = new Dictionary<string, InstructionType>();

        Dictionary<InstructionType, List<AddressingModes>> instructionToPossibleAddressingMode = new Dictionary<InstructionType, List<AddressingModes>>();

        Dictionary<AddressingModes, Func<string, BaseInstruction, bool>> addressModeParsingMethods = new Dictionary<AddressingModes, Func<string, BaseInstruction, bool>>();

        Dictionary<AddressingModes, List<Regex>> ParseMethods = new Dictionary<AddressingModes, List<Regex>>()
        {
            [AddressingModes.Immediate] = new List<Regex>()
            {
                new Regex("^[#][$](.){1,2}$"),
                new Regex("^[#]([^$]){1,3}$")
            },
            [AddressingModes.ZeroPage] = new List<Regex>()
            {
                new Regex("^[$](.){1,2}$")
            },
            [AddressingModes.ZeroPageX] = new List<Regex>()
            {
                new Regex("^[$](.){1,2}[,][X]$"),
            },
            [AddressingModes.ZeroPageY] = new List<Regex>()
            {
                new Regex("^[$](.){1,2}[,][Y]"),
            },
            [AddressingModes.Absolute] = new List<Regex>()
            {
                new Regex("^[$](.){3,4}$")
            },
            [AddressingModes.AbsoluteX] = new List<Regex>()
            {
                new Regex("^[$](.){3,4}[,][X]$"),
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
                new Regex("^[#][$](.){1,2}$"),
                new Regex("^[#]([^$]){1,3}$"),
                new Regex("^[A]$"),
                new Regex(""),
            }
        };

        List<(BaseInstruction instruction, string label)> BranchesToReVisit = new List<(BaseInstruction, string label)>();
        List<(BaseInstruction instruction, string label)> JumpsToReVisit = new List<(BaseInstruction, string label)>();

        Dictionary<string, short> labelOffsets = new Dictionary<string, short>();

        string code;

        short byteCounter = 0;
        public AssemblyParser(string sourceCodeFile)
        {
            code = sourceCodeFile;
            Setup();
        }

        private void Setup()
        {
            //setup all variables for parsing and the actual dictionary for parsing regex

            var props = typeof(InstructionType).GetEnumValues();
            foreach (var item in props)
            {
                instructionNameMap.Add(item.ToString(), (InstructionType)item);
            }

            foreach (var kvp in Helper.InstructionTypeToOpCode)
            {
                var key = kvp.Key;
                var val = kvp.Value;

                List<AddressingModes> addressingModes = new List<AddressingModes>();
                foreach (var innerKvp in val)
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
                        var bytes = ParseStr(code.Substring(1 + (isHex ? 1 : 0), code.Length - (1 + (isHex ? 1 : 0))), isHex);

                        byteCounter += (short)bytes.Count;

                        baseIns.Mode = AddressingModes.Immediate;
                        baseIns.Parameters = new List<byte>();
                        baseIns.Parameters.AddRange(bytes);

                        return true;
                    }
                    return false;
                }),
                [AddressingModes.ZeroPage] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.ZeroPage][0];
                    if (!regex.Match(code).Success) return false;

                    var hexed = code.Substring(1, code.Length - 1);

                    var bytes = ParseStr(hexed, true);

                    byteCounter += (short)bytes.Count;

                    baseIns.Mode = AddressingModes.ZeroPage;
                    baseIns.Parameters = new List<byte>();
                    baseIns.Parameters.AddRange(bytes);

                    return true;
                }),
                [AddressingModes.ZeroPageX] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.ZeroPageX][0];
                    if (!regex.Match(code).Success) return false;

                    var split = code.Split(',');

                    var bytes = ParseStr(split[0].Substring(1), true);
                    byteCounter += (short)bytes.Count;

                    baseIns.Mode = AddressingModes.ZeroPageX;
                    baseIns.Parameters = new List<byte>();
                    baseIns.Parameters.AddRange(bytes);

                    return true;
                }),
                [AddressingModes.ZeroPageY] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.ZeroPageY][0];
                    if (!regex.Match(code).Success) return false;

                    var split = code.Split(',');

                    var bytes = ParseStr(split[0].Substring(1), true);
                    byteCounter += (short)bytes.Count;

                    baseIns.Mode = AddressingModes.ZeroPageY;
                    baseIns.Parameters = new List<byte>();
                    baseIns.Parameters.AddRange(bytes);

                    return true;
                }),
                [AddressingModes.Absolute] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.Absolute][0];
                    if (!regex.Match(code).Success) return false;

                    string hexedValue = code.Substring(1);

                    var bytes = ParseStr(hexedValue, true);
                    byteCounter += (short)bytes.Count;

                    baseIns.Mode = AddressingModes.Absolute;
                    baseIns.Parameters = new List<byte>();
                    baseIns.Parameters.AddRange(bytes);

                    return true;
                }),
                [AddressingModes.AbsoluteX] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.AbsoluteX][0];
                    if (!regex.Match(code).Success) return false;

                    var args = code.Split(',');
                    string hexedValue = args[0].Substring(1);

                    var bytes = ParseStr(hexedValue, true);
                    byteCounter += (short)bytes.Count;

                    baseIns.Mode = AddressingModes.AbsoluteX;
                    baseIns.Parameters = new List<byte>();
                    baseIns.Parameters.AddRange(bytes);

                    return true;
                }),
                [AddressingModes.AbsoluteY] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.AbsoluteY][0];
                    if (!regex.Match(code).Success) return false;

                    var args = code.Split(',');
                    string hexedValue = args[0].Substring(1);

                    var bytes = ParseStr(hexedValue, true);
                    byteCounter += (short)bytes.Count;

                    baseIns.Mode = AddressingModes.AbsoluteY;
                    baseIns.Parameters = new List<byte>();
                    baseIns.Parameters.AddRange(bytes);

                    return true;
                }),
                [AddressingModes.Indirect] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.Indirect][0];
                    if (!regex.Match(code).Success) return false;

                    var hex = code.Substring(2).Trim(')');
                    var bytes = ParseStr(hex, true);
                    byteCounter += (short)bytes.Count;

                    baseIns.Mode = AddressingModes.Indirect;

                    baseIns.Parameters = new List<byte>();
                    baseIns.Parameters.AddRange(bytes);

                    return true;
                }),
                [AddressingModes.IndirectX] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.IndirectX][0];
                    if (!regex.Match(code).Success) return false;

                    var split = code.Split(',');

                    string hexedValue = split[0].Substring(2).Trim(')');
                    var bytes = ParseStr(hexedValue, true);

                    byteCounter += (short)bytes.Count;

                    baseIns.Mode = AddressingModes.IndirectX;
                    baseIns.Parameters = new List<byte>();
                    baseIns.Parameters.AddRange(bytes);

                    return true;
                }),
                [AddressingModes.IndirectY] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    var regex = ParseMethods[AddressingModes.IndirectY][0];
                    if (!regex.Match(code).Success) return false;

                    var split = code.Split(',');

                    string hexedValue = split[0].Substring(2).Trim(')');
                    var bytes = ParseStr(hexedValue, true);

                    byteCounter += (short)bytes.Count;

                    baseIns.Mode = AddressingModes.IndirectY;
                    baseIns.Parameters = new List<byte>();
                    baseIns.Parameters.AddRange(bytes);

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
                        var bytes = new List<byte>();
                   
                        if (code != "A" && code != "")
                        {
                            bytes = ParseStr(code.Substring(1 + (isHex ? 1 : 0), code.Length - (1 + (isHex ? 1 : 0))), isHex);
                        }

                        byteCounter += (short)bytes.Count;

                        baseIns.Mode = AddressingModes.Accumulator;

                        baseIns.Parameters = new List<byte>();
                        baseIns.Parameters.AddRange(bytes);

                        return true;
                    }
                    return false;
                }),
                [AddressingModes.Relative] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    BranchesToReVisit.Add((baseIns, code));
                    byteCounter++;

                    baseIns.Mode = AddressingModes.Relative;
                    baseIns.Parameters = new List<byte>();
                    return true;
                }),
                [AddressingModes.Implied] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    baseIns.Mode = AddressingModes.Implied;
                    baseIns.Parameters = new List<byte>();
                    return true;
                }),
                [AddressingModes.JumpLabel] = new Func<string, BaseInstruction, bool>((code, baseIns) =>
                {
                    JumpsToReVisit.Add((baseIns, code));
                    byteCounter += 2;

                    baseIns.Mode = AddressingModes.JumpLabel;
                    baseIns.Parameters = new List<byte>();

                    return true;
                }),
            };
        }

        public static string[] GetRidOfCommentsAndEmptyLines(string[] lines)
        {
            return lines.Select((x) =>
            {
                if (!x.Contains(';')) return x.Trim();

                return x.Substring(0, x.IndexOf(';')).Trim();
            }).Where((x) => !string.IsNullOrEmpty(x)).ToArray();
        }
        public static string[] ReplaceDefines(string[] lines)
        {
            List<string> linesWithoutDefines = new List<string>();

            Dictionary<string, string> defineValues = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (!line.Contains("define"))
                {
                    linesWithoutDefines.Add(line);
                    continue;
                }

                var splits = line.Split(' ').Where((x) => !string.IsNullOrEmpty(x)).ToArray();
                defineValues.Add(splits[1], splits[2]);
            }

            for (int i = 0; i < linesWithoutDefines.Count; i++)
            {
                foreach (var defineValue in defineValues)
                {
                    if (linesWithoutDefines[i].Contains(defineValue.Key))
                    {
                        linesWithoutDefines[i] = linesWithoutDefines[i].Replace(defineValue.Key, defineValue.Value);
                    }
                }
            }

            return linesWithoutDefines.ToArray();
        }
        public static string[] RemoveLabels(string[] lines)
        {
            Regex labelRegexMatcher = new Regex("^( ){0,}([^ ]){1,100}[:]( ){0,}$");
            return lines.Where((x) => !labelRegexMatcher.Match(x).Success).ToArray();
        }
        public static Dictionary<string, string> GenerateDefineReplacementTable(string[] lines)
        {
            Dictionary<string, string> defineValues = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (!line.Contains("define")) continue;
               
                var splits = line.Split(' ').Where((x) => !string.IsNullOrEmpty(x)).ToArray();
                defineValues.Add(splits[1], splits[2]);
            }

            return defineValues;
        }
        public List<BaseInstruction> Parse()
        {
            string[] lines = System.IO.File.ReadAllLines(code);

            lines = ReplaceDefines(lines);
            lines = GetRidOfCommentsAndEmptyLines(lines);

            Regex labelRegexMatcher = new Regex("^( ){0,}([^ ]){1,100}[:]( ){0,}$");

            List<BaseInstruction> instructions = new List<BaseInstruction>();

            for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                if (labelRegexMatcher.Match(lines[lineNumber]).Success)
                {
                    labelOffsets.Add(lines[lineNumber].Split(':')[0].ToUpper(), byteCounter);
                    continue;
                }

                var line = lines[lineNumber];

                string instructionName = line.Substring(0, 3);
                InstructionType type = GetInstructionType(instructionName);

                string rest = line.Substring(3, line.Length - 3).Trim().Replace(" ", "").ToUpper();
                var possibleMatches = instructionToPossibleAddressingMode[type];

                var insSize = instructions.Count;
                foreach (var possibleMatch in possibleMatches)
                {
                    var specificCreation = Helper.InstructionTypeToInstruction[type](byteCounter);
                    specificCreation.InstructionNumber = lineNumber;

                    bool successful = addressModeParsingMethods[possibleMatch](rest, specificCreation);

                    if (!successful) continue;

                    instructions.Add(specificCreation);
                    break;
                }

                if (instructions.Count == insSize)
                {
                    //bad things happened we were unable to add an instruction
                    throw new Exception("BAD");
                }
                byteCounter++;
            }

            foreach (var item in BranchesToReVisit)
            {
                var ins = item.instruction;
                var labelOffset = labelOffsets[item.label];

                //+1 for paramater size of branch instruction
                var diff = labelOffset - (ins.ByteOffset + 1);
                if (diff < -128 || diff > 127)
                {
                    throw new Exception("Branch too far");
                }

                //to account for program counter
                diff -= 1;

                ins.Parameters.Add((byte)diff);
            }

            foreach (var item in JumpsToReVisit)
            {
                var ins = item.instruction;
                var labelOffset = labelOffsets[item.label] + Helper.InitialOffset;

                ins.Parameters.Add((byte)(labelOffset & 255));
                ins.Parameters.Add((byte)(labelOffset >> 8));
            }

            return instructions;
        }
        private InstructionType GetInstructionType(string instructionName)
        {
            if (!instructionNameMap.ContainsKey(instructionName.ToUpper()))
            {
                throw new Exception("Invalid instruction or format");
            }
            return instructionNameMap[instructionName.ToUpper()];
        }
        private List<byte> ParseStr(string str, bool ishex)
        {
            if (str.Length <= 3)
            {
                if (ishex)
                {
                    if(str.Length == 3)
                    {
                        str.ToDecimal(out int y);
                        if (y >= Helper.MemorySize) throw new Exception();

                        byte hb = Convert.ToByte(y & 255);
                        byte lb = Convert.ToByte(y >> 8);

                        return new List<byte>() { hb, lb };
                    }

                    return new List<byte>() { Convert.ToByte(str, 16) };
                }
                return new List<byte>() { byte.Parse(str) };
            }

            if (!ishex) throw new Exception();

            if(str.Contains(','))
            {
                str = str.Split(',')[0];
            }

            str.ToDecimal(out int decimalRepresentation);
            if (decimalRepresentation >= Helper.MemorySize) throw new Exception();

            byte highByte = Convert.ToByte(decimalRepresentation & 255);
            byte lowByte = Convert.ToByte(decimalRepresentation >> 8);

            return new List<byte>() { highByte, lowByte };
        }
    }
}
