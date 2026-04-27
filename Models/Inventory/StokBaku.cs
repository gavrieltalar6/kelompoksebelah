using System;

namespace CakeProject.Models.Inventory
{
    public class StokBaku : Barang
    {
        public DateTime TglExpired { get; set; }

        public bool CekKadaluarsa()
        {
            return DateTime.Today > TglExpired.Date;
        }
    }
}