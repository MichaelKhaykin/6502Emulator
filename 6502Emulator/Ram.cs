using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator
{
    public class Ram
    {
        private byte[] Memory = new byte[0xFFFF];
        public ref byte this[int index] => ref Memory[index];    
    
        public Ram()
        {

        }
    }
}
