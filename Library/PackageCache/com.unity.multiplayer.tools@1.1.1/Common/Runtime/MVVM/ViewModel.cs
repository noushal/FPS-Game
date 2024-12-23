using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Unity.Multiplayer.Tools.Common
{
    interface IViewModel<TViewModel>
    {
        public delegate void ViewModelChangedEventHandler(TViewModel viewModel);
        public delegate void ViewModelChangedPropertyEventHandler(TViewModel viewModel, PropertyChangedEventArgs eventArgs);

        event ViewModelChangedEventHandler ViewModelChanged;
        event ViewModelChangedPropertyEventHandler PropertyChanged;
    }

    class ViewModel<TViewModel> : IViewModel<TViewModel>
        where TViewModel : ViewModel<TViewModel>
    {
        public event IViewModel<TViewModel>.ViewModelChangedEventHandler ViewModelChanged;
        public event IViewModel<TViewModel>.ViewModelChangedPropertyEventHandler PropertyChanged;

        // From: https://stackoverflow.com/questions/1315621/implementing-inotifypropertychanged-does-a-better-way-exist
        protected bool SetField<TProperty>(ref TProperty field, TProperty value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<TProperty>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnViewModelChanged()
        {
            ViewModelChanged?.Invoke((TViewModel)this);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnViewModelChanged();
            PropertyChanged?.Invoke((TViewModel)this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
