namespace CakeProject.Models
{
    public class User
    {
        public string Nama {get; set;}
        public string NoHP {get; set;}

        public User (string nama, string noHP)
        {
            Nama = nama;
            NoHP= noHP;
        }
    }
}