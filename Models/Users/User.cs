using CakeProject.Models;

namespace CakeProject.Models.Users;

public abstract class User : BaseEntity 
{
    public string Nama { get; set; }
    public string NoHP { get; set; }
}