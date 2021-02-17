using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator.Instructions.Arthimetic
{
    public class PLA : BaseInstruction
    {
        public override InstructionType Type
        {
            get
            {
                return InstructionType.PLA;
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
