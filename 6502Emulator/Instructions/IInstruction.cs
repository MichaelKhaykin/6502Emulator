using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator
{
    public interface IInstruction
    {
        InstructionType Type { get; }
        AddressingModes Mode  { get; }
        List<byte> Parameters { get; }
        int InstructionNumber { get; }
        byte OpCode { get; }
    }
}
    