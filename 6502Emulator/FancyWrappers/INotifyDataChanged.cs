using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace _6502Emulator.FancyWrappers
{
    public interface INotifyDataChanged<T>
    {
        event Action<T, PropertyChangedEventArgs> PropertyChanged;
    }
}
