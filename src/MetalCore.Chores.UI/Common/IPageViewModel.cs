namespace MetalCore.Chores.UI.Common
{
    public interface IPageViewModel
    { 
    }

    public interface IPageViewModel<T> : IPageViewModel
        where T: IViewModel
    {
        T ViewModel { get; }
    }
}
