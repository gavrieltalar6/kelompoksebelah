using CakeProject.ViewModels;

namespace CakeProject.Views;

public partial class CustomerPage : ContentPage
{
    public CustomerPage()
    {
        InitializeComponent();
        BindingContext = new CustomerViewModel();
    }
}
