using ConsumerBehavior.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows;
using System.Collections.ObjectModel;

namespace ConsumerBehavior.Command
{
    class LogariphmicCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private MainWindowViewModel _mainWindowViewModel;

        private double[] _alpha;

        private string _result = "";

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
            var errors = (string)parameter;
            var pat = @"(\d+(?:[.,]\d+)?)[*]ln[(]x(\d+)[)]";
            var reg = new Regex(pat, RegexOptions.IgnoreCase);
            var matchs = reg.Matches(_mainWindowViewModel.UParams.Replace(" ", ""));
            if (matchs.Count == _mainWindowViewModel.CountParams && _mainWindowViewModel.CountParams > 0)
            {
                _alpha = new double[matchs.Count];
                for (int i = 0; i < matchs.Count; i++)
                {
                    if (int.Parse(matchs[i].Groups[2].Value) != i + 1)
                    {
                        errors += "Неправильный индекс x\n";
                        break;
                    }
                    _alpha[i] = double.Parse(matchs[i].Groups[1].Value.Replace('.', ','));
                    if (_alpha[i] <= 0)
                    {
                        errors += "Значение альфа не может быть отрицательным и равен 0\n";
                        break;
                    }
                }
            }
            else
            {
                errors += "Не соответствует кол-во параметров и введеных значений альфа\n";
            }

            if (!string.IsNullOrEmpty(errors))
            {
                MessageBox.Show(errors, "Ошибки", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Algorithm();
        }

        private void Algorithm()
        {
            /* _result += "\\text{Дана функция полезности } U(";
             for (int i = 1; i <= _mainWindowViewModel.CountParams; i++)
             {
                 if (i == _mainWindowViewModel.CountParams)
                 {
                     _result += "x_{" + i + "}). ";
                 }
                 else
                 {
                     _result += "x_{" + i + "}, ";
                 }
             }
             _result += "\\text{ Требуется:}";
             _result += "\\text{- решить}";*/

            _result += "U = ";
            string p = "";
            for (int i = 1; i <= _mainWindowViewModel.CountParams; i++)
            {
                if (i == _mainWindowViewModel.CountParams)
                {
                    _result += _alpha[i - 1] + "\\ln(x_{" + i + "});";
                }
                else
                {
                    _result += _alpha[i - 1] + "\\ln(x_{" + i + "}) + ";

                }
               
                p += "p_{" + i + "} = " + _mainWindowViewModel.PValuesParams[i - 1] + ", \\text{ }";
            }
            _mainWindowViewModel.ResultCollection = new ObservableCollection<Result>();
            _mainWindowViewModel.ResultCollection.Add(new Result() { ItemResult = _mainWindowViewModel.RenderFormula(_result) });
            p += "M = " + _mainWindowViewModel.MParam + ".";          
            _mainWindowViewModel.ResultCollection.Add(new Result() { ItemResult = _mainWindowViewModel.RenderFormula(p) });

            
        }
    }
}
