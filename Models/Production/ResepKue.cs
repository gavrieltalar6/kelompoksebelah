namespace CakeProject.Models.Production;

internal class ResepKue;
    private Dictionary<string, double> DaftarBahanBaku { get; set; }// Key: Nama Bahan, Value: Jumlah (Bisa gram, ml, atau butir)
    private double LamaMasak { get; set; } // satuannya menit

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

                if (namaBahan.Contains("telur")) //nanti pas isi data, gaboleh isi "telor"
                {
                    totalBeratGram += (jumlah * 50);// Konversi butir ke gram (asumsi 1 butir = 50g)
                }
                else
                {
                    totalBeratGram += jumlah;                    // Asumsi bahan lain sudah dalam gram/ml
                }
            }
        }
        // Return 5% dari total berat sebagai waste
        return totalBeratGram * 0.05;
    }