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
        try
        {
            await App.TokoData.MuatSemuaAsync();
            await DisplayAlert("Loading selesai",
                $"Penjualan dimuat: {App.TokoData.DaftarPenjualan.Count}", "OK");
            Application.Current.MainPage = new AppShell();
        }
        catch (Exception ex)
        {
            await DisplayAlert("ERROR", ex.Message + "\n\n" + ex.StackTrace, "OK");
        }
    }
}
