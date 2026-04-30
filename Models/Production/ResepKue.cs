namespace CakeProject.Models.Production;

public class ResepKue
{
    public Dictionary<string, double> DaftarBahanBaku { get; set; } = new();
    public double LamaMasak { get; set; }

    public ResepKue() { } // constructor kosong untuk JSON deserialize

    public ResepKue(Dictionary<string, double> bahan, double lama)
    {
        DaftarBahanBaku = bahan;
        LamaMasak = lama;
    }

    public double KalkulasiWaste()
    {
        double totalBeratGram = 0;

        if (DaftarBahanBaku != null)
        {
            foreach (var item in DaftarBahanBaku)
            {
                string namaBahan = item.Key.ToLower();
                double jumlah = item.Value;

                if (namaBahan.Contains("telur"))
                    totalBeratGram += (jumlah * 50);
                else
                    totalBeratGram += jumlah;
            }
        }

        return totalBeratGram * 0.05;
    }
}