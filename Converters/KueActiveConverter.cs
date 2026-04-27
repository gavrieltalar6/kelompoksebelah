using System.Globalization;

namespace CakeProject.Converters;

public class KueActiveConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // value = KueTerpilih (StokProduk), parameter = NamaBarang item ini
        if (value == null) return Color.FromArgb("#D6B3D9");
        
        var kueTerpilih = value as CakeProject.Models.Inventory.StokProduk;
        string namaParameter = parameter?.ToString() ?? "";
        
        return kueTerpilih?.NamaBarang == namaParameter
            ? Color.FromArgb("#46f311")  // aktif = hijau
            : Color.FromArgb("#D6B3D9"); // tidak aktif = ungu
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}