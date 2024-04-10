using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stride.Core.Presentation.Commands
{
    public class RoutedCommand<T> : RoutedCommand, ICommand
    {
        private readonly Action<T>? _cb;
        private readonly Func<T, Task>? _acb;
        private bool _busy;

        public RoutedCommand(Action<T> cb)
        {
            _cb = cb;
        }

        public RoutedCommand(Func<T, Task> cb)
        {
            _acb = cb;
        }

        private bool Busy
        {
            get => _busy;
            set
            {
                _busy = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        
        public override event EventHandler? CanExecuteChanged;

        public override bool CanExecute(object? parameter) => !_busy;

        public override async void Execute(object? parameter)
        {
            if(Busy)
                return;
            try
            {
                Busy = true;
                if (_cb != null)
                    _cb((T)parameter!);
                else
                    await _acb!((T)parameter!);
            }
            finally
            {
                Busy = false;
            }
        }
    }
    
    public abstract class RoutedCommand : ICommand
    {
        public static RoutedCommand Create(Action cb) => new RoutedCommand<object>(_ => cb());
        public static RoutedCommand Create<TArg>(Action<TArg> cb) => new RoutedCommand<TArg>(cb);
        public static RoutedCommand CreateFromTask(Func<Task> cb) => new RoutedCommand<object>(_ => cb());
        
        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
        public abstract event EventHandler CanExecuteChanged;
    }
}
