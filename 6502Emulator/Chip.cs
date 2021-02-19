using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator
{
    public class Chip
    {
        public Ram Ram { get; }

        private byte XRegister;
        private byte YRegister;
        private byte AccumulatorRegister;

        private byte StackPointer;
        private short ProgramCounter;

        public Dictionary<FlagType, Flag> Flags = new Dictionary<FlagType, Flag>()
        {
            [FlagType.N] = new Flag("Negative flag"),
            [FlagType.Z] = new Flag("Zero flag"),
            [FlagType.C] = new Flag("Carry flag"),
            [FlagType.I] = new Flag("Interrupt disable flag"),
            [FlagType.D] = new Flag("Decimal flag (base 10 vs base 16 math)"),
            [FlagType.V] = new Flag("Overflow flag"),
        };

        public Chip()
        {
            XRegister = 0;
            YRegister = 0;
            AccumulatorRegister = 0;

            StackPointer = 0;
            ProgramCounter = 0;
        }
    }
}
