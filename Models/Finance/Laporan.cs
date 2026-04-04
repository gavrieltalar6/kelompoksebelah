using System;
using System.Collections.Generic;

namespace CakeProject.Models.Finance
{
    public class Laporan
    {
        public int IDLaporan { get; set; }
        public DateTime TanggalMulai { get; set; }
        public DateTime TanggalSelesai { get; set; }

        public double TotalPemasukan { get; set; }
        public double TotalPengeluaran { get; set; }

        public double HitungLaba()
        {
            return TotalPemasukan - TotalPengeluaran;
        }

        public void TampilkanLaporan()
        {
            Console.WriteLine("=== LAPORAN ===");
            Console.WriteLine($"Pemasukan: {TotalPemasukan}");
            Console.WriteLine($"Pengeluaran: {TotalPengeluaran}");
            Console.WriteLine($"Laba: {HitungLaba()}");
        }
    }
}