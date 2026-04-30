using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CakeProject.Models.Production;

namespace CakeProject.ViewModels;

public class BahanItem : INotifyPropertyChanged
{
    private string _namaBahan = "";
    public string NamaBahan
    {
        get => _namaBahan;
        set { _namaBahan = value; OnPropertyChanged(); }
    }

    private double _jumlah;
    public double Jumlah
    {
        get => _jumlah;
        set { _jumlah = value; OnPropertyChanged(); }
    }

    public string Satuan { get; set; } = "";

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class ResepViewModel : INotifyPropertyChanged
{
    // === COLLECTIONS ===
    public ObservableCollection<ListKue> DaftarKue { get; set; } = new();
    public ObservableCollection<BahanItem> DaftarBahan { get; set; } = new();

    // === SELECTED KUE ===
    private ListKue? _kueTerpilih;
    public ListKue? KueTerpilih
    {
        get => _kueTerpilih;
        set
        {
            _kueTerpilih = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(AdaKueTerpilih));
            MuatBahanKue();
        }
    }

    public bool AdaKueTerpilih => KueTerpilih != null;

    // === LAMA MASAK ===
    private string _lamaMasakText = "";
    public string LamaMasakText
    {
        get => _lamaMasakText;
        set { _lamaMasakText = value; OnPropertyChanged(); }
    }

    // === COMMANDS ===
    public ICommand PilihKueCommand { get; }
    public ICommand SimpanResepCommand { get; }
    public ICommand HapusBahanCommand { get; }
    public ICommand TambahBahanCommand { get; }
    public ICommand TambahKueBaruCommand { get; }
    public ICommand HapusKueCommand { get; }

    // === CONSTRUCTOR ===
    public ResepViewModel()
    {
        PilihKueCommand = new Command<ListKue>(kue => KueTerpilih = kue);
        SimpanResepCommand = new Command(async () => await SimpanResep());
        HapusBahanCommand = new Command<BahanItem>(bahan => DaftarBahan.Remove(bahan));
        TambahBahanCommand = new Command(async () => await TambahBahan());
        TambahKueBaruCommand = new Command(async () => await TambahKueBaru());
        HapusKueCommand = new Command<ListKue>(async (kue) => await HapusKue(kue));

        LoadKue();
    }

    // === METHODS ===
    private void LoadKue()
    {
        DaftarKue.Clear();
        foreach (var k in App.TokoData.DaftarMenu)
            DaftarKue.Add(k);
    }

    private void MuatBahanKue()
    {
        DaftarBahan.Clear();
        if (KueTerpilih?.DetailResep == null) return;

        LamaMasakText = KueTerpilih.DetailResep.LamaMasak.ToString();

        foreach (var bahan in KueTerpilih.DetailResep.DaftarBahanBaku)
        {
            var stok = App.TokoData.DaftarBahan
                .FirstOrDefault(b => b.NamaBarang == bahan.Key);

            DaftarBahan.Add(new BahanItem
            {
                NamaBahan = bahan.Key,
                Jumlah = bahan.Value,
                Satuan = stok?.Satuan ?? ""
            });
        }
    }

    private async Task TambahBahan()
    {
        var pilihanBahan = App.TokoData.DaftarBahan
            .Select(b => b.NamaBarang).ToArray();

        string? namaBahan = await Application.Current.MainPage.DisplayActionSheet(
            "Pilih Bahan", "Batal", null, pilihanBahan);

        if (namaBahan == null || namaBahan == "Batal") return;

        if (DaftarBahan.Any(b => b.NamaBahan == namaBahan))
        {
            await Application.Current.MainPage.DisplayAlert(
                "Peringatan", $"{namaBahan} sudah ada di resep!", "OK");
            return;
        }

        string? jumlahText = await Application.Current.MainPage.DisplayPromptAsync(
            "Jumlah", $"Masukkan jumlah {namaBahan}:",
            keyboard: Keyboard.Numeric);

        if (jumlahText == null) return;

        if (!double.TryParse(jumlahText, out double jumlah) || jumlah <= 0)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Jumlah tidak valid!", "OK");
            return;
        }

        var stok = App.TokoData.DaftarBahan.FirstOrDefault(b => b.NamaBarang == namaBahan);
        DaftarBahan.Add(new BahanItem
        {
            NamaBahan = namaBahan,
            Jumlah = jumlah,
            Satuan = stok?.Satuan ?? ""
        });
    }

    private async Task SimpanResep()
    {
        if (KueTerpilih == null) return;

        if (!double.TryParse(LamaMasakText, out double lamaMasak) || lamaMasak <= 0)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Lama masak tidak valid!", "OK");
            return;
        }

        if (DaftarBahan.Count == 0)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Resep harus punya minimal 1 bahan!", "OK");
            return;
        }

        KueTerpilih.DetailResep.LamaMasak = lamaMasak;
        KueTerpilih.DetailResep.DaftarBahanBaku = DaftarBahan
            .ToDictionary(b => b.NamaBahan, b => b.Jumlah);

        await App.TokoData.SimpanMenuAsync();

        await Application.Current.MainPage.DisplayAlert(
            "Sukses", $"Resep {KueTerpilih.NamaKue} berhasil disimpan!", "OK");
    }

    private async Task TambahKueBaru()
    {
        string? namaKue = await Application.Current.MainPage.DisplayPromptAsync(
            "Tambah Kue Baru", "Nama kue:", placeholder: "contoh: Kue Cokelat");
        if (string.IsNullOrWhiteSpace(namaKue)) return;

        if (App.TokoData.DaftarMenu.Any(k => k.NamaKue == namaKue))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Kue dengan nama ini sudah ada!", "OK");
            return;
        }

        string? hargaBeliText = await Application.Current.MainPage.DisplayPromptAsync(
            "Tambah Kue Baru", "Harga beli (Rp):", keyboard: Keyboard.Numeric);
        if (hargaBeliText == null) return;
        if (!int.TryParse(hargaBeliText, out int hargaBeli) || hargaBeli <= 0)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Harga beli tidak valid!", "OK");
            return;
        }

        string? hargaJualText = await Application.Current.MainPage.DisplayPromptAsync(
            "Tambah Kue Baru", "Harga jual (Rp):", keyboard: Keyboard.Numeric);
        if (hargaJualText == null) return;
        if (!int.TryParse(hargaJualText, out int hargaJual) || hargaJual <= 0)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Harga jual tidak valid!", "OK");
            return;
        }

        string? lamaMasakText = await Application.Current.MainPage.DisplayPromptAsync(
            "Tambah Kue Baru", "Lama masak (menit):", keyboard: Keyboard.Numeric);
        if (lamaMasakText == null) return;
        if (!double.TryParse(lamaMasakText, out double lamaMasak) || lamaMasak <= 0)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Lama masak tidak valid!", "OK");
            return;
        }

        int idBaru = App.TokoData.DaftarProduk.Count > 0
            ? App.TokoData.DaftarProduk.Max(p => p.IDBarang) + 1
            : 200;

        var produkBaru = new Models.Inventory.StokProduk
        {
            IDBarang = idBaru,
            NamaBarang = namaKue,
            HargaBeli = hargaBeli,
            HargaJual = hargaJual,
            JumlahStok = 0,
            Satuan = "Pcs",
            TglExpired = DateTime.Today.AddMonths(6)
        };

        var kueBaru = new ListKue
        {
            NamaKue = namaKue,
            Deskripsi = "",
            DetailResep = new ResepKue(new Dictionary<string, double>(), lamaMasak)
        };

        App.TokoData.DaftarProduk.Add(produkBaru);
        App.TokoData.DaftarMenu.Add(kueBaru);

        await App.TokoData.SimpanProdukAsync();
        await App.TokoData.SimpanMenuAsync();

        DaftarKue.Add(kueBaru);

        await Application.Current.MainPage.DisplayAlert(
            "Sukses",
            $"{namaKue} berhasil ditambahkan!\nResep masih kosong — pilih kue ini untuk tambah bahan.",
            "OK");

        KueTerpilih = kueBaru;
    }
    private async Task HapusKue(ListKue kue)
    {
        bool konfirmasi = await Application.Current.MainPage.DisplayAlert(
            "Hapus Kue",
            $"Apakah kamu yakin ingin menghapus {kue.NamaKue}?\nData resep dan produk akan ikut terhapus.",
            "Hapus", "Batal");

        if (!konfirmasi) return;

        // Hapus dari DaftarMenu
        App.TokoData.DaftarMenu.Remove(kue);

        // Hapus dari DaftarProduk
        var produk = App.TokoData.DaftarProduk
            .FirstOrDefault(p => p.NamaBarang == kue.NamaKue);
        if (produk != null)
            App.TokoData.DaftarProduk.Remove(produk);

        // Simpan ke JSON
        await App.TokoData.SimpanMenuAsync();
        await App.TokoData.SimpanProdukAsync();

        // Update UI
        DaftarKue.Remove(kue);

        // Reset kalau kue yang dihapus adalah yang sedang dipilih
        if (KueTerpilih == kue)
            KueTerpilih = null;

        await Application.Current.MainPage.DisplayAlert(
            "Sukses", $"{kue.NamaKue} berhasil dihapus!", "OK");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}