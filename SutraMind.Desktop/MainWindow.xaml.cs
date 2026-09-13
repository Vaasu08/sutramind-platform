using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SutraMind.Desktop.ViewModels;

namespace SutraMind.Desktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        if (DataContext is MainWindowViewModel viewModel)
            viewModel.LogoutRequested += HandleLogoutRequested;
    }

    public void SetAuthenticatedUser(string email)
    {
        if (DataContext is MainWindowViewModel viewModel)
            viewModel.SetAuthenticatedUser(email);
    }

    private void HandleLogoutRequested(object? sender, EventArgs e)
    {
        var app = (App)System.Windows.Application.Current;
        app.ShowLogin(this);
    }
}