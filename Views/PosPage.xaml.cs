using CakeProject.ViewModels;

namespace CakeProject.Views;

public partial class PosPage : ContentPage
{
    public PosPage()
    {
        InitializeComponent();
        BindingContext = new PosViewModel();
    }
}