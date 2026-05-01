using CakeProject.ViewModels;

namespace CakeProject.Views;

public partial class ResepPage : ContentPage
{
    public ResepPage()
    {
        InitializeComponent();
        BindingContext = new ResepViewModel();
    }

    private async void OnAdminClicked(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//AdminPage");

    private async void OnGudangClicked(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//GudangPage");

    private async void OnKasirClicked(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//PosPage");

    private async void OnDapurClicked(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//DapurPage");

    private async void OnResepClicked(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//ResepPage");

    private async void OnMemberClicked(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//CustomerPage");
}
