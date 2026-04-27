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
        if (BahanTerpilih == null)
        {
            await Application.Current.MainPage.DisplayAlert("Peringatan", "Pilih bahan terlebih dahulu!", "OK");
            return;
        }
        if (JumlahAmbil <= 0)
        {
            await Application.Current.MainPage.DisplayAlert("Peringatan", "Masukkan jumlah yang valid!", "OK");
            return;
        }

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
        // PROTEKSI 1: Cek apakah user sudah pilih kue
        if (KueTerpilih == null)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Pilih jenis kue terlebih dahulu!", "OK");
            return;
        }

        // PROTEKSI 2: Cek jumlah produksi
        if (JumlahProduksi <= 0)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Jumlah produksi harus lebih dari 0!", "OK");
            return;
        }

        try
        {
            // Proses simpan
            KueTerpilih.TambahStok(JumlahProduksi);
            await App.TokoData.SimpanProdukAsync();

            // Reset UI setelah sukses
            foreach (var k in ListKueTersedia) k.IsAktif = false;
            KueTerpilih = null;
            JumlahProduksi = 0;

            await Application.Current.MainPage.DisplayAlert("Sukses", "Data produksi berhasil dicatat", "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Crash Tercegah", $"Terjadi kesalahan: {ex.Message}", "OK");
        }
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
        // Load data dari data source (App.TokoData)
        foreach (var b in App.TokoData.DaftarBahan) ListBahanGudang.Add(b);
        foreach (var p in App.TokoData.DaftarProduk) ListKueTersedia.Add(p);

        PilihSatuanProduksiCommand = new Command<string>(s => SatuanProduksiDipilih = s);

        // Satukan logika PilihKue di sini
        PilihKueCommand = new Command<StokProduk>(kue =>
        {
            if (kue == null) return;

            // 1. Matikan SEMUA tombol (Reset ke Ungu)
            foreach (var k in ListKueTersedia)
            {
                k.IsAktif = false;
            }

            // 2. Aktifkan tombol yang baru diklik (Jadi Hijau)
            kue.IsAktif = true;

            // 3. Update referensi KueTerpilih
            KueTerpilih = kue;
        });

        AmbilBahanCommand = new Command(async () => await AmbilBahan());
        SimpanProduksiCommand = new Command(async () => await SimpanProduksi());
    }
    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}