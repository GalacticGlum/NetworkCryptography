/*
 * Author: Shon Verch
 * File Name: ChatroomPageDataContext.cs
 * Project: NetworkCryptography
 * Creation Date: 10/19/2017
 * Modified Date: 10/20/2017
 * Description: Stores all data in the chatroom page. It is notified when data is changed and updates data accordingly.
 */

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using NetworkCryptography.Core;

namespace NetworkCryptography.Client.Pages
{
    /// <summary>
    /// Stores all data in the chatroom page. It is notified when data is changed and updates data accordingly.
    /// </summary>
    public sealed class ChatroomPageDataContext : INotifyPropertyChanged
    {
        private ObservableCollection<User> users;

        /// <summary>
        /// A collection of all the users in the chatroom.
        /// </summary>
        public ObservableCollection<User> Users
        {
            get => users;
            set
            {
                users = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ChatMessage> messages;

        /// <summary>
        /// A collection of all the messages sent in the chatroom.
        /// </summary>
        public ObservableCollection<ChatMessage> Messages
        {
            get => messages;
            set
            {
                messages = value;
                OnPropertyChanged();
            }
        }

        public ChatroomPageDataContext()
        {
            Users = new ObservableCollection<User>();
            Messages = new ObservableCollection<ChatMessage>();

            // Initialize user events
            CoreClientApp.Client.UserManager.NewUserJoined += OnNewUserJoined;
            CoreClientApp.Client.UserManager.UserLeft += OnUserLeft;

            // Initialize chat message events
            CoreClientApp.Client.ChatMessageManager.ChatMessageReceived += OnChatMessageReceived;
        }

        /// <summary>
        /// Handles the <see cref="NewUserJoinedEventHandler"/>. Adds the <see cref="User"/> to the Users list. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnNewUserJoined(object sender, UserEventArgs args) => RunSafely(() => Users.Add(args.User));

        /// <summary>
        /// Handles the <see cref="UserLeftEventHandler"/>. Removes the <see cref="User"/> from the Users list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnUserLeft(object sender, UserEventArgs args) => RunSafely(() => Users.Remove(args.User));

        /// <summary>
        /// Handles the <see cref="ChatMessageReceivedEventHandler"/>. Adds the <see cref="ChatMessage"/> to the Messages list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnChatMessageReceived(object sender, ChatMessageEventArgs args) => RunSafely(() => Messages.Add(args.ChatMessage));

        /// <summary>
        /// Runs the <paramref name="action"/> on the UI thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        private static void RunSafely(Action action) => Application.Current.Dispatcher.Invoke(action);

        /// <summary>
        /// Event for notification of a change of data.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="name"></param>
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            if (name != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
