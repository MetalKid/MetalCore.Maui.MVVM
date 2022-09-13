using MetalCore.CQS.Mediators;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MetalCore.Chores.UI.Common
{
    public interface IViewModel : INotifyPropertyChanged
    {
        Task LoadAsync();
    }

    public class ViewModelBase : IViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected INavigationService NavigationService { get; }
        protected ICqsMediator Mediator { get; }

        public ViewModelBase(INavigationService navigationService, ICqsMediator mediator)
        {
            NavigationService = navigationService;
            Mediator = mediator;
        }

        protected void SetProperty<T>(ref T source, T value, [CallerMemberName] string methodName = null)
        {
            if (source?.Equals(value) == true)
                return;

            source = value;
            RaisePropertyChanged(methodName);
        }

        protected void RaisePropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected ICommand CreateCommand(Action callBack) =>
            new Command(callBack);

        protected ICommand CreateCommand(Action callBack, Func<bool> canExecute) =>
            new Command(callBack, canExecute);

        protected ICommand CreateCommand(Action<object> callBack) =>
            new Command(callBack);

        protected ICommand CreateCommand(Action<object> callBack, Func<object, bool> canExecute) =>
            new Command(callBack, canExecute);

        public virtual Task LoadAsync() =>
            Task.CompletedTask;
    }
}