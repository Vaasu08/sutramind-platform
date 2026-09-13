using System.Windows;

namespace SutraMind.Desktop;

public partial class LoginWindow : Window
{
    private static readonly IReadOnlyDictionary<string, string> DemoAccounts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["admin@sutramind.local"] = "Demo@123",
        ["pi@sutramind.local"] = "Demo@123",
        ["coordinator@sutramind.local"] = "Demo@123",
        ["monitor@sutramind.local"] = "Demo@123",
        ["ethics@sutramind.local"] = "Demo@123",
        ["pv@sutramind.local"] = "Demo@123"
    };

    public string AuthenticatedEmail { get; private set; } = string.Empty;

    public LoginWindow()
    {
        InitializeComponent();
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        var email = EmailTextBox.Text.Trim();
        var password = PasswordBox.Password;
        if (!DemoAccounts.TryGetValue(email, out var expectedPassword) || expectedPassword != password)
        {
            ErrorText.Text = "The email or password is not valid. Use a demo account and password Demo@123.";
            PasswordBox.Clear();
            PasswordBox.Focus();
            return;
        }

        AuthenticatedEmail = email;
        DialogResult = true;
    }
}
