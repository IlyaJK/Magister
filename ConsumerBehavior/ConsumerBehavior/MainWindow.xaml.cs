using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfMath;
using MathNet.Symbolics;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace ConsumerBehavior
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string Diff(Expr a, Expr b)
        {
            return @"\frac{\partial " + a.VariableName + "}{" + b.VariableName + "}";
        }

        public MainWindow()
        {
            InitializeComponent();
            var a = Expr.Variable("a");
            var b = Expr.Variable("b");

            var diff = a.Differentiate(a);

            // string latex = Expr.Parse("a_1_2_3^a^3^b/b_1^3").ToLaTeX();
            var latex = Diff(a, a) + " = " + diff.ToLaTeX();

            
            string fileName = @"formula.png";
            

            var parser = new TexFormulaParser();
            var formula = parser.Parse(latex);
            //var renderer = formula.GetRenderer(TexStyle.Display, 20.0, "Arial");
            //var bitmapSource = renderer.RenderToGeometry(0.0, 0.0);
            var pngBytes = formula.RenderToPng(20.0, 0.0, 0.0, "Arial");
            File.WriteAllBytes(fileName, pngBytes);
        }
    }
}
