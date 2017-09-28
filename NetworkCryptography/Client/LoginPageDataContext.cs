using System.ComponentModel;

namespace NetworkCryptography.Client
{
    public class LoginPageDataContext : INotifyPropertyChanged
    {
        private string username;
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
        public string Port
        {
            get => port;
            set
            {
                port = value;
                OnPropertyChanged(port);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
