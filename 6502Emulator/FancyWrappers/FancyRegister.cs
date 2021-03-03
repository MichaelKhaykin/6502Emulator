using _6502Emulator.FancyWrappers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator
{
    public class FancyRegister<T> : INotifyDataChanged<FancyRegister<T>>
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

        public string Name { get; private set; }

        public event Action<FancyRegister<T>, PropertyChangedEventArgs> PropertyChanged;
        public FancyRegister(T value, string name)
        {
            Value = value;
            Name = name;
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
