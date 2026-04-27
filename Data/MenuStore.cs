using CakeProject.Models.Production;
using CakeProject.Models.Inventory;
using CakeProject.Services;

namespace CakeProject.Data;

public class MenuStore
{
    // Nama file JSON
    private const string FileBahan = "stok_baku.json";
    private const string FileProduk = "stok_produk.json";
    private const string FileMenu = "list_kue.json";
    private const string FileRiwayat = "riwayat_input.json";
    private const string FilePenjualan = "riwayat_penjualan.json";

    // Data
    public List<StokBaku> DaftarBahan { get; set; } = new();
    public List<StokProduk> DaftarProduk { get; set; } = new();
    public List<ListKue> DaftarMenu { get; set; } = new();
    public List<RiwayatInputData> DaftarRiwayat { get; set; } = new();
    public List<PenjualanData> DaftarPenjualan { get; set; } = new();

    public async Task MuatSemuaAsync()
    {
        DaftarBahan = await JsonService.MuatAsync<List<StokBaku>>(FileBahan)
                      ?? BuatDataAwalBahan();

        DaftarProduk = await JsonService.MuatAsync<List<StokProduk>>(FileProduk)
                       ?? BuatDataAwalProduk();

        DaftarMenu = await JsonService.MuatAsync<List<ListKue>>(FileMenu)
                     ?? BuatDataAwalMenu();

        DaftarRiwayat = await JsonService.MuatAsync<List<RiwayatInputData>>(FileRiwayat)
                        ?? new List<RiwayatInputData>();

        DaftarPenjualan = await JsonService.MuatAsync<List<PenjualanData>>(FilePenjualan)
                          ?? new List<PenjualanData>();
    }

    public async Task SimpanBahanAsync() =>
        await JsonService.SimpanAsync(FileBahan, DaftarBahan);

    public async Task SimpanProdukAsync() =>
        await JsonService.SimpanAsync(FileProduk, DaftarProduk);

    public async Task SimpanMenuAsync() =>
        await JsonService.SimpanAsync(FileMenu, DaftarMenu);

    public async Task SimpanRiwayatAsync() =>
        await JsonService.SimpanAsync(FileRiwayat, DaftarRiwayat);

    public async Task SimpanPenjualanAsync() =>
        await JsonService.SimpanAsync(FilePenjualan, DaftarPenjualan);

    // === DATA AWAL (fallback kalau JSON belum ada) ===
    private List<StokBaku> BuatDataAwalBahan() => new()
    {
        new StokBaku { IDBarang = 1, NamaBarang = "Tepung Terigu", JumlahStok = 25, MinimalStok = 5, Satuan = "Kg", TglExpired = new DateTime(2027, 3, 25) },
        new StokBaku { IDBarang = 2, NamaBarang = "Telur", JumlahStok = 10, MinimalStok = 5, Satuan = "Butir", TglExpired = new DateTime(2027, 3, 25) },
        new StokBaku { IDBarang = 3, NamaBarang = "Gula", JumlahStok = 5, MinimalStok = 3, Satuan = "Kg", TglExpired = new DateTime(2027, 3, 25) },
        new StokBaku { IDBarang = 4, NamaBarang = "Mentega", JumlahStok = 2, MinimalStok = 3, Satuan = "Kg", TglExpired = new DateTime(2027, 3, 25) },
        new StokBaku { IDBarang = 5, NamaBarang = "Susu", JumlahStok = 1, MinimalStok = 3, Satuan = "Liter", TglExpired = new DateTime(2027, 3, 25) },
    };

    private List<StokProduk> BuatDataAwalProduk() => new()
    {
        new StokProduk { IDBarang = 101, NamaBarang = "Kue Tiramisu", HargaBeli = 15000, HargaJual = 20000, JumlahStok = 10, Satuan = "Pcs", TglExpired = new DateTime(2027, 3, 25) },
        new StokProduk { IDBarang = 102, NamaBarang = "Kue Balok Cokelat", HargaBeli = 35000, HargaJual = 50000, JumlahStok = 8, Satuan = "Pcs", TglExpired = new DateTime(2027, 3, 25) },
        new StokProduk { IDBarang = 103, NamaBarang = "Kue Ultah Vanila", HargaBeli = 75000, HargaJual = 100000, JumlahStok = 5, Satuan = "Pcs", TglExpired = new DateTime(2027, 3, 25) },
        new StokProduk { IDBarang = 104, NamaBarang = "Fudgy Brownies", HargaBeli = 100000, HargaJual = 150000, JumlahStok = 6, Satuan = "Pcs", TglExpired = new DateTime(2027, 3, 25) },
        new StokProduk { IDBarang = 105, NamaBarang = "Kue Roblox", HargaBeli = 120000, HargaJual = 170000, JumlahStok = 3, Satuan = "Pcs", TglExpired = new DateTime(2027, 3, 25) },
    };

    private List<ListKue> BuatDataAwalMenu() => new()
    {
        new ListKue { NamaKue = "Bolu Macan", Deskripsi = "Kue bolu klasik" },
        new ListKue { NamaKue = "Fudgy Brownies", Deskripsi = "Brownies cokelat lembut" },
    };
}

// === HELPER CLASSES untuk JSON ===
public class RiwayatInputData
{
    public string NamaBahan { get; set; } = "";
    public string Satuan { get; set; } = "";
    public DateTime TanggalExpired { get; set; }
    public string WaktuInput { get; set; } = "";
    public string InfoLabel => $"{Satuan}, {TanggalExpired:d MMMM yyyy}";
}

public class PenjualanData
{
    public int IDTransaksi { get; set; }
    public DateTime Tanggal { get; set; }
    public double TotalBayar { get; set; }
    public double Pajak { get; set; }
    public string MetodeBayar { get; set; } = "";
    public List<ItemPenjualan> Items { get; set; } = new();
}

public class ItemPenjualan
{
    public string NamaProduk { get; set; } = "";
    public int Jumlah { get; set; }
    public double HargaSatuan { get; set; }
    public double Subtotal { get; set; }
}