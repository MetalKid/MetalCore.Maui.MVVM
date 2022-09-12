using Microsoft.Maui.Controls;
using System.Globalization;
using System.Linq.Expressions;

namespace MetalCore.Chores.UI.Common
{
    public interface IValueConverter<TFrom, TTo>
    {
        TTo Convert(TFrom value, Type targetType, object parameter, CultureInfo culture);
        TFrom ConvertBack(TTo value, Type targetType, object parameter, CultureInfo culture);
    }

    public abstract class ValueConveterBase<TFrom, TTo> : IValueConverter<TFrom, TTo>, IValueConverter
    {
        public abstract TTo Convert(TFrom value, Type targetType, object parameter, CultureInfo culture);

        public abstract TFrom ConvertBack(TTo value, Type targetType, object parameter, CultureInfo culture);

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            this.Convert((TFrom)value, targetType, parameter, culture);

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            this.ConvertBack((TTo)value, targetType, parameter, culture);
    }

    public static class BindingHelper
    {
        public static BindingHelper<TViewModel> Create<TViewModel>(TViewModel viewModel)
            where TViewModel : IViewModel =>
            BindingHelper<TViewModel>.Create(viewModel);
    }

    public class BindingHelper<TViewModel> 
        where TViewModel : IViewModel
    {
        private readonly TViewModel _viewModel;

        public static BindingHelper<TViewModel> Create(TViewModel viewModel) => new BindingHelper<TViewModel>(viewModel);

        private BindingHelper(TViewModel viewModel) =>
            _viewModel = viewModel;

        public BindingControl<TViewModel, TControl> WithControl<TControl>(TControl control)
            where TControl : BindableObject => 
            new BindingControl<TViewModel, TControl>(_viewModel, control);
    }

    public class BindingControl<TViewModel, TControl>
        where TViewModel : IViewModel
        where TControl : BindableObject
    {
        private readonly TViewModel _viewModel;
        private readonly TControl _control;

        public BindingControl(TViewModel viewModel, TControl control) 
        {
            _viewModel = viewModel;
            _control = control;
        }

        public BindingControl<TViewModel, TControl> For(Expression<Func<TControl, BindableProperty>> bindPropertyExpression, Expression<Func<TViewModel, object>> propertyExpression)
        {
            var starting = propertyExpression.Parameters.First().Name + ".";
            _control.SetBinding(
                bindPropertyExpression.Compile()(_control), 
                new Binding(propertyExpression.Body.ToString().Replace(starting, string.Empty), 
                source: _viewModel));

            return this;
        }

        public BindingControl<TViewModel, TControl> For<TFrom, TTo>(
            Expression<Func<TControl, BindableProperty>> bindPropertyExpression, 
            Expression<Func<TViewModel, TFrom>> propertyExpression, 
            Func<TFrom, TTo> convertTo, 
            Func<TTo, TFrom> convertFrom = null)
        {
            var starting = propertyExpression.Parameters.First().Name + ".";
            _control.SetBinding(
                bindPropertyExpression.Compile()(_control), 
                new Binding(propertyExpression.Body.ToString().Replace(starting, string.Empty),
                converter: GenericConverter.Create(convertTo, convertFrom), 
                source: _viewModel));

            return this;
        }
        public BindingControl<TViewModel, TControl> OneWay(BindableProperty property, Expression<Func<TViewModel, object>> propertyExpression)
        {
            _control.SetValue(property, propertyExpression.Compile()(_viewModel));

            return this;
        }
    }
}
