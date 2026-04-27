using System.Globalization;

namespace CakeProject.Converters;

public class SatuanColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() == parameter?.ToString()
            ? Color.FromArgb("#46f311")  // dipilih
            : Colors.Transparent;         // tidak dipilih
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}