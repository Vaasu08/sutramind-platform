using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SutraMind.Desktop.Services;
using SutraMind.Desktop.ViewModels;

namespace SutraMind.Desktop;

public partial class MainWindow : Window
{
    public MainWindow(IServiceProvider services)
    {
        InitializeComponent();
        DataContext = services.GetRequiredService<MainWindowViewModel>();
        if (DataContext is MainWindowViewModel viewModel)
            viewModel.LogoutRequested += HandleLogoutRequested;
    }

    public async void SetAuthenticatedUser(string email)
    {
        await DesktopBootstrap.UpdateSessionAsync(((App)System.Windows.Application.Current).Services, email);
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.SetAuthenticatedUser(email);
            await viewModel.RefreshShellAsync();
        }
    }

    private void HandleLogoutRequested(object? sender, EventArgs e)
    {
        var app = (App)System.Windows.Application.Current;
        app.ShowLogin(this);
    }
}
