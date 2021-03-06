﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace ConsumerBehavior.ViewModels {

    class BasicViewModel : INotifyPropertyChanged
    {

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}