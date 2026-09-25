using DailyFortune.WinUI.Utilities;
using Microsoft.UI.Xaml.Data;

namespace DailyFortune.WinUI.Converters;

public class StringToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => Constants.Brush(value as string);
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
