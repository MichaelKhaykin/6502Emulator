using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator.Instructions
{
    public abstract class BaseInstruction : IInstruction
    {
        public abstract InstructionType Type { get; }
        public AddressingModes Mode { get; set; }
        public List<byte> Parameters { get; set; }
        public abstract byte OpCode { get; }
        public short ByteOffset { get; init; }
        public int InstructionNumber { get; set; }
        public string Description { get; set; }
    }
}
