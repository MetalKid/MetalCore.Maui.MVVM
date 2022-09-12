using MetalCore.Chores.UI.Setup;

namespace MetalCore.Chores.UI.Common
{
    public interface IPageResolver
    {
        Page GetPageByViewModel<TViewModel>()
           where TViewModel : IViewModel;
    }

    public class PageResolver : IPageResolver
    {
        public Page GetPageByViewModel<TViewModel>()
           where TViewModel : IViewModel
        {
            var type = typeof(IPageViewModel<>).MakeGenericType(typeof(TViewModel));

            var page = IoCSetup.Container.GetInstance(type) as Page;
            return page;
        }
    }
}
