namespace MetalCore.Chores.UI;

public partial class App : Application
{
	public App(INavigationService navigationService)
	{
		InitializeComponent();

		MainPage = navigationService.CreatePageFromViewModel<AppShellViewModel>();
	}
}
