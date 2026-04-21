using CakeProject.Models.Production;
using CakeProject.Models.Inventory;
using System.Collections.Generic;

namespace CakeProject.Data;
public class MenuStore
{
    public List<ListKue> DaftarMenu { get; set; } = new List<ListKue>();
    public List<StokBaku> DaftarBahan { get; set; } = new List<StokBaku>();

    public void InisialisasiData()
    {
        // --- BAHAN BAKU ---
        DaftarBahan.Add(new StokBaku { IDBarang = 1, NamaBarang = "Tepung Terigu", JumlahStok = 25, MinimalStok = 5, Satuan = "Kg", TglExpired = new DateTime(2027, 3, 25) });
        DaftarBahan.Add(new StokBaku { IDBarang = 2, NamaBarang = "Telur", JumlahStok = 10, MinimalStok = 5, Satuan = "Butir", TglExpired = new DateTime(2027, 3, 25) });
        DaftarBahan.Add(new StokBaku { IDBarang = 3, NamaBarang = "Gula", JumlahStok = 5, MinimalStok = 3, Satuan = "Kg", TglExpired = new DateTime(2027, 3, 25) });
        DaftarBahan.Add(new StokBaku { IDBarang = 4, NamaBarang = "Mentega", JumlahStok = 2, MinimalStok = 3, Satuan = "Kg", TglExpired = new DateTime(2027, 3, 25) });
        DaftarBahan.Add(new StokBaku { IDBarang = 5, NamaBarang = "Susu", JumlahStok = 1, MinimalStok = 3, Satuan = "Liter", TglExpired = new DateTime(2027, 3, 25) });

        // --- KUE ---
        var bahanBolu = new Dictionary<string, double> { { "Tepung", 500 }, { "Telur", 4 } };
        ResepKue resepBolu = new ResepKue(bahanBolu, 1.5);
        DaftarMenu.Add(new ListKue { NamaKue = "Bolu Macan", DetailResep = resepBolu });

        var bahanBrownies = new Dictionary<string, double> { { "Cokelat", 200 }, { "Telur", 2 } };
        ResepKue resepBrownies = new ResepKue(bahanBrownies, 0.75);
        DaftarMenu.Add(new ListKue { NamaKue = "Fudgy Brownies", DetailResep = resepBrownies });
    }
}