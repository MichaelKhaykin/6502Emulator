using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator
{
    public class Ram
    {
        private FancyMemory<byte>[] Memory = new FancyMemory<byte>[0xFFFF];
        public ref FancyMemory<byte> this[int index] => ref Memory[index];    
    
        public Ram(Action<object, PropertyChangedEventArgs> actionToRunOnChange)
        {
            for(int i = 0; i < Memory.Length; i++)
            {
                Memory[i] = new FancyMemory<byte>(0, i);
                Memory[i].PropertyChanged += new PropertyChangedEventHandler(actionToRunOnChange);
            }
        }
    }
}
