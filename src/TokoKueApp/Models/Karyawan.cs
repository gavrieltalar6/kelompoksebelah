namespace CakeProject.Models
{
    public class Karyawan : User
    {
        public string IDPegawai {get; set;}
        public string Jabatan {get; set;}
        public double Gaji {get; set;}
        public Karyawan(string nama, string noHP, string idPegawai, string jabatan, double gaji) : base(nama, noHP)
        {
            IDPegawai = idPegawai;
            Jabatan = jabatan;
            Gaji = gaji;
        }
    }
}