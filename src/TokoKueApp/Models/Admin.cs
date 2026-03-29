namespace CakeProject.Models
{
    public class Admin : Karyawan
    {
        public Admin (string nama, string noHP, string idPegawai, double gaji) : base (nama, noHP, idPegawai, "Admin", gaji)
        {
            //emang kosong
        }

        public void LihatLaporanRugiLaba()
        {
            //nanti ditaro logic
        }
    }
}