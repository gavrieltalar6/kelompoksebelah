using CakeProject.ViewModels;

namespace CakeProject.Views;

public partial class GudangPage : ContentPage
{
    public GudangPage()
    {
        InitializeComponent();
        BindingContext = new GudangViewModel();
    }
}