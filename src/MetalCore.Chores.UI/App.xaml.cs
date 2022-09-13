using MetalCore.Chores.UI.Setup;
using SimpleInjector.Lifestyles;

namespace MetalCore.Chores.UI;

public partial class App : Application
{
    private readonly SimpleInjector.Scope _scope;

	public App()
	{
		InitializeComponent();

        // Run application code
        _scope = AsyncScopedLifestyle.BeginScope(IoCSetup.Container);
        var navigationService = _scope.Container.GetInstance<INavigationService>();
        MainPage = navigationService.CreatePageFromViewModel<AppShellViewModel>();
    }
}
