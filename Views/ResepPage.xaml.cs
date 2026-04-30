using CakeProject.ViewModels;

namespace CakeProject.Views;

public partial class ResepPage : ContentPage
{
    public ResepPage()
    {
        InitializeComponent();
        BindingContext = new ResepViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ResepViewModel vm)
        {
            RenderBahan(vm);
            vm.DaftarBahan.CollectionChanged += (s, e) => RenderBahan(vm);
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.KueTerpilih))
                    RenderBahan(vm);
            };
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is ResepViewModel vm)
        {
            vm.DaftarBahan.CollectionChanged -= (s, e) => RenderBahan(vm);
        }
    }

    public void RenderBahan(ResepViewModel vm)
    {
        BahanContainer.Children.Clear();
        foreach (var bahan in vm.DaftarBahan)
        {
            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(new GridLength(80, GridUnitType.Absolute)),
                    new ColumnDefinition(new GridLength(60, GridUnitType.Absolute)),
                    new ColumnDefinition(GridLength.Auto)
                },
                Padding = new Thickness(0, 4)
            };

            grid.Children.Add(new Label
            {
                Text = bahan.NamaBahan,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Black,
                VerticalOptions = LayoutOptions.Center
            });

            var entry = new Entry
            {
                Text = bahan.Jumlah.ToString(),
                Keyboard = Keyboard.Numeric,
                WidthRequest = 80
            };
            entry.TextChanged += (s, e) =>
            {
                if (double.TryParse(e.NewTextValue, out double val) && val > 0)
                    bahan.Jumlah = val;
            };
            grid.Children.Add(entry);
            Grid.SetColumn(entry, 1);

            var satuan = new Label
            {
                Text = bahan.Satuan,
                TextColor = Colors.Gray,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(5, 0)
            };
            grid.Children.Add(satuan);
            Grid.SetColumn(satuan, 2);

            var hapusBtn = new Button
            {
                Text = "🗑",
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.Red,
                FontSize = 16,
                Padding = new Thickness(0)
            };
            var bahanRef = bahan;
            hapusBtn.Clicked += (s, e) =>
            {
                if (BindingContext is ResepViewModel vm)
                {
                    vm.DaftarBahan.Remove(bahanRef);
                    RenderBahan(vm);
                }
            };
            grid.Children.Add(hapusBtn);
            Grid.SetColumn(hapusBtn, 3);

            BahanContainer.Children.Add(grid);
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
}
