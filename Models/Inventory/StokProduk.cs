using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CakeProject.Models.Inventory
{
    // Kita tetap pakai : BarangDijual, tapi tidak perlu deklarasi ulang PropertyChanged
    public class StokProduk : BarangDijual
    {
        public DateTime TglExpired { get; set; }

        private bool _isAktif;
        public bool IsAktif
        {
            get => _isAktif;
            set
            {
                if (_isAktif != value)
                {
                    _isAktif = value;
                    // Kita panggil OnPropertyChanged punya class 'Barang'
                    OnPropertyChanged();
                }
            }
        }

        // Method ini diletakkan di sini HANYA JIKA di class 'Barang' methodnya private.
        // Tapi kalau di class 'Barang' sudah ada protected OnPropertyChanged, 
        // bagian ini bisa dihapus total.
        public bool CekKadaluarsa()
        {
            return DateTime.Today > TglExpired.Date;
        }
    }
}