using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator.FancyWrappers
{
    [DebuggerDisplay("{Description}")]
    public class FancyFlag : INotifyDataChanged<FancyFlag> 
    {
        public string Description { get; }

        public FlagType Type { get; private set; }

        private bool hasValue;

        public event Action<FancyFlag, PropertyChangedEventArgs> PropertyChanged;

        public bool HasValue
        {
            get
            {
                return hasValue;
            }
            set
            {
                this.hasValue = value;

                OnPropertyChanged();
            }
        }
        public FancyFlag(string description, FlagType type)
        {
            Description = description;
            Type = type;
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
