/*
 * Author: Shon Verch
 * File Name: MainWindow.xaml.cs
 * Project Name: NetworkCryptography
 * Creation Date: 9/25/2017
 * Modified Date: 10/16/2017
 * Description: Interaction logic for MainWindow.xaml
 */

using System.ComponentModel;
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

        private void OnWindowClose(object sender, CancelEventArgs e)
        {
            CoreClientApp.Quit();
        }

        public void ReturnToLoginPage()
        {
            Content = new LoginPage();   
        }
    }
}
