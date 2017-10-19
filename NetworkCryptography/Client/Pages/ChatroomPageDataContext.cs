/*
 * Author: Shon Verch
 * File Name: ChatroomPageDataContext.cs
 * Project: NetworkCryptography
 * Creation Date: 10/19/2017
 * Modified Date: 10/19/2017
 * Description: Stores all data in the chatroom page. It is notified when data is changed and updates data accordingly.
 */

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NetworkCryptography.Core;

namespace NetworkCryptography.Client.Pages
{
    public class ChatroomPageDataContext : INotifyPropertyChanged
    {
        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get => users;
            set
            {
                users = value;
                OnPropertyChanged();
            }
        }

        public ChatroomPageDataContext()
        {
            Users = new ObservableCollection<User>
            {
                new User(0, "Lane"),
                new User(0, "Ole"),
                new User(0, "Shon")
            };

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
