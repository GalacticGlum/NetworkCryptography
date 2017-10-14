using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace NetworkCryptography.Client.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage
    {
        public LoginPage()
        {
            InitializeComponent();
            DataContext = new LoginPageDataContext();
        }

        private void ValidatePortText(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }
            
        private void OnConnectButtonClick(object sender, RoutedEventArgs e)
        {
            // Validate and get the result of each validation, 
            BindingExpression usernameBindingExpression = UsernameTextBox.GetBindingExpression(TextBox.TextProperty);
            BindingExpression ipAddressBindingExpression = IpAddressTextBox.GetBindingExpression(TextBox.TextProperty);
            BindingExpression portBindingExpression = PortTextBox.GetBindingExpression(TextBox.TextProperty);

            usernameBindingExpression?.UpdateSource();
            ipAddressBindingExpression?.UpdateSource();
            portBindingExpression?.UpdateSource();

            bool usernameHasError = usernameBindingExpression?.HasError ?? true;
            bool ipAddressHasError = ipAddressBindingExpression?.HasError ?? true;
            bool portHasError = portBindingExpression?.HasError ?? true;

            if (usernameHasError || ipAddressHasError || portHasError) return;

            MainWindow mainWindow = (MainWindow) Window.GetWindow(this);
            mainWindow?.SwitchToChatPage();

            LoginPageDataContext loginPageDataContext = (LoginPageDataContext) DataContext;
            CoreClientApp.Run(loginPageDataContext.Username, loginPageDataContext.IpAddress, 
                int.Parse(loginPageDataContext.Port));
        }
    }
}
