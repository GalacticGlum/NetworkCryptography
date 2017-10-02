/*
 * Author: Shon Verch
 * File Name: LoginPageDataContext.cs
 * Project: NetworkCryptography
 * Creation Date: 9/27/2017
 * Modified Date: 9/27/2017
 * Description: Stores all data in the login page. It is notified when data is changed and updates data accordingly.
 */

using System.ComponentModel;

namespace NetworkCryptography.Client
{
    /// <summary>
    /// Stores all data in the login page. 
    /// It is notified when data is changed and updates data accordingly.
    /// </summary>
    public class LoginPageDataContext : INotifyPropertyChanged
    {
        private string username;

        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged(username);
            }
        }

        private string ipAddress;

        /// <summary>
        /// The IP address to connect to.
        /// </summary>
        public string IpAddress
        {
            get => ipAddress;
            set
            {
                ipAddress = value;
                OnPropertyChanged(ipAddress);
            }
        }

        private string port;

        /// <summary>
        /// The port to connect with.
        /// </summary>
        public string Port
        {
            get => port;
            set
            {
                port = value;
                OnPropertyChanged(port);
            }
        }

        /// <summary>
        /// Event for notification of a change of data.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="name"></param>
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
