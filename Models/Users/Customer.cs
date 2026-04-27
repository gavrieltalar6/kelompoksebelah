namespace CakeProject.Models.Users;
public class Customer : User
{
    public bool IsMember {get; set;}
    public int TotalPoin {get; set;}
    public (bool success, string message, int diskon) TukarPoin(int poinYangDitukar)
    {
        if (!IsMember)
        return (false, "Hanya member yang bisa tukar poin.", 0);

        if (poinYangDitukar <= 0)
            return (false, "Jumlah poin tidak valid.", 0);

        if (poinYangDitukar > TotalPoin)
            return (false, "Poin tidak cukup.", 0);

        if (poinYangDitukar < 100)
            return (false, "Minimal penukaran 100 poin.", 0);

        int diskon = (poinYangDitukar / 100) * 10000;

        // Kurangi poin
        TotalPoin -= poinYangDitukar;

        return (true, "Penukaran poin berhasil.", diskon);
    }
}
