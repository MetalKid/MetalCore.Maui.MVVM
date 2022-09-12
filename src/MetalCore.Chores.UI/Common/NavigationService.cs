using System.Reflection;

namespace MetalCore.Chores.UI.Common
{
    public interface INavigationService
    {
        Page CreatePageFromViewModel<TViewModel>()
            where TViewModel : IViewModel;

        Task NavigateToAsync<TViewModel>(object dataToPass = null, bool animated = true)
            where TViewModel : IViewModel;

        Task NavigateBackAsync(bool animated = true);
    }

    public class NavigationService : INavigationService
    {
        private readonly IPageResolver _pageResolver;

        public NavigationService(IPageResolver pageResolver)
        {
            _pageResolver = pageResolver;
        }

        private INavigation Navigation
        {
            get => Application.Current?.MainPage?.Navigation;
        }

        public Page CreatePageFromViewModel<TViewModel>()
            where TViewModel : IViewModel
        {
            var page = _pageResolver.GetPageByViewModel<TViewModel>();
            if (page == null)
                throw new InvalidOperationException($"Unable to resolve type {typeof(TViewModel).FullName}");

            var vm = GetViewModel(page);

            if (vm != null)
                page.Loaded += async (sender, e) => { await vm.LoadAsync(); };

            return page;
        }

        public async Task NavigateToAsync<TViewModel>(object dataToPass = null, bool animated = true)
            where TViewModel : IViewModel
        {
            var page = CreatePageFromViewModel<TViewModel>();

            var vm = GetViewModel(page);
            if (vm != null)
                vm.GetType().GetMethod("Prepare", BindingFlags.Public | BindingFlags.Instance, new Type[] { dataToPass.GetType() })?.Invoke(vm, new object[] { dataToPass });

            await Application.Current?.MainPage?.Navigation.PushAsync(page, animated);
        }

        public async Task NavigateBackAsync(bool animated = true)
        {
            await Navigation.PopAsync(animated);
        }

        private IViewModel GetViewModel<TPage>(TPage page)
            where TPage : Page
        {
            var prop = page.GetType().GetProperty("ViewModel", BindingFlags.Public | BindingFlags.Instance);
            return prop?.GetGetMethod()?.Invoke(page, null) as IViewModel;
        }
    }
}