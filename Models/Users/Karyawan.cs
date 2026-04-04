namespace CakeProject.Models.Users;

public class Karyawan : User
{
    public string IDPegawai {get; set;}
    public string Jabatan {get; set;}
    public double Gaji {get; set;}
    public void Presensi()
    {
        string waktuSekarang = DateTime.Now.ToString("HH:mm:ss");
        Console.WriteLine($"Pegawai {Nama} ({IDPegawai}) berhasil absen pada pukul {waktuSekarang}");
    }
}
