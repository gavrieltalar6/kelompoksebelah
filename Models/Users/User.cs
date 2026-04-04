using CakeProject.Models; // Baris ini memberi tahu VS Code untuk melihat folder Models

namespace CakeProject.Models.Users; // Tambahkan .Users

public abstract class User : BaseEntity 
{
    public string Nama { get; set; }
    public string NoHP { get; set; }
}