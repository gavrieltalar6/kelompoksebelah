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

        var transaksiTerbaru = App.TokoData.DaftarPenjualan.FirstOrDefault();
        if (transaksiTerbaru != null)
        {
            var itemPertama = transaksiTerbaru.Items.FirstOrDefault();
            if (itemPertama != null)
            {
                DaftarAlert.Add(new AlertItem
                {
                    Icon = "🧾",
                    Pesan = $"Transaksi #{transaksiTerbaru.IDTransaksi} : {itemPertama.NamaProduk} x{itemPertama.Jumlah}"
                });
            }
        }

        if (DaftarAlert.Count == 0)
            DaftarAlert.Add(new AlertItem { Icon = "✅", Pesan = "Semua stok aman, tidak ada alert." });

        // === LAPORAN ===
        LaporanTerkini.Clear();
        foreach (var p in App.TokoData.DaftarPenjualan.Take(5))
            LaporanTerkini.Add(p);

        // === PRODUK ===
        DaftarProduk.Clear();
        foreach (var p in App.TokoData.DaftarProduk)
            DaftarProduk.Add(p);
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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}