namespace CakeProject.Models.Production;
using CakeProject.Models.Inventory;
public class ListKue
{
    public string NamaKue {get; set;}
    public string Deskripsi {get; set;}
    public StokProduk DataStok {get; set;}
    internal ResepKue DetailResep { get; set; }
    public string GetInfoLengkap()
    {
        string id = DataStok != null ? DataStok.IDBarang.ToString() : "??";
        string status = (DataStok != null && DataStok.CekKadaluarsa()) ? "KADALUARSA" : "AMAN";

        string infoWaste;
        if (DetailResep != null)
        {
            double hasilWaste = DetailResep.KalkulasiWaste();
            infoWaste = $"{hasilWaste} gram";
        }
        else
        {
            infoWaste = "Data resep tidak ada";
        }

        return 
            $"[{id}] {NamaKue.ToUpper()}\n" +
            $"Harga: Rp {DataStok?.HargaJual ?? 0}\n" +
            $"Status: {status}\n" +
            $"Estimasi Waste: {infoWaste}";
    }
}