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
    public class ChatMessage
    {
        public User User { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }

        public ChatMessage(User user, string message, DateTime time)
        {
            User = user;
            Message = message;
            Time = time;
        }
    }

    public class ChatroomPageDataContext : INotifyPropertyChanged
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

            CoreClientApp.Client.UserManager.UserListReceived += HandleUserListReceived;
            CoreClientApp.Client.UserManager.NewUserJoined += HandleNewUserJoined;

            //Messages = new ObservableCollection<ChatMessage>
            //{
            //    new ChatMessage(userShon, "Hello, I am batman!", new DateTime(2017, 10, 19, 19, 20, 10)),
            //    new ChatMessage(userLane, "No, I am batman!", new DateTime(2017, 10, 19, 19, 22, 56)),
            //    new ChatMessage(userOle, "Wait, I thought Elessar was batman?", new DateTime(2017, 10, 19, 19, 23, 37)),
            //    new ChatMessage(userShon, "No! I am batman!", new DateTime(2017, 10, 19, 19, 26, 7)),
            //    new ChatMessage(userShon, "Actually maybe I'm not batman.", new DateTime(2017, 10, 19, 19, 26, 42)),
            //    new ChatMessage(userShon, "Frankly, who even knows!", new DateTime(2017, 10, 19, 19, 27, 41)),
            //    new ChatMessage(userShon, "Ahhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh!" +
            //                              "THIS IS A LONG MESSAGE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" +
            //                              "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" +
            //                              "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", new DateTime(2017, 10, 19, 19, 27, 41))
            //};
        }

        private void HandleUserListReceived(object sender, UserListEventArgs args)
        {
            Users = new ObservableCollection<User>(args.Users);
        }

        private void HandleNewUserJoined(object sender, UserEventArgs args)
        {
            // Add the user to the ObservableCOllection on the UI thread, otherwise the change isn't registered.
            Application.Current.Dispatcher.Invoke(() =>
            {
                Users.Add(args.User);
            });
        }

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
