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

        public MainWindowViewModel()
        {
            Functions = new ObservableCollection<Function>();
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            var names = new string[4] { "Аддитивная", "Логарифмическая", "Квадратичная", "Мультипликативная" };
            var formulas = new string[4] { @"U(x)=\sum_{j=1}^{n}\alpha_{j}x_{j}^{\beta_{j}}", @"U(x)=\sum_{j=1}^{n}\alpha_{j}ln(x_{j})",
                @"U(x)=\sum_{j=1}^{n}\alpha_{j}x_{j}+\frac{1}{2}\sum_{i=1}^{n}\sum_{j=1}^{n}b_{ij}x_{i}x_{j}",@"U(x)=a\prod_{j=1}^{n}x_{j}^{\alpha_{j}}"};
            IsVisibleParams = Visibility.Hidden;
            var funcs = new ICommand[4] { new AdditiveCommand(this), new LogariphmicCommand(this), new QuadCommand(this), new MultiplicativeCommand(this) };
            for (int i = 0; i < 4; i++)
            {
                Functions.Add(new Function() { Name = names[i], SourceFormula = RenderFormula(formulas[i]), Func = funcs[i] });
            }

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
                                if(int.Parse(matchs[i].Groups[1].Value) != i+1)
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
        


        public string SetText(string text)
        {
            return "\\text{" + text + "}";
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

       

        private int _countParams = 2;

        public int CountParams
        {
            get { return _countParams; }
            set
            {
                _countParams = value;
                OnPropertyChanged(nameof(CountParams));
            }
        }


        private double _mParam = 100.0;

        public double MParam
        {
            get { return _mParam; }
            set
            {
                _mParam = value;
                OnPropertyChanged(nameof(MParam));
            }
        }

        private string _uParams = "1 * ln(x1) + 2.5 * ln(x2)";

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

     

        private string _pParams = "p1 = 1; p2 = 4.5";

        public string PParams
        {
            get { return _pParams; }
            set
            {
                _pParams = value;
                OnPropertyChanged(nameof(PParams));
            }
        }

       

        private string _fAQP = "Пример:\np1=1; p2=2.1; p3 = 3,14\nИндексы при p должны идти по возрастанию";

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