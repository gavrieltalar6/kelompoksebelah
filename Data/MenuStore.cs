using CakeProject.Models.Production;
using CakeProject.Models.Inventory;
using System.Collections.Generic;

namespace CakeProject.Data;
    public class MenuStore
    {
        // 1. Wadah Utama (List of ListKue)
        public List<ListKue> DaftarMenu { get; set; } = new List<ListKue>();

        public void InisialisasiData()
        {
            // --- KUE 1: BOLU MACAN ---
            var bahanBolu = new Dictionary<string, double> { { "Tepung", 500 }, { "Telur", 4 } };// Dictionary bahan
            ResepKue resepBolu = new ResepKue(bahanBolu, 1.5); // itu bahanBolu dictionary isi resep, 1.5 itu lama masaknya
            DaftarMenu.Add(new ListKue { NamaKue = "Bolu Macan", DetailResep = resepBolu }); //di add ke menu utama

            // --- KUE 2: BROWNIES ---
            var bahanBrownies = new Dictionary<string, double> { { "Cokelat", 200 }, { "Telur", 2 } };
            ResepKue resepBrownies = new ResepKue(bahanBrownies, 0.75);
            DaftarMenu.Add(new ListKue { NamaKue = "Fudgy Brownies", DetailResep = resepBrownies });
        }
    }