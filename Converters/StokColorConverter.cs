using System.Globalization;

namespace CakeProject.Converters;

public class StokColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double stok)
        {
            if (stok <= 0) return Colors.Red;
            if (stok <= 3) return Color.FromArgb("#F3D37A"); // kuning
            return Color.FromArgb("#46f311"); // hijau
        }
        return Colors.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}