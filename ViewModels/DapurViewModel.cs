using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CakeProject.Models.Inventory;

namespace CakeProject.ViewModels;

public class DapurViewModel : INotifyPropertyChanged
{
    // Mengambil data real-time dari DatabaseService
    public ObservableCollection<StokBaku> ListBahanGudang { get; } = new();
    public ObservableCollection<StokProduk> ListKueTersedia { get; } = new();

    private async Task AmbilBahan()
    {
        if (BahanTerpilih == null || JumlahAmbil <= 0) return;

        bool berhasil = BahanTerpilih.KurangiStok(JumlahAmbil);
        if (!berhasil)
        {
            await Application.Current.MainPage.DisplayAlert("Gagal", "Stok tidak cukup!", "OK");
            return;
        }

        await App.TokoData.SimpanBahanAsync();
        JumlahAmbil = 0;
        BahanTerpilih = null;
        await Application.Current.MainPage.DisplayAlert("Sukses", "Bahan berhasil diambil!", "OK");
    }

    private async Task SimpanProduksi()
    {
        if (KueTerpilih == null || JumlahProduksi <= 0) return;

        KueTerpilih.TambahStok(JumlahProduksi);
        await App.TokoData.SimpanProdukAsync();
        JumlahProduksi = 0;
        KueTerpilih = null;
        await Application.Current.MainPage.DisplayAlert("Sukses", "Hasil produksi disimpan!", "OK");
    }
    private double _jumlahAmbil;
    public double JumlahAmbil
    {
        get => _jumlahAmbil;
        set { _jumlahAmbil = value; OnPropertyChanged(); }
    }

    private double _jumlahProduksi;
    public double JumlahProduksi
    {
        get => _jumlahProduksi;
        set { _jumlahProduksi = value; OnPropertyChanged(); }
    }

    public ICommand AmbilBahanCommand { get; }
    public ICommand SimpanProduksiCommand { get; }
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
        foreach (var b in App.TokoData.DaftarBahan) ListBahanGudang.Add(b);
        foreach (var p in App.TokoData.DaftarProduk) ListKueTersedia.Add(p);

        PilihSatuanProduksiCommand = new Command<string>(s => SatuanProduksiDipilih = s);
        PilihKueCommand = new Command<StokProduk>(k => KueTerpilih = k);

        AmbilBahanCommand = new Command(async () => await AmbilBahan());
        SimpanProduksiCommand = new Command(async () => await SimpanProduksi());
    }
    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}