using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CakeProject.Data;
using CakeProject.Models.Users;

namespace CakeProject.ViewModels;

public class CustomerViewModel : INotifyPropertyChanged
{
    private readonly CustomerStore _store = new();

    public ObservableCollection<Customer> DaftarCustomer { get; set; } = new();

    // Form tambah customer baru
    private string _namaBaru = string.Empty;
    public string NamaBaru
    {
        get => _namaBaru;
        set { _namaBaru = value; OnPropertyChanged(nameof(NamaBaru)); }
    }

    private string _noHpBaru = string.Empty;
    public string NoHpBaru
    {
        get => _noHpBaru;
        set { _noHpBaru = value; OnPropertyChanged(nameof(NoHpBaru)); }
    }

    private bool _isMemberBaru = false;
    public bool IsMemberBaru
    {
        get => _isMemberBaru;
        set { _isMemberBaru = value; OnPropertyChanged(nameof(IsMemberBaru)); }
    }

    private string _pesanStatus = string.Empty;
    public string PesanStatus
    {
        get => _pesanStatus;
        set { _pesanStatus = value; OnPropertyChanged(nameof(PesanStatus)); OnPropertyChanged(nameof(AdaPesan)); }
    }

    public bool AdaPesan => !string.IsNullOrEmpty(_pesanStatus);

    public ICommand MuatCommand { get; }
    public ICommand TambahCustomerCommand { get; }
    public ICommand HapusCustomerCommand { get; }

    public CustomerViewModel()
    {
        MuatCommand = new Command(async () => await MuatData());
        TambahCustomerCommand = new Command(async () => await TambahCustomer());
        HapusCustomerCommand = new Command<Customer>(async (c) => await HapusCustomer(c));

        _ = MuatData();
    }

    private async Task MuatData()
    {
        await _store.MuatAsync();
        DaftarCustomer.Clear();
        foreach (var c in _store.DaftarCustomer)
            DaftarCustomer.Add(c);
    }

    private async Task TambahCustomer()
    {
        // Validasi
        if (string.IsNullOrWhiteSpace(NamaBaru))
        {
            PesanStatus = "⚠️ Nama tidak boleh kosong.";
            return;
        }
        if (string.IsNullOrWhiteSpace(NoHpBaru))
        {
            PesanStatus = "⚠️ No. HP tidak boleh kosong.";
            return;
        }
        // Cek duplikat NoHP
        if (_store.CariCustomerByNoHP(NoHpBaru) != null)
        {
            PesanStatus = "⚠️ No. HP sudah terdaftar.";
            return;
        }

        // Buat ID baru
        int idBaru = _store.DaftarCustomer.Count > 0
            ? _store.DaftarCustomer.Max(c => c.Id) + 1
            : 1;

        var customerBaru = new Customer
        {
            Id = idBaru,
            Nama = NamaBaru,
            NoHP = NoHpBaru,
            IsMember = IsMemberBaru,
            TotalPoin = 0
        };

        _store.DaftarCustomer.Add(customerBaru);
        DaftarCustomer.Add(customerBaru);
        await _store.SimpanAsync();

        // Reset form
        NamaBaru = string.Empty;
        NoHpBaru = string.Empty;
        IsMemberBaru = false;
        PesanStatus = $"✅ {customerBaru.Nama} berhasil didaftarkan!";
    }

    private async Task HapusCustomer(Customer customer)
    {
        if (customer == null) return;
        _store.DaftarCustomer.Remove(customer);
        DaftarCustomer.Remove(customer);
        await _store.SimpanAsync();
        PesanStatus = $"🗑️ {customer.Nama} dihapus.";
    }

    public event PropertyChangedEventHandler PropertyChanged = null!;
    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
