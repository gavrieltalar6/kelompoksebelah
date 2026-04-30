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
}