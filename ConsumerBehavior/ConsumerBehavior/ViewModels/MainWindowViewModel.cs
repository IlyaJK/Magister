using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
//using ConsumerBehavior.Models;
using ConsumerBehavior.Command;
using WpfMath;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MathNet.Symbolics;
using Expr = MathNet.Symbolics.SymbolicExpression;
using ExprLast = MathNet.Symbolics.Expression;

namespace ConsumerBehavior.ViewModels
{
    class MainWindowViewModel : BasicViewModel
    {

        private ObservableCollection<Function> _functions;

        public ObservableCollection<Function> Functions
        {
            get { return _functions; }
            set
            {
                _functions = value;
                OnPropertyChanged(nameof(Functions));

            }
        }

        private ObservableCollection<Result> _resultCollection;

        public ObservableCollection<Result> ResultCollection
        {
            get { return _resultCollection; }
            set
            {
                _resultCollection = value;
                OnPropertyChanged(nameof(ResultCollection));

            }
        }

        public string ConvertCommaToDot<T>(T param)
        {
            return param.ToString().Replace(",", ".");
        }

        public string ConvertDotToComma<T>(T param)
        {
            return param.ToString().Replace(".", ",");
        }

        public string ConvertToDot<T>(T param)
        {
            return param.ToString().Replace(",", ".");
        }

        public Result RedLine { get; private set; }

        public MainWindowViewModel()
        {
            Functions = new ObservableCollection<Function>();
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            var names = new string[3] { "Аддитивная", "Логарифмическая", "Мультипликативная" };
            var formulas = new string[3] { @"U(x)=\sum_{j=1}^{n}\alpha_{j}x_{j}^{\beta_{j}}", @"U(x)=\sum_{j=1}^{n}\alpha_{j}ln(x_{j})", @"U(x)=a\prod_{j=1}^{n}x_{j}^{\alpha_{j}}"};
            IsVisibleParams = Visibility.Hidden;
           
            var funcs = new IUpdate[3] { new AdditiveCommand(this), new LogariphmicCommand(this), new MultiplicativeCommand(this) };
            for (int i = 0; i < 3; i++)
            {
                Functions.Add(new Function() { Name = names[i], SourceFormula = RenderFormula(formulas[i]), Func = funcs[i] });
            }
            RedLine = new Result() { ItemResult = RenderFormula(SetText("абзац")), TopPadding = Visibility.Hidden };
        }

        public Expr FindRoot(ExprLast variable, ExprLast expr)
        {

            ExprLast simple = Algebraic.Expand(Rational.Numerator(Rational.Simplify(variable, expr)));


            ExprLast[] coeff = Polynomial.Coefficients(variable, simple);
            switch (coeff.Length)
            {
                case 1: return ExprLast.Zero.Equals(coeff[0]) ? variable : ExprLast.Undefined;
                case 2: return Rational.Simplify(variable, Algebraic.Expand(-coeff[0] / coeff[1]));               
                default: return ExprLast.Undefined;
            }
        }

        public ExprLast ConvertExprToExprLast(Expr eq)
        {
            return Infix.ParseOrThrow(eq.ToString());
        }

        public Expr ConvertExprLastToExpr(ExprLast eq)
        {
            return eq;
        }

        public Expr Diff(string expr, string var)
        {
            return Expr.Parse(expr).Differentiate(Expr.Parse(var));
           
        }

        public string ReplaceVariablesToLatex(string input, string[] variables)
        {
            for (int i = 1; i <= CountParams; i++)
            {
                for (int j = 0; j < variables.Length; j++)
                {
                    input = input.Replace(variables[j] + i, @"\" + variables[j] + "_{" + i + "}");
                }
            }
            return input;
        }

        public void Header()
        {
            ResultCollection = new ObservableCollection<Result>();
            var result = SetText("Дана функция полезности ", true) + "U(";
            for (int i = 1; i <= CountParams; i++)
            {
                if (i == CountParams)
                {
                    result += "x_{" + i + "}). ";
                }
                else
                {
                    result += "x_{" + i + "}, ";
                }
            }
            result += SetText(" Требуется:");
            ResultCollection.Add(new Result() { ItemResult = RenderFormula(result) });

            result = SetText("- решить задачу оптимального поведения при заданных ценах ", true);

            for (int i = 1; i <= CountParams; i++)
            {
                result += "p_{" + i + "}";
                if (i < CountParams)
                {
                    result += ", ";
                }
            }

            result += SetText(" и доходе M.");

            ResultCollection.Add(new Result() { ItemResult = RenderFormula(result) });

            result = SetText("- найти функцию опроса потребителя и вычислить реакции потребителя при изменении дохода и цен в точке ", true);

            ResultCollection.Add(new Result() { ItemResult = RenderFormula(result) });

            result = SetText("оптимума.");

            ResultCollection.Add(new Result() { ItemResult = RenderFormula(result) });

            ResultCollection.Add(new Result() { ItemResult = RenderFormula(SetText("- вычислить предельные полезности товаров в точке оптимума.", true)) });


            result = SetText("- вычислить норму замещения для ", true);
            for (int i = 1; i <= CountParams; i++)
            {
                result += SetText(i + "-го");
                if (i < CountParams - 1)
                {
                    result += ", ";
                }
                else if (i == CountParams - 1)
                {
                    result += SetText(" и ");
                }
            }

            result += SetText(" товаров в точке оптимума.");

            ResultCollection.Add(new Result() { ItemResult = RenderFormula(result) });
            ResultCollection.Add(new Result() { ItemResult = RenderFormula(SetText("- вычислить коэффициенты эластичности по доходу и ценам для заданных цен и дохода.", true)) });


        }

        public double[] PValuesParams { get; set; }

        #region Triggers

        private ICommand _onSelectionComboBoxFunction;

        public ICommand OnSelectionComboBoxFunction
        {
            get
            {
                if (_onSelectionComboBoxFunction == null)
                {
                    _onSelectionComboBoxFunction = new RelayCommand(param =>
                    {
                        IsVisibleParams = Visibility.Visible;
                        SelectedFunction.Func.UpdateFAQInfo();
                    });
                }
                return _onSelectionComboBoxFunction;
            }
        }

        private ICommand _onClickSolve;

        public ICommand OnClickSolve
        {
            get
            {
                if (_onClickSolve == null)
                {
                    _onClickSolve = new RelayCommand(param =>
                    {
                        var errors = "";
                        if (CountParams <= 0)
                        {
                            errors += "Кол-во параметров должно быть больше 0\n";
                        }
                        if (MParam <= 0)
                        {
                            errors += "Параметр M должен быть больше 0\n";
                        }

                        var pat = @"p(\d+)=([-]?\d+(?:[.,]\d+)?)[;]?";
                        var reg = new Regex(pat, RegexOptions.IgnoreCase);
                        var matchs = reg.Matches(PParams.Replace(" ", ""));

                        if (matchs.Count == CountParams && CountParams > 0)
                        {
                            PValuesParams = new double[CountParams];
                            for (int i = 0; i < matchs.Count; i++)
                            {
                                if (int.Parse(matchs[i].Groups[1].Value) != i + 1)
                                {
                                    errors += "Неправильный индекс p\n";
                                    break;
                                }
                                PValuesParams[i] = double.Parse(matchs[i].Groups[2].Value.Replace('.', ','));
                                if (PValuesParams[i] < 0)
                                {
                                    errors += "Значение p не может быть отрицательным\n";
                                    break;
                                }
                            }
                        }
                        else
                        {
                            errors += "Не соответствует кол-во параметров и введеных значений p\n";
                        }


                        SelectedFunction.Func.Execute(errors);
                    });
                }
                return _onClickSolve;
            }
        }

        private ICommand _onClickFaqCoeffs;

        public ICommand OnClickFaqCoeffs
        {
            get
            {
                if (_onClickFaqCoeffs == null)
                {
                    _onClickFaqCoeffs = new RelayCommand(param =>
                    {

                        IsVisibleFAQCoeffs = IsVisibleFAQCoeffs == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
                    });
                }
                return _onClickFaqCoeffs;
            }
        }

        private ICommand _onClickFAQP;

        public ICommand OnClickFAQP
        {
            get
            {
                if (_onClickFAQP == null)
                {
                    _onClickFAQP = new RelayCommand(param =>
                    {

                        IsVisibleFAQP = IsVisibleFAQP == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
                    });
                }
                return _onClickFAQP;
            }
        }

        //private ICommand _onLoad;

        //public ICommand OnLoad
        //{
        //    get
        //    {
        //        if (_onLoad == null)
        //        {
        //            _onLoad =  new RelayCommand(param =>
        //            {

        //            });
        //        }
        //        return _onLoad;
        //    }
        //}

        #endregion

        private Visibility _isVisibleParams = Visibility.Hidden;

        public Visibility IsVisibleParams
        {
            get { return _isVisibleParams; }
            set
            {
                _isVisibleParams = value;
                OnPropertyChanged(nameof(IsVisibleParams));

            }
        }

        public BitmapSource RenderFormula(string data)
        {
            var parser = new TexFormulaParser();
            var formula = parser.Parse(data);
            var renderer = formula.GetRenderer(TexStyle.Display, 15.0, "Arial");

            return renderer.RenderToBitmap(0.0, 0.0);
        }

        private Function _selectedFunction;

        public Function SelectedFunction
        {
            get { return _selectedFunction; }
            set
            {
                _selectedFunction = value;
                OnPropertyChanged(nameof(SelectedFunction));
            }
        }



        public string SetText(string text, bool isTab = false)
        {
            return "\\text{" + (isTab ? "     " : "") + text + "}";
        }


        private string _caption = "Решение задачи оптимального поведения потребителя";

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }



        private int _countParams;

        public int CountParams
        {
            get { return _countParams; }
            set
            {
                _countParams = value;
                OnPropertyChanged(nameof(CountParams));
            }
        }


        private double _mParam;

        public double MParam
        {
            get { return _mParam; }
            set
            {
                _mParam = value;
                OnPropertyChanged(nameof(MParam));
            }
        }

        private string _uParams;

        public string UParams
        {
            get { return _uParams; }
            set
            {
                _uParams = value;
                OnPropertyChanged(nameof(UParams));
            }
        }

        private string _fAQCoeffs;

        public string FAQCoeffs
        {
            get { return _fAQCoeffs; }
            set
            {
                _fAQCoeffs = value;
                OnPropertyChanged(nameof(FAQCoeffs));
            }
        }

        private Visibility _isVisibleFAQCoeffs = Visibility.Hidden;

        public Visibility IsVisibleFAQCoeffs
        {
            get { return _isVisibleFAQCoeffs; }
            set
            {
                _isVisibleFAQCoeffs = value;
                OnPropertyChanged(nameof(IsVisibleFAQCoeffs));

            }
        }



        private string _pParams;

        public string PParams
        {
            get { return _pParams; }
            set
            {
                _pParams = value;
                OnPropertyChanged(nameof(PParams));
            }
        }


      
        private string _fAQP = "Пример:\np1=1; p2=2.1; p3 = 3,14\nИндексы при p должны идти по возрастанию\nЗначения pi должны быть строго больше 0";

        public string FAQP
        {
            get { return _fAQP; }
            set
            {
                _fAQP = value;
                OnPropertyChanged(nameof(FAQP));
            }
        }

        private Visibility _isVisibleFAQP = Visibility.Hidden;

        public Visibility IsVisibleFAQP
        {
            get { return _isVisibleFAQP; }
            set
            {
                _isVisibleFAQP = value;
                OnPropertyChanged(nameof(IsVisibleFAQP));

            }
        }
        
        public string ToDotNumber(double number)
        {
            return number.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        }

    }



}