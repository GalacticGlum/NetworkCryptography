using System;
using System.Windows;
using System.Windows.Input;
using NetworkCryptography.Core;

namespace NetworkCryptography.Client.Pages
{
    /// <summary>
    /// Interaction logic for ChatroomPage.xaml
    /// </summary>
    public partial class ChatroomPage
    {
        public ChatroomPage()
        {
            InitializeComponent();
            DataContext = new ChatroomPageDataContext();
        }

        private void OnSendButtonClicked(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void OnMessageTextboxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // We only want to send the message if we press enter without CTRL and/or SHIFT down.
            bool isCtrlUp = !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
            bool isShiftUp = !(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));

            // If we are pressing enter and we have ctrl or shift up then bail!
            if (e.Key != Key.Enter || !isCtrlUp || !isShiftUp) return;

            SendMessage();
            e.Handled = true;
        }

        private void SendMessage()
        {
            string textMessage = MessageTextBox.Text;
            if (string.IsNullOrEmpty(textMessage)) return;

            MessageTextBox.Text = string.Empty;

            ChatMessage message = new ChatMessage(CoreClientApp.Client.UserManager.BelongingUser, textMessage, DateTime.Now);
            ChatroomPageDataContext dataContext = (ChatroomPageDataContext)DataContext;
            dataContext.Messages.Add(message);
        }
    }
}
