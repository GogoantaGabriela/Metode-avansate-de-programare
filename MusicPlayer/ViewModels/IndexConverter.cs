using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MusicPlayer.ViewModels;

public sealed class IndexConverter : IValueConverter
{
    public static readonly IndexConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is int idx ? (idx + 1).ToString() : "?";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => DependencyProperty.UnsetValue;
}


public sealed class GuidEqualConverter : IMultiValueConverter
{
    public static readonly GuidEqualConverter Instance = new();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        => values.Length == 2 && values[0] is Guid g1 && values[1] is Guid g2 && g1 == g2;

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
