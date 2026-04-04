namespace CakeProject.Models;

public abstract class BaseEntity
{
    // Properti Identitas Unik (ID)
    public int Id { get; set; }

    // Properti Waktu Data Dibuat
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}