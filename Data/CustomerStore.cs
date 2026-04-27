using CakeProject.Models.Users;
using CakeProject.Services;

namespace CakeProject.Data;

public class CustomerStore
{
    private const string FileCustomer = "daftar_customer.json";
    
    public List<Customer> DaftarCustomer { get; set; } = new();

    public async Task MuatAsync()
    {
        DaftarCustomer = await JsonService.MuatAsync<List<Customer>>(FileCustomer)
                         ?? BuatDataAwal();
    }

    public async Task SimpanAsync() =>
        await JsonService.SimpanAsync(FileCustomer, DaftarCustomer);

    public Customer CariCustomer(int idCustomer)
    {
        return DaftarCustomer.FirstOrDefault(c => c.Id == idCustomer);
    }

    public Customer CariCustomerByNoHP(string noHP)
    {
        return DaftarCustomer.FirstOrDefault(c => c.NoHP == noHP);
    }

    public async Task UpdatePoinCustomer(int idCustomer, int poinBaru)
    {
        var customer = CariCustomer(idCustomer);
        if (customer != null)
        {
            customer.TotalPoin = poinBaru;
            await SimpanAsync();
        }
    }

    private List<Customer> BuatDataAwal() => new()
    {
        new Customer 
        { 
            Id = 1, 
            Nama = "Budi Santoso", 
            NoHP = "081234567890", 
            IsMember = true, 
            TotalPoin = 500 
        },
        new Customer 
        { 
            Id = 2, 
            Nama = "Ani Wijaya", 
            NoHP = "082345678901", 
            IsMember = true, 
            TotalPoin = 300 
        },
        new Customer 
        { 
            Id = 3, 
            Nama = "Citra Dewi", 
            NoHP = "083456789012", 
            IsMember = false, 
            TotalPoin = 0 
        },
    };
}
