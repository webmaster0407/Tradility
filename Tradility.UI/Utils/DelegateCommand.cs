using System;
using System.Windows.Input;

namespace Tradility.UI.Utils
{
    public class DelegateCommand : ICommand
    {
        private readonly Action action;
        bool canExecute;

        public event EventHandler? CanExecuteChanged;


        public DelegateCommand(Action action)
        {
            this.action = action;
        }

        public void SetCanExecute(bool canExecute)
        {
            if (this.canExecute != canExecute)
            {
                this.canExecute = canExecute;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => action?.Invoke();
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> action;
        bool canExecute;

        public event EventHandler? CanExecuteChanged;


        public DelegateCommand(Action<T> action)
        {
            this.action = action;
        }

        public void SetCanExecute(bool canExecute)
        {
            if (this.canExecute != canExecute)
            {
                this.canExecute = canExecute;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => action?.Invoke((T)parameter!);
    }
}
