using System;

namespace CakeProject.Models.Inventory
{
    public class BarangJadi : BarangDijual
    {
        public string Vendor { get; set; }
        public string Dimensi { get; set; }
        public string Warna { get; set; }

        public string TampilkanSpesifikasi()
        {
            return $"Vendor: {Vendor}\n" +
            $"Dimensi: {Dimensi}\n" +
            $"Warna: {Warna}";
        }
    }
}