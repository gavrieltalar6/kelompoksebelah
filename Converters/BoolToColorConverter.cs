using System.Globalization;

namespace CakeProject.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isAktif)
        {
            return isAktif 
                ? Color.FromArgb("#46f311") // Hijau jika aktif
                : Color.FromArgb("#D6B3D9"); // Ungu jika tidak
        }
        return Color.FromArgb("#D6B3D9");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        => throw new NotImplementedException();
}