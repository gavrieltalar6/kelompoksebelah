using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CakeProject.Data;
using CakeProject.Models.Inventory;
using CakeProject.Models.Finance;
using CakeProject.Models.Users;

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
            OnPropertyChanged(nameof(SubtotalText));
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
    private readonly CustomerStore _customerStore;

    // === COLLECTIONS ===
    public ObservableCollection<StokProduk> DaftarProduk { get; set; } = new();
    public ObservableCollection<KeranjangItem> Keranjang { get; set; } = new();

    // === SEARCH PRODUK ===
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

    // === CUSTOMER ===
    private string _noHpCustomer = "";
    public string NoHpCustomer
    {
        get => _noHpCustomer;
        set
        {
            _noHpCustomer = value;
            OnPropertyChanged();
        }
    }

    private Customer _customerSekarang;
    public Customer CustomerSekarang
    {
        get => _customerSekarang;
        set
        {
            _customerSekarang = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(InfoCustomer));
            OnPropertyChanged(nameof(InfoPoinCustomer));
        }
    }

    public string InfoCustomer
    {
        get
        {
            if (CustomerSekarang == null)
                return "Belum ada customer";
            return $"{CustomerSekarang.Nama} - Poin: {CustomerSekarang.TotalPoin}";
        }
    }

    // === PROPERTI INFO POIN (dibutuhkan XAML) ===

    /// <summary>
    /// Teks info poin customer yang tampil di sebelah label "Tukar Poin"
    /// Contoh: "Poin kamu: 150" atau kosong kalau tidak ada customer
    /// </summary>
    public string InfoPoinCustomer
    {
        get
        {
            if (CustomerSekarang == null)
                return "";
            return $"Poin kamu: {CustomerSekarang.TotalPoin}";
        }
    }

    /// <summary>
    /// Teks info diskon dari poin yang dipakai
    /// Contoh: "Diskon poin: Rp 10.000"
    /// </summary>
    public string InfoDiskonPoin
    {
        get
        {
            if (DiskonSekarang <= 0) return "";
            return $"Diskon poin: Rp {DiskonSekarang:N0}";
        }
    }

    /// <summary>
    /// Menentukan apakah section diskon & total setelah diskon ditampilkan
    /// </summary>
    public bool AdaDiskonPoin => DiskonSekarang > 0;

    /// <summary>
    /// Teks total harga setelah dikurangi diskon poin
    /// </summary>
    public string TotalSetelahDiskonText => $"Rp {TotalAkhir:N0}";

    // === POIN ===
    private string _poinDitukarText = "0";

    /// <summary>
    /// Text binding untuk Entry poin di XAML.
    /// Validasi dilakukan di sini: cek angka negatif, huruf, dan melebihi poin dimiliki.
    /// </summary>
    public string PoinDitukarText
    {
        get => _poinDitukarText;
        set
        {
            _poinDitukarText = value;
            OnPropertyChanged();
            ValidasiDanSetPoin(value);
        }
    }

    private int _poinDitukar = 0;
    public int PoinDitukar
    {
        get => _poinDitukar;
        private set
        {
            _poinDitukar = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DiskonSekarang));
            OnPropertyChanged(nameof(TotalAkhir));
            OnPropertyChanged(nameof(TotalAkhirText));
            OnPropertyChanged(nameof(TotalSetelahDiskonText));
            OnPropertyChanged(nameof(InfoDiskonPoin));
            OnPropertyChanged(nameof(AdaDiskonPoin));
        }
    }

    private async void ValidasiDanSetPoin(string input)
    {
        // Kosong = anggap 0, tidak error
        if (string.IsNullOrWhiteSpace(input))
        {
            PoinDitukar = 0;
            return;
        }

        // Cek apakah input bukan angka atau mengandung huruf/karakter aneh
        if (!int.TryParse(input, out int nilai))
        {
            PoinDitukar = 0;
            await Application.Current.MainPage.DisplayAlert(
                "Input Tidak Valid",
                "Input point invalid. Masukkan angka yang benar.",
                "OK");
            _poinDitukarText = "0";
            OnPropertyChanged(nameof(PoinDitukarText));
            return;
        }

        // Cek angka negatif
        if (nilai < 0)
        {
            PoinDitukar = 0;
            await Application.Current.MainPage.DisplayAlert(
                "Input Tidak Valid",
                "Input point invalid. Poin tidak boleh negatif.",
                "OK");
            _poinDitukarText = "0";
            OnPropertyChanged(nameof(PoinDitukarText));
            return;
        }

        // Cek apakah customer sudah dipilih sebelum tukar poin
        if (nilai > 0 && CustomerSekarang == null)
        {
            PoinDitukar = 0;
            await Application.Current.MainPage.DisplayAlert(
                "Belum Ada Customer",
                "Masukkan nomor HP customer terlebih dahulu.",
                "OK");
            _poinDitukarText = "0";
            OnPropertyChanged(nameof(PoinDitukarText));
            return;
        }

        // Cek apakah melebihi poin yang dimiliki
        if (CustomerSekarang != null && nilai > CustomerSekarang.TotalPoin)
        {
            PoinDitukar = 0;
            await Application.Current.MainPage.DisplayAlert(
                "Poin Tidak Cukup",
                $"Point tidak cukup. Poin kamu saat ini: {CustomerSekarang.TotalPoin}",
                "OK");
            _poinDitukarText = "0";
            OnPropertyChanged(nameof(PoinDitukarText));
            return;
        }

        // Semua validasi lolos
        PoinDitukar = nilai;
    }

    // === KALKULASI DISKON & TOTAL ===
    // Rumus: setiap 100 poin = Rp 10.000 diskon (bisa disesuaikan)
    public int DiskonSekarang => (PoinDitukar / 100) * 10000;
    public double TotalAkhir => Math.Max(0, TotalHarga - DiskonSekarang);
    public string TotalAkhirText => $"Rp {TotalAkhir:N0}";

    // === TOTAL ===
    public double TotalHarga => Keranjang.Sum(k => k.Subtotal);
    public string TotalHargaText => $"Rp {TotalHarga:N0}";

    // === COMMANDS ===
    public ICommand TambahKeKeranjangCommand { get; }
    public ICommand TambahJumlahCommand { get; }
    public ICommand KurangiJumlahCommand { get; }
    public ICommand HapusKeranjangCommand { get; }
    public ICommand CariCustomerCommand { get; }
    public ICommand BayarCommand { get; }

    public PosViewModel()
    {
        _store = App.TokoData;
        _customerStore = new CustomerStore();

        // Load data customer
        _ = _customerStore.MuatAsync();

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

        CariCustomerCommand = new Command(async () => await CariCustomer());
        BayarCommand = new Command(async () => await Bayar(), () => Keranjang.Count > 0);

        LoadProduk();
    }

    private void LoadProduk()
    {
        DaftarProduk.Clear();
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

    private async void TambahKeKeranjang(StokProduk produk)
{
    if (produk.JumlahStok <= 0)
    {
        await Application.Current.MainPage.DisplayAlert(
            "Stok Habis", $"{produk.NamaBarang} sedang tidak tersedia.", "OK");
        return;
    }

    var existing = Keranjang.FirstOrDefault(k => k.Produk.IDBarang == produk.IDBarang);
    if (existing != null)
    {
        // Cek kalau jumlah di keranjang sudah melebihi stok
        if (existing.Jumlah >= produk.JumlahStok)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Stok Tidak Cukup", $"Stok {produk.NamaBarang} hanya {produk.JumlahStok} {produk.Satuan}.", "OK");
            return;
        }
        existing.Jumlah++;
    }
    else
        Keranjang.Add(new KeranjangItem { Produk = produk });

    RefreshTotal();
}

    private void RefreshTotal()
    {
        OnPropertyChanged(nameof(TotalHarga));
        OnPropertyChanged(nameof(TotalHargaText));
        OnPropertyChanged(nameof(TotalAkhir));
        OnPropertyChanged(nameof(TotalAkhirText));
        OnPropertyChanged(nameof(TotalSetelahDiskonText));
        OnPropertyChanged(nameof(AdaDiskonPoin));
        OnPropertyChanged(nameof(InfoDiskonPoin));
        ((Command)BayarCommand).ChangeCanExecute();
    }

    // ===== CARI CUSTOMER BY NO HP =====
    private async Task CariCustomer()
    {
        if (string.IsNullOrWhiteSpace(NoHpCustomer))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Masukkan nomor HP terlebih dahulu.", "OK");
            return;
        }

        CustomerSekarang = _customerStore.CariCustomerByNoHP(NoHpCustomer);

        if (CustomerSekarang == null)
        {
            await Application.Current.MainPage.DisplayAlert("Tidak Ditemukan", "Customer dengan nomor HP tersebut tidak ditemukan.", "OK");

            // Reset poin jika customer tidak ditemukan
            PoinDitukar = 0;
            _poinDitukarText = "0";
            OnPropertyChanged(nameof(PoinDitukarText));
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert(
                "Customer Ditemukan ✓",
                $"Halo, {CustomerSekarang.Nama}!\nPoin kamu: {CustomerSekarang.TotalPoin} poin",
                "OK");
        }
    }

    // ===== BAYAR =====
    private async Task Bayar()
    {
        if (Keranjang.Count == 0)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Keranjang kosong.", "OK");
            return;
        }

        double totalBayar = TotalHarga;
        int diskonDiterima = 0;

        // Proses tukar poin jika ada
        if (CustomerSekarang != null && PoinDitukar > 0)
        {
            var result = CustomerSekarang.TukarPoin(PoinDitukar);

            if (!result.success)
            {
                await Application.Current.MainPage.DisplayAlert("Error", result.message, "OK");
                return;
            }

            diskonDiterima = result.diskon;
            totalBayar = TotalAkhir;

            // Simpan pengurangan poin ke database
            await _customerStore.UpdatePoinCustomer(CustomerSekarang.Id, CustomerSekarang.TotalPoin);
        }

        // ===== TAMBAH POIN DARI TRANSAKSI =====
        // Rumus: setiap Rp 1.000 dapat 1 poin
        if (CustomerSekarang != null)
        {
            int poinBaru = (int)(TotalHarga / 1000);

            if (poinBaru > 0)
            {
                CustomerSekarang.TotalPoin += poinBaru;
                await _customerStore.UpdatePoinCustomer(CustomerSekarang.Id, CustomerSekarang.TotalPoin);
            }
        }

        // Buat data transaksi
        var transaksi = new PenjualanData
        {
            IDTransaksi = new Random().Next(1000, 9999),
            Tanggal = DateTime.Now,
            TotalBayar = totalBayar,
            Pajak = TotalHarga * 0.11,
            MetodeBayar = "Tunai",
            Items = Keranjang.Select(k => new ItemPenjualan
            {
                NamaProduk = k.Produk.NamaBarang,
                Jumlah = k.Jumlah,
                HargaSatuan = k.Produk.HargaJual,
                Subtotal = k.Subtotal
            }).ToList()
        };

        // Simpan transaksi
        _store.DaftarPenjualan.Add(transaksi);
        await _store.SimpanPenjualanAsync();

        // Update stok
        foreach (var item in Keranjang)
        {
            item.Produk.KurangiStok(item.Jumlah);
        }
        await _store.SimpanProdukAsync();

        // Pesan sukses
        int poinDidapat = CustomerSekarang != null ? (int)(TotalHarga / 1000) : 0;
        string pesan = $"Transaksi Berhasil!\n\nTotal: Rp {TotalHarga:N0}";
        if (diskonDiterima > 0)
            pesan += $"\nDiskon Poin: Rp {diskonDiterima:N0}\nTotal Bayar: Rp {totalBayar:N0}";
        if (poinDidapat > 0)
            pesan += $"\n\n+{poinDidapat} poin ditambahkan ke akun {CustomerSekarang.Nama}!";

        await Application.Current.MainPage.DisplayAlert("Sukses", pesan, "OK");

        // Reset UI
        Keranjang.Clear();
        NoHpCustomer = "";
        CustomerSekarang = null;
        PoinDitukar = 0;
        _poinDitukarText = "0";
        OnPropertyChanged(nameof(PoinDitukarText));
        RefreshTotal();

    }
    public void RefreshProduk()
    {
        DaftarProduk.Clear();
        foreach (var p in _store.DaftarProduk)
            DaftarProduk.Add(p);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
