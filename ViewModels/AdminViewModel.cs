using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CakeProject.Data;
using CakeProject.Models.Inventory;

namespace CakeProject.ViewModels;

public class AlertItem
{
    public string Icon { get; set; } = "";
    public string Pesan { get; set; } = "";
}

public class AdminViewModel : INotifyPropertyChanged
{
    // === METRICS ===
    private int _jumlahStokKritis;
    public int JumlahStokKritis
    {
        get => _jumlahStokKritis;
        set { _jumlahStokKritis = value; OnPropertyChanged(); }
    }

    private int _jumlahProduk;
    public int JumlahProduk
    {
        get => _jumlahProduk;
        set { _jumlahProduk = value; OnPropertyChanged(); }
    }

    private string _produkTerlaris = "-";
    public string ProdukTerlaris
    {
        get => _produkTerlaris;
        set { _produkTerlaris = value; OnPropertyChanged(); }
    }

    // === COLLECTIONS ===
    public ObservableCollection<AlertItem> DaftarAlert { get; set; } = new();
    public ObservableCollection<PenjualanData> LaporanTerkini { get; set; } = new();
    public ObservableCollection<StokProduk> DaftarProduk { get; set; } = new();

    // === EDIT HARGA ===
    public ICommand EditHargaCommand { get; }

    public AdminViewModel()
    {
        EditHargaCommand = new Command<StokProduk>(async (produk) => await EditHarga(produk));
    }

    public async Task RefreshData()
    {
        // === METRICS ===
        var stokKritis = App.TokoData.DaftarBahan
            .Where(b => b.CekStokRendah()).ToList();
        JumlahStokKritis = stokKritis.Count;
        JumlahProduk = App.TokoData.DaftarProduk.Count;

        var terlaris = App.TokoData.DaftarPenjualan
            .SelectMany(p => p.Items)
            .GroupBy(i => i.NamaProduk)
            .OrderByDescending(g => g.Sum(i => i.Jumlah))
            .FirstOrDefault();
        ProdukTerlaris = terlaris?.Key ?? "Belum ada data";

        // === ALERTS ===
        DaftarAlert.Clear();
        foreach (var b in stokKritis)
        {
            DaftarAlert.Add(new AlertItem
            {
                Icon = "⚠️",
                Pesan = $"{b.NamaBarang} : Kritis ({b.JumlahStok} {b.Satuan})"
            });
        }

        var transaksiTerbaru = App.TokoData.DaftarPenjualan
    .OrderByDescending(p => p.Tanggal)
    .FirstOrDefault();

        if (transaksiTerbaru != null)
        {
            // Gabungkan semua item jadi satu string
            var daftarItem = string.Join(", ", transaksiTerbaru.Items
                .Select(i => $"{i.NamaProduk} x{i.Jumlah}"));

            DaftarAlert.Add(new AlertItem
            {
                Icon = "🧾",
                Pesan = $"Transaksi #{transaksiTerbaru.IDTransaksi} : {daftarItem}"
            });
        }

        if (DaftarAlert.Count == 0)
            DaftarAlert.Add(new AlertItem { Icon = "✅", Pesan = "Semua stok aman, tidak ada alert." });

        // === LAPORAN ===
        // === LAPORAN ===
        LaporanTerkini.Clear();
        foreach (var p in App.TokoData.DaftarPenjualan
            .OrderByDescending(p => p.Tanggal)
            .Take(5))
            LaporanTerkini.Add(p);
        // === PRODUK ===
        DaftarProduk.Clear();
        foreach (var p in App.TokoData.DaftarProduk)
            DaftarProduk.Add(p);

        // === KEUNTUNGAN ===
        var hari = DateTime.Today;
        var awalMinggu = hari.AddDays(-(int)hari.DayOfWeek);

        double profitHariIni = 0;
        double profitMingguIni = 0;

        foreach (var transaksi in App.TokoData.DaftarPenjualan)
        {
            foreach (var item in transaksi.Items)
            {
                // Cari produk yang cocok untuk ambil HargaBeli
                var produk = App.TokoData.DaftarProduk
                    .FirstOrDefault(p => p.NamaBarang == item.NamaProduk);
                if (produk == null) continue;

                double profit = produk.HitungProfit() * item.Jumlah;

                if (transaksi.Tanggal.Date == hari)
                    profitHariIni += profit;

                if (transaksi.Tanggal.Date >= awalMinggu)
                    profitMingguIni += profit;
            }
        }

        KeuntunganHariIni = $"Rp {profitHariIni:N0}";
        KeuntunganMingguIni = $"Rp {profitMingguIni:N0}";
    }

    private async Task EditHarga(StokProduk produk)
    {
        string result = await Application.Current.MainPage.DisplayPromptAsync(
            "Edit Harga",
            $"Masukkan harga jual baru untuk {produk.NamaBarang}:",
            initialValue: produk.HargaJual.ToString(),
            keyboard: Keyboard.Numeric);

        if (result == null) return;

        if (int.TryParse(result, out int hargaBaru) && hargaBaru > 0)
        {
            produk.HargaJual = hargaBaru;
            await App.TokoData.SimpanProdukAsync();
            await Application.Current.MainPage.DisplayAlert("Sukses", $"Harga {produk.NamaBarang} diperbarui!", "OK");
            RefreshData();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Gagal", "Harga tidak valid!", "OK");
        }
    }
    private string _keuntunganHariIni = "Rp 0";
    public string KeuntunganHariIni
    {
        get => _keuntunganHariIni;
        set { _keuntunganHariIni = value; OnPropertyChanged(); }
    }

    private string _keuntunganMingguIni = "Rp 0";
    public string KeuntunganMingguIni
    {
        get => _keuntunganMingguIni;
        set { _keuntunganMingguIni = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}