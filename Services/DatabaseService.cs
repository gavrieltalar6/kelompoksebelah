using System.Collections.ObjectModel;
using CakeProject.Models.Inventory; // Namespace sesuai model kamu

namespace CakeProject.Services;

public class DatabaseService
{
    private static DatabaseService _instance;
    public static DatabaseService Instance => _instance ??= new DatabaseService();

    // List untuk Bahan Baku (Tepung, Telur, dll)
    public ObservableCollection<StokBaku> DaftarStokBaku { get; set; } = new();

    // List untuk Hasil Produksi Kue (Red Velvet, dll)
    public ObservableCollection<StokProduk> DaftarStokProduk { get; set; } = new();

    private DatabaseService() { }
}