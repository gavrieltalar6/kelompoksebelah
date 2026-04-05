namespace CakeProject.Models.Inventory
{
    public class StokProduk : BarangDijual
    {
        public DateTime TglExpired {get; set;}
        public bool CekKadaluarsa()
        {
            return DateTime.Now > TglExpired;
        }
    }
}