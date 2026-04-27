using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CakeProject.Data;
using CakeProject.Models.Inventory;

namespace CakeProject.ViewModels;

public class GudangViewModel : INotifyPropertyChanged
{
    // Mengacu ke satu sumber data yang sama di seluruh aplikasi
    private readonly MenuStore _store;

    // === COLLECTIONS (Data untuk ditampilkan di layar) ===
    public ObservableCollection<StokBaku> DaftarBahan { get; set; } = new();
    public ObservableCollection<RiwayatInputData> RiwayatInputList { get; set; } = new();
    
    public List<string> DaftarKategori { get; set; } = new() { "Tepung", "Telur & Susu", "Gula & Lemak", "Lainnya" };
    public List<string> DaftarSatuan { get; set; } = new() { "Kg", "Butir", "Liter" };

    // === FORM PROPERTIES (Tempat menampung input dari UI) ===
    private StokBaku _bahanDipilih;
    public StokBaku BahanDipilih
    {
        get => _bahanDipilih;
        set { _bahanDipilih = value; OnPropertyChanged(); }
    }

    private double _jumlahMasuk;
    public double JumlahMasuk
    {
        get => _jumlahMasuk;
        set { _jumlahMasuk = value; OnPropertyChanged(); }
    }

    private string _satuanDipilih = "Kg";
    public string SatuanDipilih
    {
        get => _satuanDipilih;
        set { _satuanDipilih = value; OnPropertyChanged(); }
    }

    private DateTime _tanggalExpired = DateTime.Today.AddMonths(6);
    public DateTime TanggalExpired
    {
        get => _tanggalExpired;
        set { _tanggalExpired = value; OnPropertyChanged(); }
    }

    // === COMMANDS ===
    public ICommand SimpanStokCommand { get; }
    public ICommand PilihSatuanCommand { get; }

    // === CONSTRUCTOR ===
    public GudangViewModel()
    {
        // Hubungkan ke Pusat Data di App.xaml.cs
        _store = App.TokoData;

        // Inisialisasi tombol-tombol
        SimpanStokCommand = new Command(async () => await SimpanStok());
        PilihSatuanCommand = new Command<string>(s => SatuanDipilih = s);

        // Ambil data yang sudah ada di memori Pusat Data
        LoadDataAwal();
    }

    private void LoadDataAwal()
    {
        DaftarBahan.Clear();
        foreach (var b in _store.DaftarBahan) 
            DaftarBahan.Add(b);

        RiwayatInputList.Clear();
        foreach (var r in _store.DaftarRiwayat) 
            RiwayatInputList.Add(r);
    }

    private async Task SimpanStok()
    {
        // Validasi: Cek apakah user sudah pilih bahan dan isi jumlahnya
        if (BahanDipilih == null || JumlahMasuk <= 0)
        {
            await Application.Current.MainPage.DisplayAlert("Peringatan", "Pilih bahan dan masukkan jumlah yang benar!", "OK");
            return;
        }

        // 1. Update jumlah stok di objek bahan baku
        BahanDipilih.TambahStok(JumlahMasuk);
        BahanDipilih.TglExpired = TanggalExpired;

        // 2. Buat data riwayat baru
        var riwayat = new RiwayatInputData
        {
            NamaBahan = BahanDipilih.NamaBarang,
            Satuan = SatuanDipilih,
            TanggalExpired = TanggalExpired,
            WaktuInput = DateTime.Now.ToString("h:mm tt")
        };

        // 3. Masukkan riwayat ke Pusat Data (supaya bisa disimpan ke JSON)
        _store.DaftarRiwayat.Insert(0, riwayat);

        // 4. Masukkan ke list UI agar langsung muncul di layar
        RiwayatInputList.Insert(0, riwayat);

        // 5. PERMANENKAN: Simpan ke file JSON
        await _store.SimpanBahanAsync();
        await _store.SimpanRiwayatAsync();

        // 6. Reset form input agar kosong kembali
        JumlahMasuk = 0;
        BahanDipilih = null;

        await Application.Current.MainPage.DisplayAlert("Sukses", "Stok berhasil ditambahkan!", "OK");
    }

    // === BOILERPLATE MVVM ===
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}