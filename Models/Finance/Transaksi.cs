using System;

namespace CakeProject.Models.Finance
{
    public class Transaksi
    {
        public int IDTransaksi { get; set; }
        public DateTime Tanggal { get; set; }
        public double TotalBayar { get; set; }
        public double Pajak { get; set; }

        public virtual double HitungTotal()
        {
            return TotalBayar + Pajak;
        }

        public void CetakStruk()
        {
            Console.WriteLine($"ID: {IDTransaksi}");
            Console.WriteLine($"Tanggal: {Tanggal}");
            Console.WriteLine($"Total: {HitungTotal()}");
        }
    }
}