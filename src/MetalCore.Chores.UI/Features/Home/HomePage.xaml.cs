
namespace MetalCore.Chores.UI.Features.Home;

public partial class HomePage : ContentPage, IPageViewModel<HomePageViewModel>
{
    public HomePageViewModel ViewModel { get; }

	public HomePage(HomePageViewModel viewModel)
	{
        ViewModel = viewModel;
        BindingContext = viewModel;

		InitializeComponent();

        this.Loaded += HomePage_Loaded;
    }

    private void HomePage_Loaded(object sender, EventArgs e)
    {
        SetupBinding();
    }

    private void SetupBinding()
    {
        var binder = BindingHelper.Create(ViewModel);

        binder.WithControl(this)
            .For(c => c.BindTitle(), vm => vm.CounterText);

        binder.WithControl(btnCounter)
            .For(c => c.BindClick(), vm => vm.CounterCommand)
            .For(c => c.BindText(), vm => vm.Counter, Converters.FromIntToString, Converters.FromStringToInt);

        binder.WithControl(btnNext)
            .For(c => c.BindClick(), vm => vm.NextCommand);

        binder.WithControl(btnBack)
            .For(c => c.BindClick(), vm => vm.BackCommand);
    }
}