using ConsumerBehavior.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows;

namespace ConsumerBehavior.Command
{
    class AdditiveCommand : IUpdate
    {
        public event EventHandler CanExecuteChanged;

        private MainWindowViewModel _main;

        private double[] _alpha;
        private double[] _betta;
        private string _alpha_x_st_betta;
        private string _pi;
        private List<object> _lst;
        private double[] _x_star;


        public void UpdateFAQInfo()
        {
            TestData();
            _main.FAQCoeffs = "Пример:\n1.5*x1^0.1 + 2*x2^0,2 + 3,7*x3^0.3\nИндексы при x должны идти по возрастанию\nЗначения в степени xi должны быть в (0,1)\nЗначения при xi должны быть строго больше 0";
        }

        private void TestData()
        {
            _main.UParams = "1.5*x1^0.1 + 2*x2^0,2 + 3,7*x3^0.3";
            _main.PParams = "p1 = 1; p2 = 2; p3 = 3";
            _main.MParam = 100.0;
            _main.CountParams = 3;
        }

        public AdditiveCommand(MainWindowViewModel mainWindowViewModel)
        {
            _main = mainWindowViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }


        public void Execute(object parameter)
        {
            _alpha_x_st_betta = _pi = "";
             _x_star = new double[_main.CountParams];
            _lst = new List<object>();
            var errors = (string)parameter;
            var pat = @"(?:(\d+(?:[.,]\d+)?)[*]x(\d+)\^(\d+(?:[.,]\d+)?)[+]?)+";
            var reg = new Regex(pat, RegexOptions.IgnoreCase);
            var match = reg.Match(_main.UParams.Replace(" ", ""));
            
            if (match.Groups[1].Captures.Count == _main.CountParams && _main.CountParams > 0)
            {
                _alpha = new double[_main.CountParams];
                _betta = new double[_main.CountParams];
                for (int i = 0; i < _main.CountParams; i++)
                {
                    
                    if (int.Parse(match.Groups[2].Captures[i].Value) != i + 1)
                    {
                        errors += "Неправильный индекс x\n";
                        break;
                    }
                    _betta[i] = double.Parse(_main.ConvertDotToComma(match.Groups[3].Captures[i]));
                    if (_betta[i] <= 0.0 || _betta[i] >= 1.0)
                    {
                        errors += "Значение бетта должны быть в (0,1)\n";
                        break;
                    }
                    _alpha[i] = double.Parse(_main.ConvertDotToComma(match.Groups[1].Captures[i]));
                    if (_alpha[i] <= 0.0)
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
            else
            {
                Algorithm();
            }
        }

        private void Algorithm()
        {
            _main.Header();

            var result = "U = ";
            var p_values = "";
            for (int i = 1; i <= _main.CountParams; i++)
            {
                if (i == _main.CountParams)
                {

                    _alpha_x_st_betta += _main.ConvertCommaToDot(_alpha[i - 1].ToString()) + "*x_" + i + "^{" + _main.ConvertCommaToDot(_betta[i - 1].ToString()) + "}";

                }
                else
                {
                    _alpha_x_st_betta += _main.ConvertCommaToDot(_alpha[i - 1].ToString()) + "*x_" + i + "^{" + _main.ConvertCommaToDot(_betta[i - 1].ToString()) + "} + ";

                }


                p_values += "p_" + i + " = " + _main.ToDotNumber(_main.PValuesParams[i - 1]);
                _pi += "p_" + i;
                if (i < _main.CountParams)
                {
                    _pi += "," + _main.SetText(" ");
                    p_values += ",";
                }
            }

            _main.ResultCollection.Add(_main.RedLine);
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result + _alpha_x_st_betta), Align = HorizontalAlignment.Center });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(p_values), Align = HorizontalAlignment.Center });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Решение:", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("1. Решим задачу оптимального поведения потребителя, если цены благ соответственно равны ", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(p_values + _main.SetText(" и доход равен ") + "M = " + _main.MParam + ".") });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Найдем функции спроса потребителя:", true)) });
        }


    }
}
