using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator.Instructions.Arthimetic
{
    public class TXS : IInstruction
    {
        public InstructionType Type
        {
            get
            {
                return InstructionType.TXS;
            }
        }
        public byte OpCode
        {
            get
            {
                return Helper.InstructionTypeToOpCode[Type][Mode];
            }
        }
        public AddressingModes Mode { get; init; }
        public List<byte> Parameters { get; init; }
        public int InstructionNumber { get; init; }
    }
}
