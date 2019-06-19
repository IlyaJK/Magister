﻿using ConsumerBehavior.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows;
using System.Collections.ObjectModel;
using Expr = MathNet.Symbolics.SymbolicExpression;
using ExprLast = MathNet.Symbolics.Expression;
using MathNet.Symbolics;


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

        private string _L;
        private string[] _variables;
        private double[] _x_star;
        private string _lambda = "lambda";
        private string _M = "M";
        private List<object> _lst;

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
            _pi = _xi = _pi_xi = _minus_pi_xi = _alpha_ln_xi = _L = "";
            _x_star = new double[_main.CountParams];
            _lst = new List<object>();
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
                    _variables[i] = "x_" + (i + 1);
                    if (int.Parse(matchs[i].Groups[2].Value) != i + 1)
                    {
                        errors += "Неправильный индекс x\n";
                        break;
                    }
                    _alpha[i] = double.Parse(_main.ConvertDotToComma(matchs[i].Groups[1]));
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
                    _alpha_ln_xi += _main.ToDotNumber(_alpha[i - 1]) + "*\\ln(x_" + i + ")";

                }
                else
                {
                    _alpha_ln_xi += _main.ToDotNumber(_alpha[i - 1]) + "*\\ln(x_" + i + ") + ";

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
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result + _alpha_ln_xi), Align = HorizontalAlignment.Center });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(p_values), Align = HorizontalAlignment.Center });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Решение:", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("1. Решим задачу оптимального поведения потребителя, если цены благ соответственно равны ", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(p_values + _main.SetText(" и доход равен ") + "M = " + _main.MParam + ".") });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Найдем функции спроса потребителя:", true)) });

            for (int i = 1; i <= _main.CountParams; i++)
            {
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula("x_" + i + " = f_" + i + "(" + _pi + ",M);"), Align = HorizontalAlignment.Center });
            }

            result = _main.SetText("где ");
            var blag = "";
            for (int i = 1; i <= _main.CountParams; i++)
            {
                _xi += "x_" + i;
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
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula("x_j \\geq 0, j = 1, " + _main.CountParams + _main.SetText("           (1)")), Align = HorizontalAlignment.Center });


            for (int i = 1; i <= _main.CountParams; i++)
            {
                _pi_xi += "x_" + i + "*p_" + i;
                _minus_pi_xi += "- x_" + i + "*p_" + i;

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


            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + result) });
            result = "L(" + _xi + ",\\lambda) = " + _alpha_ln_xi + " + \\lambda(M " + _minus_pi_xi + ").";
            _L = _alpha_ln_xi + "+" + _lambda + "*(" + _M + _minus_pi_xi + ")";
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + result) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Найдем частные производные функции Лагранжа и приравняем их к нулю:", true)) });

            for (int i = 0; i < _main.CountParams; i++)
            {
                _lst.Add(_main.Diff(_L.Replace("\\", ""), _variables[i]));
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + @"\frac{\partial L}{\partial x_" + (i + 1) + "} = " + (_lst[i] as Expr).ToLaTeX().Replace(_lambda, @"\lambda") + " = 0") });
            }

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + @"\frac{\partial L}{\partial \lambda } = " + _M + _minus_pi_xi + " = 0") });
            _main.ResultCollection.Add(_main.RedLine);
            for (int i = 0; i < _lst.Count; i++)
            {
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + (_lst[i] as Expr).Substitute(Expr.Parse("p_" + (i + 1)), Expr.Parse("0")).ToLaTeX() + " = " + @"\lambda*p_" + (i + 1)) });
            }
            _main.ResultCollection.Add(_main.RedLine);
            for (int i = 0; i < _lst.Count; i++)
            {

                var root = _main.FindRoot(ExprLast.Symbol("x_" + (i + 1)), Infix.ParseOrThrow((_lst[i] as Expr).ToString()));
                _lst.RemoveAt(i);
                _lst.Insert(i, root);

                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + "x_" + (i + 1) + " = " + (_lst[i] as Expr).ToLaTeX().Replace(_lambda, @"\" + _lambda)) });

            }
            _main.ResultCollection.Add(_main.RedLine);
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + _pi_xi + " = " + _M + _main.SetText("          (4)")) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Подставим найденные значения ", true) + _xi + _main.SetText(" в (4) и выразим ") + @"\" + _lambda) });
            result = _pi_xi;
            for (int i = 0; i < _lst.Count; i++)
            {
                var expr = Expr.Parse(result).Substitute(Expr.Parse("x_" + (i + 1)), _lst[i] as Expr);
                result = _main.ConvertExprLastToExpr(Rational.Simplify(ExprLast.Symbol("x_" + (i + 1)), _main.ConvertExprToExprLast(expr))).ToString();
            }
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + Expr.Parse(result).ToLaTeX().Replace(_lambda, @"\" + _lambda) + " = " + _M) });

            result += "-" + _M;
            var lam_star = _main.FindRoot(ExprLast.Symbol(_lambda), Infix.ParseOrThrow(result));
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + @"\lambda^* = " + lam_star.ToLaTeX()) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + _main.SetText(@"Найдем оптимальный набор благ потребителя.")) });
            result = @"X^* = (";

            for (int i = 0; i < _main.CountParams; i++)
            {
                result += "x_" + (i + 1) + "^*";
                if (i < _main.CountParams - 1)
                {
                    result += ",";
                }
            }
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + result + ")" + _main.SetText(@" при заданных ценах на блага и доходе:")) });
            _main.ResultCollection.Add(_main.RedLine);



            for (int i = 0; i < _main.CountParams; i++)
            {
                var res = (_lst[i] as Expr).Substitute(Expr.Parse(_lambda), lam_star);
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + "x_" + (i + 1) + "^* = " + res.ToLaTeX()) });
                _lst[i] = res;
            }
            _main.ResultCollection.Add(_main.RedLine);
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + p_values + ", M = " + _main.ConvertCommaToDot(_main.MParam)) });
            _main.ResultCollection.Add(_main.RedLine);

            for (int i = 0; i < _main.CountParams; i++)
            {
                var res = (_lst[i] as Expr).Substitute(Expr.Parse(_M), Expr.Parse(_main.ConvertCommaToDot(_main.MParam))).Substitute(Expr.Parse("p_" + (i + 1)), Expr.Parse(_main.ConvertCommaToDot(_main.PValuesParams[i])));
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + "x_" + (i + 1) + "^* = " + res.ToLaTeX()) });
                _x_star[i] = double.Parse(_main.ConvertDotToComma(res));
            }

            result = @"X^* = (";

            for (int i = 0; i < _main.CountParams; i++)
            {
                result += "x_" + (i + 1) + "^*";
                if (i < _main.CountParams - 1)
                {
                    result += ",";
                }
            }

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Проверим, выполняется ли для найденного оптимального решения ", true) + result + _main.SetText(") бюджетное ограничение:")) });

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_pi_xi + " = M"), Align = HorizontalAlignment.Center });

            result = "";

            for (int i = 0; i < _main.CountParams; i++)
            {
                result += _main.ConvertCommaToDot(_main.PValuesParams[i]) + "*" + _main.ConvertCommaToDot(_x_star[i]);
                if (i < _main.CountParams - 1)
                {
                    result += "+";
                }
            }
            _main.ResultCollection.Add(_main.RedLine);
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result + " = " + _main.ConvertCommaToDot(_main.MParam)), Align = HorizontalAlignment.Center });
            _main.ResultCollection.Add(_main.RedLine);
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(Expr.Parse(result).ToLaTeX() + " = " + _main.ConvertCommaToDot(_main.MParam)), Align = HorizontalAlignment.Center });


            result = @"\overline{ X}^ * = (" + _x_star.Aggregate("", (b, n) => (!string.IsNullOrEmpty(b) ? Math.Truncate(double.Parse(b)).ToString() + ";" : "") + Math.Truncate(n).ToString()) + ")";
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Если речь идет о неделимых благах, то оптимальный выбор потребителя составит ", true) + result + ",") });

            result = _main.SetText(" т.е. ему необходимо приобрести ");
            for (int i = 0; i < _main.CountParams; i++)
            {
                result += (i + 1) + _main.SetText("-го блага - ") + _main.ConvertCommaToDot(Math.Truncate(_x_star[i]));

                if (i < _main.CountParams - 2)
                {
                    result += ", ";
                }
                if (i < _main.CountParams - 1)
                {
                    result += _main.SetText(" и ");
                }
            }         

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result + ".")});
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Но т.к. мы условились, что речь будет идти о делимых благах, то оптимальный выбор потребителя будет:", true)) });

            result = @"\overline{ X}^ * = (" + _x_star.Aggregate("", (b, n) => (!string.IsNullOrEmpty(b) ? _main.ConvertCommaToDot(b) + ";" : "") + _main.ConvertCommaToDot(n.ToString())) + ")";
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + result + _main.SetText(", т.е. следует приобрести: ")) });


            for (int i = 0; i < _main.CountParams; i++)
            {
                result = _main.SetText("блага ", true) + (i + 1) + _main.SetText("-го вида - ") + _main.ConvertCommaToDot(_x_star[i]);

                if (i < _main.CountParams - 1)
                {
                    result += ", ";
                }
                if (i == _main.CountParams - 1)
                {
                    result += ".";
                }
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result) });
            }
            _main.ResultCollection.Add(_main.RedLine);
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Решение 2:", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Функции спроса потребителя найдены в пункте 1.", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Вычислим реакции потребителя при изменении дохода М и цен ", true) + _pi + _main.SetText(" в точке оптимума ") + @"\overline{ X}^ *.") });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Реакции потребителя при изменении дохода М:", true))});

            for (int i = 0; i < _main.CountParams; i++)
            {
                var res = _main.Diff((_lst[i] as Expr).ToString(), "M");
                var p = Expr.Parse(_main.ConvertCommaToDot(_main.PValuesParams[i].ToString()));
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + @"\frac{\partial x_" + (i+1) + @"}{\partial M } = " + res.ToLaTeX() + @"\approx" + res.Substitute(Expr.Parse("p_" + (i+1)), p).ToLaTeX()) });
            }
            
        }

    }
}
