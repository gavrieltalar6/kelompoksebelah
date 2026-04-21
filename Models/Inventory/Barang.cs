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

        public bool KurangiStok(double jumlah)
        {
            if (JumlahStok >= jumlah)
            {
                JumlahStok -= jumlah;
                return true;
            }
            return false;
        }

        public bool CekStokRendah()
        {
            return JumlahStok < MinimalStok;
        }
    }
}