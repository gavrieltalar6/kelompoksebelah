using System;

namespace CakeProject.Models.Finance
{
    public class Penjualan : Transaksi
    {
        public int IDCustomer { get; set; }
        public string MetodeBayar { get; set; }
        public int DiskonPoin { get; set; }

        public override double HitungTotal()
        {
            double total = TotalBayar - DiskonPoin + Pajak;
            return total;
        }

        public bool ValidasiPembayaran(double uang)
        {
            return uang >= HitungTotal();
        }
    }
}