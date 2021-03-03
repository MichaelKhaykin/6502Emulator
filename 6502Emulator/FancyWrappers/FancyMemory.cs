using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using _6502Emulator.FancyWrappers;

namespace _6502Emulator
{
    public class FancyMemory<T> : INotifyDataChanged<FancyMemory<T>>
    {
        private T value;
        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;

                OnPropertyChanged();
            }
        }

        public int Index { get; private set; }

        public event Action<FancyMemory<T>, PropertyChangedEventArgs> PropertyChanged;
        public FancyMemory(T value, int index)
        {
            Value = value;
            Index = index;
        }
    
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
