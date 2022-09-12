using System.Globalization;

namespace MetalCore.Chores.UI.Common
{
    public static class GenericConverter
    {
        public static GenericConverter<TFrom, TTo> Create<TFrom, TTo>(Func<TFrom, TTo> convertTo, Func<TTo, TFrom> convertFrom) =>
            new GenericConverter<TFrom, TTo>(convertTo, convertFrom);
    }

    public class GenericConverter<TFrom, TTo> : IValueConverter
    {
        private readonly Func<TFrom, TTo> _convertTo;
        private readonly Func<TTo, TFrom> _convertFrom;

        public GenericConverter(Func<TFrom, TTo> convertTo, Func<TTo, TFrom> convertFrom)
        {
            _convertTo = convertTo;
            _convertFrom = convertFrom;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (_convertTo == null)
                throw new NotImplementedException();

            return (TTo)_convertTo((TFrom)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (_convertFrom == null)
                throw new NotImplementedException();

            return (TFrom)_convertFrom((TTo)value);
        }
    }
}
