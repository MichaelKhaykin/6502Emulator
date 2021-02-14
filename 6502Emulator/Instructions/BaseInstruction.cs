using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator.Instructions
{
    public class BaseInstruction : IInstruction
    {
        public InstructionType Type => throw new Exception();
        public AddressingModes Mode { get; set; }
        public List<byte> Parameters { get; set; }
        public int InstructionNumber { get; init; }
        public byte OpCode => throw new Exception();
    }
}
