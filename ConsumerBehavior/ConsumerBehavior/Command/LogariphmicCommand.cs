using ConsumerBehavior.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace ConsumerBehavior.Command
{
    class LogariphmicCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private MainWindowViewModel _mainWindowViewModel;

        public LogariphmicCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            string pat = @"(?:\s*(\d+(?:[.,]\d+)?)\s*[*]\s*ln\s*[(]\s*x\d+\s*[)]\s*(?:[+]\s*)?)+";
            var reg = new Regex(pat, RegexOptions.IgnoreCase);
            var matchs = reg.Matches(_mainWindowViewModel.TextUParams);
            foreach (var match in matchs)
            {

            }

        }
    }
}
