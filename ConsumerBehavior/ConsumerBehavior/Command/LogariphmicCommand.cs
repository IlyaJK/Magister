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

        private string _pi;
        private string _xi;
        private string _pi_xi;
        private string _minus_pi_xi;
        private string _alpha_ln_xi;

        private string _alpha_ln_xi_nl;
        private string[] _variables;
        private string[] _vars_no_index;
        private string _minus_pi_xi_nl;
        private string _L_nl;
        private string _lambda = "lambda";
        private string _M_nl = "M";

        public LogariphmicCommand(MainWindowViewModel mainWindowViewModel)
        {
            _main = mainWindowViewModel;
            _vars_no_index = new string[] { "x", "p" };
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _pi = _xi = _pi_xi = _minus_pi_xi = _alpha_ln_xi = _alpha_ln_xi_nl = _minus_pi_xi_nl = _L_nl = "";
            var errors = (string)parameter;
            var pat = @"(\d+(?:[.,]\d+)?)[*]ln[(]x(\d+)[)]";
            var reg = new Regex(pat, RegexOptions.IgnoreCase);
            var matchs = reg.Matches(_main.UParams.Replace(" ", ""));
            if (matchs.Count == _main.CountParams && _main.CountParams > 0)
            {
                _alpha = new double[matchs.Count];
                _variables = new string[matchs.Count];
                for (int i = 0; i < matchs.Count; i++)
                {
                    _variables[i] = "x" + (i + 1);
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
            var p_values = "";
            for (int i = 1; i <= _main.CountParams; i++)
            {
                if (i == _main.CountParams)
                {
                    _alpha_ln_xi += _main.ToDotNumber(_alpha[i - 1]) + "\\ln(x_{" + i + "})";
                    _alpha_ln_xi_nl += _main.ToDotNumber(_alpha[i - 1]) + "*ln(x" + i + ")";
                }
                else
                {
                    _alpha_ln_xi += _main.ToDotNumber(_alpha[i - 1]) + "\\ln(x_{" + i + "}) + ";
                    _alpha_ln_xi_nl += _main.ToDotNumber(_alpha[i - 1]) + "*ln(x" + i + ") + ";
                }
                

                p_values += "p_{" + i + "} = " + _main.ToDotNumber(_main.PValuesParams[i - 1]);
                _pi += "p_{" + i + "}";
                if (i < _main.CountParams)
                {
                    _pi += "," + _main.SetText(" ");
                    p_values += ",";
                }
            }
           
            _main.ResultCollection.Add(_main.RedLine);
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result + _alpha_ln_xi), Align = HorizontalAlignment.Center });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(p_values), Align = HorizontalAlignment.Center });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Решение:", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("1. Решим задачу оптимального поведения потребителя, если цены благ соответственно равны ", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(p_values + _main.SetText(" и доход равен ") + "M = " + _main.MParam + ".") });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Найдем функции спроса потребителя:", true)) });

            for (int i = 1; i <= _main.CountParams; i++)
            {
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula("x_{" + i + "} = f_{" + i + "}(" + _pi + ",M);"), Align = HorizontalAlignment.Center });
            }

            result = _main.SetText("где ");
            var blag = "";
            for (int i = 1; i <= _main.CountParams; i++)
            {
                _xi += "x_{" + i + "}";
                blag += i + "-го";
                if (i < _main.CountParams)
                {
                    _xi += ", ";

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

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result + _xi + _main.SetText(" - количество приобретаемого блага " + blag + " вида.")) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Для этого решим следующую задачу оптимального поведения потребителя.", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Математическая модель задачи имеет вид:")) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula("x_{j} \\geq 0, j = 1, " + _main.CountParams + _main.SetText("           (1)")), Align=HorizontalAlignment.Center });

         
            for (int i = 1; i <= _main.CountParams; i++)
            {
                _pi_xi += "x_{" + i + "}" + "p_{" + i + "}";
                _minus_pi_xi += "- x_{" + i + "}" + "p_{" + i + "}";
                _minus_pi_xi_nl += "-x" + i + "*p" + i;
                if (i < _main.CountParams)
                {
                    _pi_xi += " + ";
                    _minus_pi_xi += " ";
                }
               
            }
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_pi_xi + " = M" + _main.SetText("            (2)")), Align = HorizontalAlignment.Center });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula("U = " + _alpha_ln_xi + " \\rightarrow max" + _main.SetText("            (3)")), Align = HorizontalAlignment.Center });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Левая часть условия (2) – стоимость приобретаемых благ, а условие (3) означает, что полезность этих", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("благ должна быть максимальной.")) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Решим задачу методом множителей Лагранжа. Функция Лагранжа:", true)) });

            result = "L(" + _xi + ",\\lambda) = U(" + _xi + ") + \\lambda(M " + _minus_pi_xi + ").";
           
           
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + result)});
            result = "L(" + _xi + ",\\lambda) = " + _alpha_ln_xi + " + \\lambda(M " + _minus_pi_xi + ").";
            _L_nl = _alpha_ln_xi_nl + "+" + _lambda + "*(" + _M_nl + _minus_pi_xi_nl + ")";
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + result) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Найдем частные производные функции Лагранжа и приравняем их к нулю:", true)) });

            for (int i = 0; i < _main.CountParams; i++)
            {
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + @"\frac{\partial L}{\partial x_{" + (i + 1) + "}} = " + _main.ReplaceVariablesToLatex(_main.Diff(_L_nl, _variables[i]).ToLaTeX().Replace(_lambda, @"\lambda"), _vars_no_index) + " = 0") });
            }

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + @"\frac{\partial L}{\partial \lambda } = " + _M_nl + _minus_pi_xi + " = 0") });
        }
    }
}
