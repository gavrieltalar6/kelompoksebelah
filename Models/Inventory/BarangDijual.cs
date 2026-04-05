namespace CakeProject.Models.Inventory
{
    public class BarangDijual : Barang
    {
        public int HargaJual { get; set; }
        public string Kategori { get; set; }

        public int HitungProfit()
        {
            return HargaJual - HargaBeli;
        }
    }
}