using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CakeProject.Data;
using CakeProject.Models.Inventory;
using CakeProject.Models.Finance;

namespace CakeProject.ViewModels;

public class KeranjangItem : INotifyPropertyChanged
{
    public StokProduk Produk { get; set; }

    private int _jumlah = 1;
    public int Jumlah
    {
        get => _jumlah;
        set
        {
            _jumlah = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Subtotal));
        }
    }

    public double Subtotal => Produk.HargaJual * Jumlah;
    public string SubtotalText => $"Rp {Subtotal:N0}";

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class PosViewModel : INotifyPropertyChanged
{
    private readonly MenuStore _store;

    // === COLLECTIONS ===
    public ObservableCollection<StokProduk> DaftarProduk { get; set; } = new();
    public ObservableCollection<KeranjangItem> Keranjang { get; set; } = new();

    // === SEARCH ===
    private string _kataCari = "";
    public string KataCari
    {
        get => _kataCari;
        set
        {
            _kataCari = value;
            OnPropertyChanged();
            FilterProduk();
        }
    }

    // === TOTAL ===
    public double TotalHarga => Keranjang.Sum(k => k.Subtotal);
    public string TotalHargaText => $"Rp {TotalHarga:N0}";

    // === COMMANDS ===
    public ICommand TambahKeKeranjangCommand { get; }
    public ICommand TambahJumlahCommand { get; }
    public ICommand KurangiJumlahCommand { get; }
    public ICommand HapusKeranjangCommand { get; }
    public ICommand BayarCommand { get; }

    public PosViewModel()
    {
        _store = new MenuStore();
        _store.InisialisasiData();

        TambahKeKeranjangCommand = new Command<StokProduk>(TambahKeKeranjang);
        TambahJumlahCommand = new Command<KeranjangItem>(item =>
        {
            item.Jumlah++;
            RefreshTotal();
        });
        KurangiJumlahCommand = new Command<KeranjangItem>(item =>
        {
            if (item.Jumlah > 1) item.Jumlah--;
            else Keranjang.Remove(item);
            RefreshTotal();
        });
        HapusKeranjangCommand = new Command(() =>
        {
            Keranjang.Clear();
            RefreshTotal();
        });
        BayarCommand = new Command(Bayar, () => Keranjang.Count > 0);

        LoadProduk();
    }

    private void LoadProduk()
    {
        foreach (var p in _store.DaftarProduk)
            DaftarProduk.Add(p);
    }

    private void FilterProduk()
    {
        DaftarProduk.Clear();
        var hasil = _store.DaftarProduk
            .Where(p => p.NamaBarang.Contains(KataCari, StringComparison.OrdinalIgnoreCase));
        foreach (var p in hasil)
            DaftarProduk.Add(p);
    }

    private void TambahKeKeranjang(StokProduk produk)
    {
        var existing = Keranjang.FirstOrDefault(k => k.Produk.IDBarang == produk.IDBarang);
        if (existing != null)
            existing.Jumlah++;
        else
            Keranjang.Add(new KeranjangItem { Produk = produk });

        RefreshTotal();
    }

    private void RefreshTotal()
    {
        OnPropertyChanged(nameof(TotalHarga));
        OnPropertyChanged(nameof(TotalHargaText));
    }

    private void Bayar()
    {
        var transaksi = new Penjualan
        {
            IDTransaksi = new Random().Next(1000, 9999),
            Tanggal = DateTime.Now,
            TotalBayar = TotalHarga,
            Pajak = TotalHarga * 0.11,
            MetodeBayar = "Tunai"
        };

        // nanti bisa disimpan ke JSON di sini
        Keranjang.Clear();
        RefreshTotal();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}