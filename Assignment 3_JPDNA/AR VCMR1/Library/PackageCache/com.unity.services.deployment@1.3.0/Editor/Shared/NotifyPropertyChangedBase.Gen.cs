// WARNING: Auto generated code. Modifications will be lost!
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Unity.Services.Deployment.Editor.Shared
{
    abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(caller));
            }
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string caller = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(caller);
            return true;
        }
    }
}
