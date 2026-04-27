using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CakeProject.Models.Inventory;
using CakeProject.Services;

namespace CakeProject.ViewModels;

public class DapurViewModel : INotifyPropertyChanged
{
    // Mengambil data real-time dari DatabaseService
    public ObservableCollection<StokBaku> ListBahanGudang => DatabaseService.Instance.DaftarStokBaku;
    public ObservableCollection<StokProduk> ListKueTersedia => DatabaseService.Instance.DaftarStokProduk;

    private StokBaku _bahanTerpilih;
    public StokBaku BahanTerpilih
    {
        get => _bahanTerpilih;
        set { _bahanTerpilih = value; OnPropertyChanged(); }
    }

    private StokProduk _kueTerpilih;
    public StokProduk KueTerpilih
    {
        get => _kueTerpilih;
        set { _kueTerpilih = value; OnPropertyChanged(); }
    }

    private string _satuanProduksiDipilih = "Pcs";
    public string SatuanProduksiDipilih
    {
        get => _satuanProduksiDipilih;
        set { _satuanProduksiDipilih = value; OnPropertyChanged(); }
    }

    public ICommand PilihSatuanProduksiCommand { get; }
    public ICommand PilihKueCommand { get; }

    public DapurViewModel()
    {
        PilihSatuanProduksiCommand = new Command<string>(s => SatuanProduksiDipilih = s);
        
        // Command untuk memilih kue berdasarkan objek produknya
        PilihKueCommand = new Command<StokProduk>(k => KueTerpilih = k);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}