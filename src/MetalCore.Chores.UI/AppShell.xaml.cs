using MetalCore.Chores.UI.Common;
using MetalCore.Chores.UI.Features.Home;
using static MetalCore.Chores.UI.AppShell;

namespace MetalCore.Chores.UI;

public partial class AppShell : Shell, IPageViewModel<AppShellViewModel>
{
    public AppShellViewModel ViewModel { get; }

    public AppShell(AppShellViewModel viewModel)
	{
		InitializeComponent();

        ViewModel = viewModel;

		this.Title = "MetalCore Chores";
	}
}

public class AppShellViewModel : ViewModelBase
{
    public AppShellViewModel(INavigationService navigationService) : base(navigationService)
    {
    }
}