using ConsumerBehavior;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
//using ConsumerBehavior.Models;
using ConsumerBehavior.Command;
using System.Collections.Generic;
using WpfMath;
using System.Text;
using WpfMath.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Linq;
using System.Windows.Controls;

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

    

        private BitmapSource _sourceFormulaNear;

        public BitmapSource SourceFormulaNear
        {
            get { return _sourceFormulaNear; }
            set
            {
                _sourceFormulaNear = value;
                OnPropertyChanged(nameof(SourceFormulaNear));

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

                       SelectedFunction.IsVisibleFormula = Visibility.Collapsed;
                       SourceFormulaNear = SelectedFunction.SourceFormula;
                   });
                }
                return _onSelectionComboBoxFunction;
            }
        }


        public MainWindowViewModel()
        {
            Functions = new ObservableCollection<Function>();
          
            var names = new string[4] { "Аддитивная", "Логарифмическая", "Квадратичная", "Мультипликативная" };
            var formulas = new string[4] { @"U_1(x)=\sum_{j=1}^{n}\alpha_{j}ln(x_{j})", @"U_2(x)=\sum_{j=1}^{n}\alpha_{j}ln(x_{j})",
                @"U_3(x)=\sum_{j=1}^{n}\alpha_{j}ln(x_{j})", @"U_4(x)=\sum_{j=1}^{n}\alpha_{j}ln(x_{j})" };
            foreach (var nameFormula in names.Zip(formulas, (n, f) => new string[2] { n, f }))
            {
                
                var parser = new TexFormulaParser();
                var formula = parser.Parse(nameFormula[1]);
                //var renderer = formula.GetRenderer(TexStyle.Display, 15.0, "Arial");
                //var geometry = renderer.RenderToGeometry(0, 0);
                //var converter = new SVGConverter();
                //var svgPathText = converter.ConvertGeometry(geometry);
                //var svgText = AddSVGHeader(svgPathText);
                
                var renderer = formula.GetRenderer(TexStyle.Display, 15.0, "Arial");
                var bitmapSource = renderer.RenderToBitmap(0.0, 0.0);




                Functions.Add(new Function() { Name = nameFormula[0], SourceFormula = bitmapSource });
            }
            
        }

        private string AddSVGHeader(string svgText)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>")
                .AppendLine("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" >")
                .AppendLine(svgText)
                .AppendLine("</svg>");

            return builder.ToString();
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