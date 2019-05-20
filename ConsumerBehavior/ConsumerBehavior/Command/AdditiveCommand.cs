﻿using ConsumerBehavior.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace ConsumerBehavior.Command
{
    class AdditiveCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private MainWindowViewModel _mainWindowViewModel;

        public AdditiveCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
           
        }
    }
}
