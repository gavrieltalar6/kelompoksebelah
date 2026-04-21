namespace CakeProject.Models.Production;

internal class ResepKue
{
    private Dictionary<string, double> DaftarBahanBaku { get; set; }
    private double LamaMasak { get; set; }

    public ResepKue(Dictionary<string, double> bahan, double lama)
    {
        DaftarBahanBaku = bahan;
        LamaMasak = lama;
    }

    internal double KalkulasiWaste()
    {
        double totalBeratGram = 0;

        if (DaftarBahanBaku != null)
        {
            foreach (var item in DaftarBahanBaku)
            {
                string namaBahan = item.Key.ToLower();
                double jumlah = item.Value;

                if (namaBahan.Contains("telur"))
                {
                    totalBeratGram += (jumlah * 50);
                }
                else
                {
                    totalBeratGram += jumlah;
                }
            }
        }

        return totalBeratGram * 0.05;
    }
}