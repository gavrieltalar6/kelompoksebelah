using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CakeProject.Data;
using CakeProject.Models.Inventory;

namespace CakeProject.ViewModels;

public class GudangViewModel : INotifyPropertyChanged
{
    private readonly MenuStore _store;

    // === COLLECTIONS ===
    public ObservableCollection<StokBaku> DaftarBahan { get; set; } = new();
    public ObservableCollection<RiwayatInput> RiwayatInputList { get; set; } = new();
    public List<string> DaftarKategori { get; set; } = new() { "Tepung", "Telur & Susu", "Gula & Lemak", "Lainnya" };
    public List<string> DaftarSatuan { get; set; } = new() { "Kg", "Butir", "Liter" };

    // === FORM PROPERTIES ===
    private StokBaku _bahanDipilih;
    public StokBaku BahanDipilih
    {
        get => _bahanDipilih;
        set { _bahanDipilih = value; OnPropertyChanged(); }
    }

    private string _kategoriDipilih;
    public string KategoriDipilih
    {
        get => _kategoriDipilih;
        set { _kategoriDipilih = value; OnPropertyChanged(); }
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

    private DateTime _tanggalMasuk = DateTime.Today;
    public DateTime TanggalMasuk
    {
        get => _tanggalMasuk;
        set { _tanggalMasuk = value; OnPropertyChanged(); }
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

    public GudangViewModel()
    {
        _store = new MenuStore();
        _store.InisialisasiData();

        SimpanStokCommand = new Command(SimpanStok);
        PilihSatuanCommand = new Command<string>(s => SatuanDipilih = s);

        LoadData();
    }

    private void LoadData()
    {
        foreach (var bahan in _store.DaftarBahan)
            DaftarBahan.Add(bahan);
    }

    private void SimpanStok()
    {
        if (BahanDipilih == null || JumlahMasuk <= 0) return;

        // Update stok di dummy data
        BahanDipilih.TambahStok(JumlahMasuk);
        BahanDipilih.TglExpired = TanggalExpired;

        // Tambah ke riwayat
        RiwayatInputList.Insert(0, new RiwayatInput
        {
            NamaBahan = BahanDipilih.NamaBarang,
            Satuan = SatuanDipilih,
            TanggalExpired = TanggalExpired,
            WaktuInput = DateTime.Now.ToString("h:mm tt")
        });

        // Reset form
        JumlahMasuk = 0;
        BahanDipilih = null;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

// Helper class untuk riwayat
public class RiwayatInput
{
    public string NamaBahan { get; set; }
    public string Satuan { get; set; }
    public DateTime TanggalExpired { get; set; }
    public string WaktuInput { get; set; }
    public string InfoLabel => $"{Satuan}, {TanggalExpired:d MMMM yyyy}";
}