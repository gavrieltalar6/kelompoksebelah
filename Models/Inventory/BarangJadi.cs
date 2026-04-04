using System;

namespace CakeProject.Models.Inventory
{
    public class BarangJadi : Barang
    {
        public double HargaJual { get; set; }
        public string Vendor { get; set; }
        public string Dimensi { get; set; }
        public string Warna { get; set; }

        public double HitungProfit()
        {
            return HargaJual - HargaBeli;
        }

        public void TampilkanSpesifikasi()
        {
            Console.WriteLine($"Vendor: {Vendor}");
            Console.WriteLine($"Dimensi: {Dimensi}");
            Console.WriteLine($"Warna: {Warna}");
        }
    }
}