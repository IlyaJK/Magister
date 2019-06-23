using ConsumerBehavior.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace ConsumerBehavior.Command
{
    class MultiplicativeCommand : IUpdate
    {
        public event EventHandler CanExecuteChanged;

        private MainWindowViewModel _main;

        public void UpdateFAQInfo()
        {
            TestData();
            _main.FAQCoeffs = "Пример:\n2.5*x1^0.1 * x2^0,2 * x3^0.3\nИндексы при x должны идти по возрастанию\nЗначения в степени xi должны быть в (0,1)\nПервое значение при x должно быть строго больше 0";
        }

        private void TestData()
        {
            _main.UParams = "2.5*x1^0.1 * x2^0,2 * x3^0.3";
            _main.PParams = "p1 = 1; p2 = 2; p3 = 3";
            _main.MParam = 100.0;
            _main.CountParams = 3;
        }

        public MultiplicativeCommand(MainWindowViewModel mainWindowViewModel)
        {
            _main = mainWindowViewModel;
            
           
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
           
        }
    }
}
