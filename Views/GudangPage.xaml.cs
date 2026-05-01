using CakeProject.ViewModels;

namespace CakeProject.Views;

public partial class GudangPage : ContentPage
{
    public GudangPage()
    {
        InitializeComponent();
        BindingContext = new GudangViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is GudangViewModel vm)
            vm.RefreshData();
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
