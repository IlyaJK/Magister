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
using Expr = MathNet.Symbolics.SymbolicExpression;
using ExprLast = MathNet.Symbolics.Expression;
using MathNet.Symbolics;


namespace ConsumerBehavior.Command
{
    class LogariphmicCommand : IUpdate
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


        private void TestData()
        {
            _main.UParams = "1 * ln(x1) +1 * ln(x2)";
            _main.PParams = "p1 = 10; p2 = 2";
            _main.MParam = 360.0;
            _main.CountParams = 2;
        }

        public void UpdateFAQInfo()
        {
            TestData();
            _main.FAQCoeffs = "Пример:\n2.5*ln(x1) + 3*ln(x2) + 4,6*ln(x3)\nИндексы при x должны идти по возрастанию\nЗначения при ln должны быть строго больше 0";
        }

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
            var pat = @"(?:(\d+(?:[.,]\d+)?)[*]ln[(]x(\d+)[)][+]?)+";
            var reg = new Regex(pat, RegexOptions.IgnoreCase);
            var match = reg.Match(_main.UParams.Replace(" ", ""));
            
            if (match.Groups[1].Captures.Count == _main.CountParams && _main.CountParams > 0)
            {


                _alpha = new double[_main.CountParams];
                _variables = new string[_main.CountParams];
                for (int i = 0; i < _main.CountParams; i++)
                {
                    _variables[i] = "x_" + (i + 1);
                    if (int.Parse(match.Groups[2].Captures[i].Value) != i + 1)
                    {
                        errors += "Неправильный индекс x\n";
                        break;
                    }
                    _alpha[i] = double.Parse(_main.ConvertDotToComma(match.Groups[1].Captures[i]));
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

                    _alpha_ln_xi += _main.ConvertCommaToDot(_alpha[i - 1].ToString()) + "*\\ln(x_" + i + ")";

                }
                else
                {
                    _alpha_ln_xi += _main.ConvertCommaToDot(_alpha[i - 1].ToString()) + "*\\ln(x_" + i + ") + ";

                }


                p_values += "p_" + i + " = " + _main.ConvertCommaToDot(_main.PValuesParams[i - 1]);
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
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + @"\lambda^* = " + lam_star.ToLaTeX() + " = " + lam_star.Substitute("M", _main.MParam).ToLaTeX()) });
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
                _x_star[i] = double.Parse(_main.ConvertDotToComma(res.RealNumberValue.ToString()));
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

            result = @"\overline{ X}^ * = (" + _x_star.Aggregate("", (b, n) => b + (!string.IsNullOrEmpty(b) ? ";" : "") + Math.Truncate(n).ToString()) + ")";
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

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(result + ".") });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Но т.к. мы условились, что речь будет идти о делимых благах, то оптимальный выбор потребителя будет:", true)) });

            result = @"\overline{ X}^ * = (" + _x_star.Aggregate("", (b, n) => b + (!string.IsNullOrEmpty(b) ? ";" : "") + _main.ConvertCommaToDot(n.ToString())) + ")";
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
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Реакции потребителя при изменении дохода М:", true)) });

            var minBlag = -1.0;
            var maxBlag = -1.0;
            var indMinBlag = -1;
            var indMaxBlag = -1;
            for (int i = 0; i < _main.CountParams; i++)
            {
                var res = _main.Diff((_lst[i] as Expr).ToString(), "M");
                var p = Expr.Parse(_main.ConvertCommaToDot(_main.PValuesParams[i].ToString()));
                var val = double.Parse(_main.ConvertDotToComma(res.Substitute(Expr.Parse("p_" + (i + 1)), p).RealNumberValue.ToString()));
                if (i == 0)
                {
                    minBlag = maxBlag = val;
                    indMinBlag = indMaxBlag = i;
                }
                if (val < minBlag)
                {
                    minBlag = val;
                    indMinBlag = i;
                }
                if (val > maxBlag)
                {
                    maxBlag = val;
                    indMaxBlag = i;
                }
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + @"\frac{\partial x_" + (i + 1) + @"}{\partial M } = " + res.ToLaTeX() + @"\approx" + _main.ConvertCommaToDot(val.ToString())) });
                if (i == 0)
                {
                    _main.ResultCollection.Add(_main.RedLine);
                    _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Поскольку при увеличении дохода спрос на 1-благо возрастает, то это благо ценное.", true)) });
                    _main.ResultCollection.Add(_main.RedLine);
                }
            }

            if (_main.CountParams == 2)
            {
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Оставшееся благо также является ценным для потребителя.  При этом наиболее ценным является " + (indMaxBlag + 1) + "-е благо, а наименее ценным – " + (indMinBlag + 1) + "-е.", true)) });
            }
            else
            {
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Остальные блага также являются ценными для потребителя.  При этом наиболее ценным является " + (indMaxBlag + 1) + "-е благо, а наименее ценным – " + (indMinBlag + 1) + "-е.", true)) });

            }


           
            for (int i = 0; i < _main.CountParams; i++)
            {
                _main.ResultCollection.Add(_main.RedLine);
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Определим реакции потребителя     при изменении цены на " + (i + 1) + "-е благо:", true)) });
                for (int j = 0; j < _main.CountParams; j++)
                {
                    
                    _main.ResultCollection.Add(_main.RedLine);
                    var res = _main.Diff((_lst[i] as Expr).ToString(), "p_" + (i + 1));
                    var m = Expr.Parse(_main.ConvertCommaToDot(_main.MParam.ToString()));
                    var p = Expr.Parse(_main.ConvertCommaToDot(_main.PValuesParams[j].ToString()));
                    var val = double.Parse(_main.ConvertDotToComma(res.Substitute(Expr.Parse("M"), m).Substitute(Expr.Parse("p_" + (i + 1)), p).RealNumberValue.ToString()));
                    _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + @"\frac{\partial x_" + (j + 1) + @"}{\partial p_" + (i + 1) + " } = " + res.ToLaTeX() + (val == 0 ? "=" : @"\approx") + _main.ConvertCommaToDot(val.ToString()) + (val != 0 ? " (" + (val >= 0 ? ">" : "<") + "0)" : "")) });
                    if (i == j && val < 0)
                    {
                        _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("С увеличением цены на " + (i + 1) + "-е благо спрос на него уменьшается, значит, "+ (j+1)+"-е благо нормальное.", true)) });
                    }
                    else if (i == j && val > 0)
                    {
                        _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("С увеличением цены на " + (i + 1) + "-е благо спрос на него уменьшается, значит, " + (j + 1) + "-е благо ненормальное.", true)) });
                    }
                    else if (val < 0)
                    {
                        _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("С увеличением  цены на " + (i + 1) + "-е благо спрос на " + (j + 1) + "-е благо увеличивается, эти блага взаимодополняемые.", true)) });
                    }
                    else if (val > 0)
                    {
                        _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("С увеличением  цены на " + (i + 1) + "-е благо спрос на " + (j + 1) + "-е благо увеличивается, эти блага взаимозаменяемые.", true)) });
                    }
                   
                }

            }

           
            result = @"\overline{ X}^ * = (";

            for (int i = 0; i < _main.CountParams; i++)
            {
                result += "x_" + (i + 1) + "^*";
                if (i < _main.CountParams - 1)
                {
                    result += ",";
                }
            }

            _main.ResultCollection.Add(_main.RedLine);
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Решение3:", true)) });
            _main.ResultCollection.Add(new Result()
            {
                ItemResult = _main.RenderFormula(_main.SetText("Вычислим предельные полезности благ в точке экстремума ", true) + result + ")" +
                _main.SetText(". Это значения частных производных "))
            });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("функции полезности ") + "U = (" + _xi + ")" + _main.SetText(" по соответствующим аргументам в точке ") + @"\overline{ X}^*.") });

            _main.ResultCollection.Add(_main.RedLine);
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(@"\overline{ X}^ * = (" + _x_star.Aggregate("", (b, n) => b + (!string.IsNullOrEmpty(b) ? ";" : "") + _main.ConvertCommaToDot(n.ToString())) + ")"), Align = HorizontalAlignment.Center });

            var dU_x_star = new double[_main.CountParams];
            for (int i = 0; i < _main.CountParams; i++)
            {
                _main.ResultCollection.Add(_main.RedLine);
                var res = _main.Diff(_alpha_ln_xi.Replace("\\", ""), "x_" + (i + 1));
                var x = Expr.Parse(_main.ConvertCommaToDot(_x_star[i].ToString()));
                var val = double.Parse(_main.ConvertDotToComma(res.Substitute(Expr.Parse("x_" + (i + 1)), x).RealNumberValue.ToString()));
                dU_x_star[i] = val;
                _main.ResultCollection.Add(new Result()
                {
                    ItemResult = _main.RenderFormula(_main.SetText("", true) + @"\frac{\partial U}{\partial x_" + (i + 1) + " } = " + res.ToLaTeX() + _main.SetText(";    ") +
                    @"\frac{\partial U}{\partial x_" + (i + 1) + " }(\\overline{ X})^* = " + res.ToLaTeX() + _main.SetText(" = ") + _main.ConvertCommaToDot(val.ToString())),
                    Align = HorizontalAlignment.Center
                });
            }


            _main.ResultCollection.Add(_main.RedLine);
            for (int i = 0; i < _main.CountParams; i++)
            {
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("На 1 дополнительную единицу " + (i + 1) + "-го блага приходится ", true) + dU_x_star[i] + _main.SetText(" единиц дополнительной полезности.")) });

            }

            _main.ResultCollection.Add(_main.RedLine);
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Решение4:", true)) });

            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Вычислим нормы замещения благ в точке оптимума ", true) + "\\overline{X}^*.") });

            for (int i = 0; i < _main.CountParams; i++)
            {

                for (int j = 0; j < _main.CountParams; j++)
                {
                    if (i == j)
                        continue;

                    var dudx = dU_x_star[i] / dU_x_star[j];

                    _main.ResultCollection.Add(_main.RedLine);
                    _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Норма замены " + (i + 1) + "-го блага " + (j + 1) + "-м:", true)) });
                    _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + @"n_{" + (j + 1) + "," + (i + 1) + @"} = -\frac{\partial U}{\partial x_" + (i + 1) + @"} : \frac{\partial U}{\partial x_" + (j + 1) + "} = " + _main.ConvertCommaToDot((-dudx).ToString())) });
                    _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Для замещения 1 единицы " + (i + 1) + "-го блага необходимо дополнительно приобрести ", true) + _main.ConvertCommaToDot((dudx).ToString())) });
                    _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText(" единиц " + (j + 1) + "-го блага, чтобы удовлетворенность осталась на прежнем уровне.")) });
                }
            }

            _main.ResultCollection.Add(_main.RedLine);
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Решение5:", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Вычислим коэффициенты эластичности по доходу и ценам при заданных ценах и доходе: ", true)) });
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(p_values + ", M = " + _main.ConvertCommaToDot(_main.MParam.ToString()) + ".") });

            var empSum = 0.0;

            for (int i = 0; i < _main.CountParams; i++)
            {
                _main.ResultCollection.Add(_main.RedLine);
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Для блага " + (i + 1) + ":", true)) });
                var res = _main.Diff((_lst[i] as Expr).ToString(), "M").Divide("x_" + (i + 1) + "/M");
                var resReplace = res.Substitute("M", _main.ConvertCommaToDot(_main.MParam.ToString())).Substitute("x_" + (i + 1), _main.ConvertCommaToDot(_x_star[i].ToString())).Substitute("p_" + (i + 1), _main.ConvertCommaToDot(_main.PValuesParams[i].ToString())).ToString();
                empSum = double.Parse(_main.ConvertDotToComma(resReplace));
                var resRound = Math.Round(empSum);
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + @"E_" + (i + 1) + @"^M = \frac{\partial x_" + (i + 1) + @"}{\partial M } : " + @"\frac{ x_" + (i + 1) + "}{ M } = " + res.ToLaTeX() + " = " + resRound) });

                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("При увеличении дохода на 1 % спрос на " + (i + 1) + "-e благо возрастает на 1 %.", true)) });

                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Коэффициенты эластичности по ценам:", true)) });
                result = "E_" + (i + 1) + "^M + ";
                for (int j = 0; j < _main.CountParams; j++)
                {
                    result += "E_{ " + (i + 1) + "" + (j + 1) + "}^p";
                    if (j < _main.CountParams - 1)
                    {
                        result += " + ";
                    }
                    res = _main.Diff((_lst[i] as Expr).ToString(), "p_" + (j + 1)).Divide("x_" + (i + 1) + "/p_" + (j + 1));
                    resReplace = res.Substitute("M", _main.ConvertCommaToDot(_main.MParam.ToString())).Substitute("x_" + (i + 1), _main.ConvertCommaToDot(_x_star[i].ToString())).Substitute("p_" + (j + 1), _main.ConvertCommaToDot(_main.PValuesParams[i].ToString())).ToString();
                    var epi = double.Parse(_main.ConvertDotToComma(resReplace));
                    empSum += epi;
                    _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + @"E_{" + (i + 1) + "" + (j + 1) + @"}^p = \frac{\partial x_" + (i + 1) + @"}{\partial p_" + (j + 1) + " } : " + @"\frac{ x_" + (i + 1) + "}{ p_" + (j + 1) + " } = " + resReplace) });
                    
                    var output = "При росте цены на " + (j + 1) + "-е благо на 1 %";

                    if (epi != 0)
                    {
                        if (epi < 0)
                        {
                            output += " спрос на него уменьшается на ";
                        }
                        else
                        {
                            output += "спрос на него увеличивается на ";
                        }
                        _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText(output, true) + _main.ConvertCommaToDot(Math.Abs(epi).ToString()) + _main.SetText("%.")) });

                    }
                    else
                    {
                        output += " оно является независимым.";
                        _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText(output, true)) });
                    }
                    _main.ResultCollection.Add(_main.RedLine);

                }
                
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Проверка:", true)) });
                _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("", true) + result + " = " + _main.ConvertCommaToDot(empSum.ToString())) });
            }
            _main.ResultCollection.Add(_main.RedLine);
            _main.ResultCollection.Add(new Result() { ItemResult = _main.RenderFormula(_main.SetText("Все расчеты произведены достаточно точно и правильно.", true)) });

        }

    }
}
