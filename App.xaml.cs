using CakeProject.Views;
namespace CakeProject;

public partial class App : Application
{
    public static Data.MenuStore TokoData { get; } = new Data.MenuStore();

    public App()
    {
        InitializeComponent();

        // Muat data synchronous sebelum app jalan
        Task.Run(async () => await TokoData.MuatSemuaAsync()).Wait();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell()); // ← langsung AppShell, tidak perlu LoadingPage
    }
}