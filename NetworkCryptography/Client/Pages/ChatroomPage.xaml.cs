/*
 * Author: Shon Verch
 * File Name: ChatroomPage.xaml.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/20/2017
 * Modified Date: 10/21/2017
 * Description: Interaction logic for ChatroomPage.xaml
 */

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace NetworkCryptography.Client.Pages
{
    /// <summary>
    /// Interaction logic for ChatroomPage.xaml
    /// </summary>
    public partial class ChatroomPage
    {
        /// <summary>
        /// Initializes a <see cref="ChatroomPage"/>.
        /// </summary>
        public ChatroomPage()
        {
            InitializeComponent();
            DataContext = new ChatroomPageDataContext();
        }

        /// <summary>
        /// Handle a send button click event. When this occurs, send the message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnSendButtonClicked(object sender, RoutedEventArgs args) => SendMessage();

        /// <summary>
        /// Handle the message textbox key events. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMessageTextboxPreviewKeyDown(object sender, KeyEventArgs args)
        {
            // We only want to send the message if we press enter without CTRL and/or SHIFT down.
            bool isCtrlUp = !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
            bool isShiftUp = !(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));

            // If we are pressing enter and we have ctrl or shift up then bail!
            if (args.Key != Key.Enter || !isCtrlUp || !isShiftUp) return;

            SendMessage();
            args.Handled = true;
        }

        /// <summary>
        /// Send the current message.
        /// </summary>
        private void SendMessage()
        {
            // If our text message is empty, don't send anything!
            string textMessage = MessageTextBox.Text;
            if (string.IsNullOrEmpty(textMessage)) return;

            // Clear our message text box and send the message.
            MessageTextBox.Text = string.Empty;
            CoreClientApp.Client.ChatMessageManager.SendMessage(textMessage);
        }

        /// <summary>
        /// Handle the load event for the message items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMessageItemsLoaded(object sender, RoutedEventArgs args)
        {
            ScrollViewer scrollViewer = (ScrollViewer) VisualTreeHelper.GetChild(MessageItems, 0);
            scrollViewer.ScrollChanged += OnScrollChanged;
        }

        /// <summary>
        /// Handle a scroll change in our message list control scroll viewer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnScrollChanged(object sender, ScrollChangedEventArgs args)
        {
            ScrollViewer scrollViewer = (ScrollViewer) sender;
            ((ChatroomPageDataContext)DataContext).IsScrolledToBottom = scrollViewer.IsScrolledToBottom();
        }

        /// <summary>
        /// Handle a message notification click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageNotificationClicked(object sender, MouseButtonEventArgs e)
        {
            // "Jump to present" - scroll to our latest item (the bottom).
            ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(MessageItems, 0);
            scrollViewer.ScrollToBottom();
        }

        /// <summary>
        /// Handle the click event of the settings button.
        /// Opens the settings dialog panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            SettingsDialogPanel.IsOpen = true;
        }
    }
}
