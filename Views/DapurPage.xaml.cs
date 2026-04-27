using CakeProject.ViewModels;

namespace CakeProject.Views;

public partial class DapurPage : ContentPage
{
    public DapurPage()
    {
        InitializeComponent();
        BindingContext = new DapurViewModel();
    }
}