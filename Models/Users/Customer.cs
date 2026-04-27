using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CakeProject.Models.Users;

// Tambahin INotifyPropertyChanged di sebelah User
public class Customer : User, INotifyPropertyChanged
{
    public bool IsMember { get; set; }

    // Rombak TotalPoin jadi full property
    private int _totalPoin;
    public int TotalPoin 
    { 
        get => _totalPoin; 
        set 
        {
            if (_totalPoin != value)
            {
                _totalPoin = value;
                OnPropertyChanged(); // Ini yang bikin UI auto-refresh
            }
        } 
    }

    public (bool success, string message, int diskon) TukarPoin(int poinYangDitukar)
    {
        if (!IsMember)
            return (false, "Hanya member yang bisa tukar poin.", 0);

        if (poinYangDitukar <= 0)
            return (false, "Jumlah poin tidak valid.", 0);

        if (poinYangDitukar > TotalPoin)
            return (false, "Poin tidak cukup.", 0);

        if (poinYangDitukar < 100)
            return (false, "Minimal penukaran 100 poin.", 0);

        // Hitung diskon (100 poin = 10.000)
        int diskon = (poinYangDitukar / 100) * 10000;

        // Kurangi poin (Ini otomatis manggil set block di atas dan nge-trigger UI refresh!)
        TotalPoin -= poinYangDitukar;

        return (true, "Penukaran poin berhasil.", diskon);
    }

    // --- Boilerplate INotifyPropertyChanged ---
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}