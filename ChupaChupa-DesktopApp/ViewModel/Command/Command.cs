using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chupachupa_DesktopApp.ViewModel
{
    public class Command : ICommand
    {
        private Action _action;
        private Boolean _execute;
        public Boolean CanExec
        {
            set
            {
                if (_execute != value)
                {
                    _execute = value;
                    if (CanExecuteChanged != null)
                        CanExecuteChanged(this, new EventArgs());
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return (_execute);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            this._action();
        }

        public Command(Action a)
        {
            _action = a;
            this.CanExec = true;
        }

        public Command(Action a, Boolean b)
        {
            _action = a;
            this.CanExec = b;
        }
    }
}
