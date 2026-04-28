using CakeProject.Views;
namespace CakeProject;

public partial class App : Application
{
    public static Data.MenuStore TokoData { get; } = new Data.MenuStore();

    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new LoadingPage());
    }
}