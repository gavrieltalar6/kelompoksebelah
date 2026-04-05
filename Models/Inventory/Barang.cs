namespace CakeProject.Models.Inventory
{
    public class Barang
    {
        public int IDBarang { get; set; }
        public string NamaBarang { get; set; }
        public int HargaBeli { get; set; }
        public double JumlahStok { get; set; }
        public double MinimalStok { get; set; }
        public string Satuan { get; set; }

        public void TambahStok(double jumlah)
        {
            JumlahStok += jumlah;
        }

        public void KurangiStok(double jumlah)
        {
            if (JumlahStok >= jumlah)
                JumlahStok -= jumlah;
            else
                Console.WriteLine("Stok tidak cukup!");
        }

        public bool CekStokRendah()
        {
            return JumlahStok < MinimalStok;
        }
    }
}