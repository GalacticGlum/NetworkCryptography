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
    }
}
