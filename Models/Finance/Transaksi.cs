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

       public string CetakStruk()
        {
            string struk = "";
            struk += $"ID: {IDTransaksi}\n";
            struk += $"Tanggal: {Tanggal}\n";
            struk += $"Total: {HitungTotal()}\n";
            
            return struk;
        }
    }
}