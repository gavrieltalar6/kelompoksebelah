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

        public string TampilkanLaporan()
        {
           string laporan = "=== LAPORAN ===\n";
        laporan += $"Pemasukan: {TotalPemasukan}\n";
        laporan += $"Pengeluaran: {TotalPengeluaran}\n";
        laporan += $"Laba: {HitungLaba()}";

        return laporan;
        }
    }
}