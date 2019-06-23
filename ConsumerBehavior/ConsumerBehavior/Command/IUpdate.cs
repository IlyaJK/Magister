using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConsumerBehavior.Command
{
    public interface IUpdate : ICommand
    {
        void UpdateFAQInfo();
    }
}
