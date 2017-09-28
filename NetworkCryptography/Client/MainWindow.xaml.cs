using System.Windows;
using NetworkCryptography.Client.Pages;

namespace NetworkCryptography.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Content = new LoginPage();
        }

        public void SwitchToChatPage()
        {
            ResizeMode = ResizeMode.CanResize;
            Content = new ChatroomPage();
        }
    }
}
