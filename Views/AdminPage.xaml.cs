using CakeProject.Data;
using CakeProject.Models.Inventory;
using CakeProject.ViewModels;

namespace CakeProject.Views;

public partial class AdminPage : ContentPage
{
    public AdminPage()
    {
        InitializeComponent();
        BindingContext = new AdminViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AdminViewModel vm)
        {
            await vm.RefreshData();
            RenderAlert(vm);
            RenderLaporan(vm);
            RenderProduk(vm);
        }
    }

    private async void OnAdminClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//AdminPage");

    private async void OnGudangClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//GudangPage");

    private async void OnKasirClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//PosPage");

    private async void OnDapurClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//DapurPage");

    private async void OnResepClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//ResepPage");

    private async void OnMemberClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//CustomerPage");
    // ───────────────────────────────────────────────────────────────────────

    private void RenderAlert(AdminViewModel vm)
    {
        AlertContainer.Children.Clear();
        foreach (var alert in vm.DaftarAlert)
        {
            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star)
                }
            };

            var icon = new Label { Text = alert.Icon, FontSize = 16 };
            var pesan = new Label
            {
                Text = alert.Pesan,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(8, 0, 0, 0),
                TextColor = Colors.Black
            };

            grid.Children.Add(icon);
            grid.Children.Add(pesan);
            Grid.SetColumn(icon, 0);
            Grid.SetColumn(pesan, 1);

            AlertContainer.Children.Add(grid);
        }
    }

    private void RenderLaporan(AdminViewModel vm)
    {
        LaporanContainer.Children.Clear();

        var data = App.TokoData.DaftarPenjualan
            .OrderByDescending(p => p.Tanggal)
            .Take(5)
            .ToList();

        if (data.Count == 0)
        {
            LaporanContainer.Children.Add(new Label
            {
                Text = "Belum ada transaksi",
                TextColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Center
            });
            return;
        }

        foreach (var p in data)
        {
            LaporanContainer.Children.Add(new Label
            {
                Text = $"#{p.IDTransaksi} - Rp {p.TotalBayar:N0} - {p.Tanggal:dd MMM HH:mm}",
                TextColor = Colors.Black,
                FontSize = 12,
                Padding = new Thickness(0, 4)
            });
        }
    }

    private void RenderProduk(AdminViewModel vm)
    {
        ProdukContainer.Children.Clear();
        foreach (var p in vm.DaftarProduk)
        {
            var grid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                Padding = new Thickness(0, 8)
            };
            var info = new VerticalStackLayout();
            info.Children.Add(new Label { Text = p.NamaBarang, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
            info.Children.Add(new Label { Text = $"Rp {p.HargaJual:N0}", TextColor = Colors.Black });
            grid.Children.Add(info);

            var btn = new Button
            {
                Text = "Edit Harga",
                FontSize = 11,
                BackgroundColor = Color.FromArgb("#EBD5EB"),
                TextColor = Colors.Black,
                CornerRadius = 10
            };
            var produk = p;
            btn.Clicked += async (s, e) => await EditHarga(produk);
            grid.Children.Add(btn);
            Grid.SetColumn(btn, 1);

            ProdukContainer.Children.Add(grid);
        }
    }

    private async Task EditHarga(StokProduk produk)
    {
        string result = await DisplayPromptAsync(
            "Edit Harga",
            $"Masukkan harga jual baru untuk {produk.NamaBarang}:",
            initialValue: produk.HargaJual.ToString(),
            keyboard: Keyboard.Numeric);

        if (result == null) return;

        if (int.TryParse(result, out int hargaBaru) && hargaBaru > 0)
        {
            produk.HargaJual = hargaBaru;
            await App.TokoData.SimpanProdukAsync();
            await DisplayAlert("Sukses", $"Harga {produk.NamaBarang} diperbarui!", "OK");
            if (BindingContext is AdminViewModel vm)
            {
                await vm.RefreshData();
                RenderAlert(vm);
                RenderLaporan(vm);
                RenderProduk(vm);
            }
        }
        else
        {
            await DisplayAlert("Gagal", "Harga tidak valid!", "OK");
        }
    }
}
