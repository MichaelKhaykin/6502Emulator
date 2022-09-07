using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator.FancyWrappers
{
    public class PropertyObservationWrapper<T> : INotifyDataChanged<PropertyObservationWrapper<T>>
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
        public object Tag { get; private set; }

        public event Action<PropertyObservationWrapper<T>, PropertyChangedEventArgs> PropertyChanged;
        public PropertyObservationWrapper(T value, object tag)
        {
            Value = value;
            Tag = tag;
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
