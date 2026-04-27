using System;

namespace CakeProject.Models.Finance
{
    public class Penjualan : Transaksi
    {
        public int IDCustomer { get; set; }
        public string MetodeBayar { get; set; }
        public double DiskonDiterima { get; set; } // ← UBAH dari DiskonPoin jadi DiskonDiterima

        public override double HitungTotal()
        {
            double total = TotalBayar - DiskonDiterima + Pajak; // ← Pakai DiskonDiterima
            return total;
        }

        public bool ValidasiPembayaran(double uang)
        {
            return uang >= HitungTotal();
        }

        public (bool success, string message, double totalAkhir) ProsesPenjualan(Customer customer, int poinDitukar)
        {
            // Customer tukar poin
            var result = customer.TukarPoin(poinDitukar);
            
            if (!result.Item1) // Jika gagal
            {
                return (false, result.Item2, 0); // ← TAMBAH return untuk case gagal
            }

            // Jika berhasil
            DiskonDiterima = result.Item3; // Ambil diskon
            double totalAkhir = HitungTotal();
            
            return (true, "Penjualan berhasil.", totalAkhir);
        }
    }
}