using _6502Emulator.FancyWrappers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator
{
    public class PropertyChangeTracker<TKey, TValue>
    {
        public Action<PropertyObservationWrapper<TValue>, PropertyChangedEventArgs> OnPropChanged { get; }
        public Dictionary<TKey, TValue> ChangedPropValues { get; }

        public PropertyChangeTracker()
        {
            ChangedPropValues = new Dictionary<TKey, TValue>();

            OnPropChanged = new Action<PropertyObservationWrapper<TValue>, PropertyChangedEventArgs>((obj, args) =>
            {
                var name = (TKey)obj.Tag;
                if (ChangedPropValues.ContainsKey(name) == false)
                {
                    ChangedPropValues.Add(name, default);
                }
                ChangedPropValues[name] = obj.Value;
            });
        }
    }
}
