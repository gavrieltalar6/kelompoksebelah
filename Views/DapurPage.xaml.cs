using CakeProject.ViewModels;

namespace CakeProject.Views;

public partial class DapurPage : ContentPage
{
    public DapurPage()
    {
        InitializeComponent();
        BindingContext = new DapurViewModel();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is DapurViewModel vm)
            vm.RefreshData();
    }
}