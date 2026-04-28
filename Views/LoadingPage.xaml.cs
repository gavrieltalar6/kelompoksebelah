namespace CakeProject.Views;

public partial class LoadingPage : ContentPage
{
    public LoadingPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Tunggu data selesai dimuat
        await App.TokoData.MuatSemuaAsync();
        
        // Baru pindah ke AppShell
        Application.Current.MainPage = new AppShell();
    }
}