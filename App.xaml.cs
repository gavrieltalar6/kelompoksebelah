using Microsoft.Extensions.DependencyInjection;

namespace CakeProject;

public partial class App : Application
{
    public static Data.MenuStore TokoData { get; } = new Data.MenuStore();

    public App()
    {
        InitializeComponent();
        Task.Run(async () => await TokoData.MuatSemuaAsync());
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}