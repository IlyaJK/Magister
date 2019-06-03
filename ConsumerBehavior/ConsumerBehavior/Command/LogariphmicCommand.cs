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
            else
            {
                Algorithm();
            }

           
        }

        private void Algorithm()
        {

            _main.Header();

            var result = "U = ";
            var u = "";
            var uWithU = "";
            string p = "";
            string p1 = "";
            for (int i = 1; i <= _main.CountParams; i++)
            {
                if (i == _main.CountParams)
                {
                    u += _main.ToDotNumber(_alpha[i - 1]) + "\\ln(x_{" + i + "})";
                }
                else
                {
                    u += _main.ToDotNumber(_alpha[i - 1]) + "\\ln(x_{" + i + "}) + ";
                }
                p += "p_{" + i + "} = " + _main.ToDotNumber(_main.PValuesParams[i - 1]);
                p1 += "p_{" + i + "}";
                if (i < _main.CountParams)
                {
                    p += "," + _main.SetText(" ");
                    p1 += ",";
                }
            }
            uWithU = result + u;
            result = uWithU;
            _main.ResultCollection.Add(_main.RedLine);
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result), Align = HorizontalAlignment.Center });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(p), Align = HorizontalAlignment.Center });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Решение:", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("1. Решим задачу оптимального поведения потребителя, если цены благ соответственно равны ", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(p + _main.SetText(" и доход равен ") + "M = " + _main.MParam + ".") });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Найдем функции спроса потребителя:", true)) });

            for (int i = 1; i <= _main.CountParams; i++)
            {
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula("x_{" + i + "} = f_{" + i + "}(" + p1 + ",M);"), Align = HorizontalAlignment.Center });
            }


            result = _main.SetText("где ");
            var blag = "";
            for (int i = 1; i <= _main.CountParams; i++)
            {
                result += "x_{" + i + "}";
                blag += i + "-го";
                if (i < _main.CountParams)
                {
                    result += ", ";

                }

                if (i < _main.CountParams - 1)
                {
                    blag += ", ";
                }
                else if (i == _main.CountParams - 1)
                {
                    blag += " и ";
                }
            }

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result + _main.SetText(" - количество приобретаемого блага " + blag + " вида.")) });

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Для этого решим следующую задачу оптимального поведения потребителя.", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Математическая модель задачи имеет вид:")) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula("x_{j} \\geq 0, j = 1, " + _main.CountParams + _main.SetText("                                                               (1)")), Align=HorizontalAlignment.Center });

            result = "";
            for (int i = 1; i <= _main.CountParams; i++)
            {
                result += "x_{" + i + "}" + "p_{" + i + "}";
                if (i < _main.CountParams)
                {
                    result += " + ";
                }
               
            }
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result + " = M" + _main.SetText("                                                               (2)")), Align = HorizontalAlignment.Center });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(uWithU + " \\rightarrow max" + _main.SetText("                                                      (3)")), Align = HorizontalAlignment.Center });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Левая часть условия (2) – стоимость приобретаемых благ, а условие (3) означает, что полезность этих", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("благ должна быть максимальной.")) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Решим задачу методом множителей Лагранжа. Функция Лагранжа:", true)) });
            result = "";
            var x = "";
            for (int i = 1; i <= _main.CountParams; i++)
            {
                x += "x_{" + i + "}";
                if (i < _main.CountParams)
                {
                    x += ",";
                }
            }
            result = "L(" + x + ",\\lambda) = U(" + x + ") + \\lambda(M - ";
            var xp = "";
            for (int i = 1; i <= _main.CountParams; i++)
            {
                xp += "x_{" + i + "}" + "p_{" + i + "}";
                if (i < _main.CountParams)
                {
                    xp += " - ";
                }
            }
            result += xp + ").";
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + result)});
            result = "L(" + x + ",\\lambda) = " + u + " + \\lambda(M - " + xp + ").";
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + result) });
        }
    }
}
