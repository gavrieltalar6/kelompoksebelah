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
        if (KueTerpilih == null)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Pilih jenis kue terlebih dahulu!", "OK");
            return;
        }
        if (JumlahProduksi <= 0)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Jumlah produksi harus lebih dari 0!", "OK");
            return;
        }

        // Cari resep dari DaftarMenu
        var menu = App.TokoData.DaftarMenu
            .FirstOrDefault(m => m.NamaKue == KueTerpilih.NamaBarang);

        if (menu?.DetailResep == null)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Resep tidak ditemukan!", "OK");
            return;
        }

        // Cek apakah semua bahan cukup
        var kekurangan = new List<string>();
        foreach (var bahan in menu.DetailResep.DaftarBahanBaku)
        {
            double jumlahDibutuhkan = bahan.Value * JumlahProduksi;
            var stok = App.TokoData.DaftarBahan
                .FirstOrDefault(b => b.NamaBarang == bahan.Key);

            if (stok == null || stok.JumlahStok < jumlahDibutuhkan)
                kekurangan.Add($"{bahan.Key}: butuh {jumlahDibutuhkan}, ada {stok?.JumlahStok ?? 0}");
        }

        if (kekurangan.Count > 0)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Stok Tidak Cukup",
                "Bahan berikut kurang:\n" + string.Join("\n", kekurangan),
                "OK");
            return;
        }

        // Kurangi semua bahan sesuai resep x jumlah produksi
        foreach (var bahan in menu.DetailResep.DaftarBahanBaku)
        {
            double jumlahDibutuhkan = bahan.Value * JumlahProduksi;
            var stok = App.TokoData.DaftarBahan
                .FirstOrDefault(b => b.NamaBarang == bahan.Key);
            stok?.KurangiStok(jumlahDibutuhkan);
        }

        // Tambah stok produk
        KueTerpilih.TambahStok(JumlahProduksi);

        // Simpan ke JSON
        await App.TokoData.SimpanBahanAsync();
        await App.TokoData.SimpanProdukAsync();

        // Reset UI
        foreach (var k in ListKueTersedia) k.IsAktif = false;
        KueTerpilih = null;
        JumlahProduksi = 0;

        await Application.Current.MainPage.DisplayAlert("Sukses", "Produksi dicatat dan stok bahan dikurangi!", "OK");
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
    public void RefreshData()
    {
        ListBahanGudang.Clear();
        foreach (var b in App.TokoData.DaftarBahan)
            ListBahanGudang.Add(b);

        ListKueTersedia.Clear();
        foreach (var p in App.TokoData.DaftarProduk)
            ListKueTersedia.Add(p);
    }
    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}