using _6502Emulator.FancyWrappers;
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

        private PropertyObservationWrapper<byte>[] Memory = new PropertyObservationWrapper<byte>[0xFFFF];
        public ref PropertyObservationWrapper<byte> this[int index] => ref Memory[index];    
    
        public Ram(Action<PropertyObservationWrapper<byte>, PropertyChangedEventArgs> actionToRunOnChange)
        {
            for(int i = 0; i < Memory.Length; i++)
            {
                Memory[i] = new PropertyObservationWrapper<byte>(0, new MemoryData(i));
             
                Memory[i].PropertyChanged += actionToRunOnChange;
            }
        }
    }
}
