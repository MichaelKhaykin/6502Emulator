using _6502Emulator.FancyWrappers;
using _6502Emulator.VisualizationClasses;
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
        public Func<TKey, string> KeyDisplayMethod { get; }
        public PropertyChangeTracker()
        {
            var keyType = typeof(TKey);
            bool isNative = keyType.IsNativeType();
          
            if(isNative)
            {
                KeyDisplayMethod = new Func<TKey, string>(key => key.ToString());
            }
            else
            {
                var type = typeof(TKey).GetInterface("IVisualizeMeAsString");

                if(type == null)
                {
                    throw new ArgumentException($"{typeof(TKey)} is not native nor does it implement {type}");
                }

                KeyDisplayMethod = new Func<TKey, string>(key =>
                {
                    if(key is IVisualizeMeAsString casted)
                    {
                        return casted.VisualizeMe();
                    }

                    throw new ArgumentException("Key was null bucko");
                });
            }

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
