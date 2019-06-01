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

        private MainWindowViewModel _main;

        private double[] _alpha;

        public LogariphmicCommand(MainWindowViewModel mainWindowViewModel)
        {
            _main = mainWindowViewModel;
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
            var matchs = reg.Matches(_main.UParams.Replace(" ", ""));
            if (matchs.Count == _main.CountParams && _main.CountParams > 0)
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
            _main.ResultCollection = new ObservableCollection<Result>();
            var result =_main.SetText("Дана функция полезности ") + "U(";
             for (int i = 1; i <= _main.CountParams; i++)
             {
                 if (i == _main.CountParams)
                 {
                     result += "x_{" + i + "}). ";
                 }
                 else
                 {
                     result += "x_{" + i + "}, ";
                 }
             }
             result += _main.SetText(" Требуется");
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result) });

             result =_main.SetText("- решить задачу оптимального поведения при заданных ценах ");

            for (int i = 1; i <= _main.CountParams; i++)
            {
                result += "p_{" + i + "}";
                if (i < _main.CountParams)
                {
                    result += ", ";
                }
            }

            result += _main.SetText(" и доходе M.");

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result) });

            result = _main.SetText("- найти функцию опроса потребителя и вычислить реакции потребителя при изменении дохода и цен в точке ");

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result) });

            result = _main.SetText("оптимума.");

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result) });

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("- вычислить предельные полезности товаров в точке оптимума.")) });

            
            result = _main.SetText("- вычислить норму замещения для ");
            for (int i = 1; i <= _main.CountParams; i++)
            {
                result += _main.SetText(i + "-го");
                if (i < _main.CountParams-1)
                {
                    result += ", ";
                }
                else if (i == _main.CountParams-1)
                {
                    result += _main.SetText(" и ");
                }
            }

            result += _main.SetText(" товаров в точке оптимума.");

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result) });

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("- вычислить коэффициенты эластичности по доходу и ценам для заданных цен и дохода.")) });

            result = "U = ";
            string p = "";
            for (int i = 1; i <= _main.CountParams; i++)
            {
                if (i == _main.CountParams)
                {
                    result += _main.ToDotNumber(_alpha[i - 1]) + "\\ln(x_{" + i + "});";
                }
                else
                {
                    result += _main.ToDotNumber(_alpha[i - 1]) + "\\ln(x_{" + i + "}) + ";

                }
                p += "p_{" + i + "} = " + _main.ToDotNumber(_main.PValuesParams[i - 1]);
                if (i < _main.CountParams)
                {
                    p +=  "," + _main.SetText(" ");
                }
               

            }
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("отступ")), TopPadding=Visibility.Hidden} );
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result), Align= HorizontalAlignment.Center});

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(p), Align = HorizontalAlignment.Center });


        }
    }
}
