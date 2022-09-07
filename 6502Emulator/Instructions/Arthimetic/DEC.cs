using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator.Instructions.Arthimetic
{
    public class DEC : BaseInstruction
    {
        public override InstructionType Type
        {
            get
            {
                return InstructionType.DEC;
            }
        }
        public override byte OpCode
        {
            get
            {
                return Helper.InstructionTypeToOpCode[Type][Mode];
            }
        }
    }
}
