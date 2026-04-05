using System;

namespace CakeProject.Models.Inventory
{
    public class BarangJadi : BarangDijual
    {
        public string Vendor { get; set; }
        public string Dimensi { get; set; }
        public string Warna { get; set; }

        public void TampilkanSpesifikasi()
        {
            Console.WriteLine($"Vendor: {Vendor}");
            Console.WriteLine($"Dimensi: {Dimensi}");
            Console.WriteLine($"Warna: {Warna}");
        }
    }
}