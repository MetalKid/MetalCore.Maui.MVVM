using MetalCore.Chores.Domain.Features.Home.Queries.Count;
using MetalCore.CQS.Mediators;
using System.Windows.Input;

namespace MetalCore.Chores.UI.Features.Home
{
    public class HomePageViewModel : ViewModelBase
    {
        public ICommand BackCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand CounterCommand { get; }

        public HomePageViewModel(INavigationService navigationService, ICqsMediator mediator) : base(navigationService, mediator)
        {
            BackCommand = CreateCommand(() => NavigationService.NavigateBackAsync());
            NextCommand = CreateCommand(() => NavigationService.NavigateToAsync<HomePageViewModel>(_count));
            CounterCommand = CreateCommand(OnButtonClick);
        }

        private int _count;
        public int Counter
        {
            get => _count;
            set
            {
                SetProperty(ref _count, value, nameof(CounterText));
                RaisePropertyChanged(nameof(Counter));
            }
        }

        public string CounterText => $"You clicked the button {Counter} times!";

        public void Prepare(int count)
        {
            Counter = count;
        }

        public override async Task LoadAsync()
        {
            Counter += (await Mediator.ExecuteAsync(new CountQuery())).Data.InitialCount;
        }

        protected void OnButtonClick()
        {
            Counter++;

            SemanticScreenReader.Announce(CounterText);
        }
    }
}