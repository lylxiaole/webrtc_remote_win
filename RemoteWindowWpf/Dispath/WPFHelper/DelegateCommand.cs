using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Dispath
{
    //Command类
    public delegate void method(object obj);
    public class DelegateCommand : ICommand
    {
        method method;
        public DelegateCommand(method fun)
        {
            method = fun;
        }

      
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            method.Invoke(parameter);
        }

        ~DelegateCommand()
        {
            method = null;

        }
    }
}
