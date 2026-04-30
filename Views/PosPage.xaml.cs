using CakeProject.ViewModels;

namespace CakeProject.Views;

public partial class PosPage : ContentPage
{
    public PosPage()
    {
        InitializeComponent();
        BindingContext = new PosViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PosViewModel vm)
            vm.RefreshProduk();
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
