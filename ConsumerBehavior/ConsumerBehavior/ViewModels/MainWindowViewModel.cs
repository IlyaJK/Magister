using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
//using ConsumerBehavior.Models;
using ConsumerBehavior.Command;
using WpfMath;
using System.Windows.Media.Imaging;
using System.Linq;

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


        private ICommand _onSelectionComboBoxFunction;

        public ICommand OnSelectionComboBoxFunction
        {
            get
            {
                if (_onSelectionComboBoxFunction == null)
                {
                    _onSelectionComboBoxFunction = new RelayCommand(param =>
                   {
                       IsVisibleUParams = Visibility.Visible;
                       TextUParams = "";
                   });
                }
                return _onSelectionComboBoxFunction;
            }
        }

        private ICommand _onClick;

        public ICommand OnClick
        {
            get
            {
                if (_onClick == null)
                {
                    _onClick = new RelayCommand(param =>
                    {
                        var log = new LogariphmicCommand(this);
                        log.Execute(null);
                    });
                }
                return _onClick;
            }
        }


        public MainWindowViewModel()
        {
            Functions = new ObservableCollection<Function>();

            var names = new string[4] { "Аддитивная", "Логарифмическая", "Квадратичная", "Мультипликативная" };
            var formulas = new string[4] { @"U(x)=\sum_{j=1}^{n}\alpha_{j}x_{j}^{\beta_{j}}", @"U(x)=\sum_{j=1}^{n}\alpha_{j}ln(x_{j})",
                @"U(x)=\sum_{j=1}^{n}\alpha_{j}x_{j}+\frac{1}{2}\sum_{i=1}^{n}\sum_{j=1}^{n}b_{ij}x_{i}x_{j}",@"U(x)=a\prod_{j=1}^{n}x_{j}^{\alpha_{j}}"};
            IsVisibleUParams = Visibility.Hidden;
            foreach (var nameFormula in names.Zip(formulas, (n, f) => (name: n, formula: f)))
            {
                Functions.Add(new Function() { Name = nameFormula.name, SourceFormula = RenderFormula(nameFormula.formula), IsVisibleFormula=Visibility.Visible });
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

        private Visibility _isVisibleUParams;

        public Visibility IsVisibleUParams
        {
            get { return _isVisibleUParams; }
            set
            {
                _isVisibleUParams = value;
                OnPropertyChanged(nameof(IsVisibleUParams));
            }
        }

        private string _textUParams;

        public string TextUParams
        {
            get { return _textUParams; }
            set
            {
                _textUParams = value;
                OnPropertyChanged(nameof(TextUParams));
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
    }



}