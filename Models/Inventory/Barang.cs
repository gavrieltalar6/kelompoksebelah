using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CakeProject.Models.Inventory
{
    public class Barang : INotifyPropertyChanged
    {
        public int IDBarang { get; set; }
        public string NamaBarang { get; set; }
        public int HargaBeli { get; set; }
        
        private double _jumlahStok;
        public double JumlahStok
        {
            get => _jumlahStok;
            set { _jumlahStok = value; OnPropertyChanged(); }
        }

        public double MinimalStok { get; set; }
        public string Satuan { get; set; }

        public void TambahStok(double jumlah)
        {
            JumlahStok += jumlah;
        }

        public bool KurangiStok(double jumlah)
        {
            if (JumlahStok >= jumlah)
            {
                JumlahStok -= jumlah;
                return true;
            }
            return false;
        }

        public bool CekStokRendah() => JumlahStok < MinimalStok;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}