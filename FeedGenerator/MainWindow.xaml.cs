using System.Collections.Generic;
using System.Windows;
using System.Threading.Tasks;

namespace FeedGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //static Server server = new Server();
        Task _task;

        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            //todo:
            int port = 8050;
            string feedTitle = "Michal test feed";
            var filePaths = new List<string>();
            filePaths.Add(@"c:\@Downloads\Podcasts\odkryj_swoje_przekonania-michal_pasterski.mp3");
            filePaths.Add(@"c:\@Downloads\Podcasts\odrzuc_perfekcjonizm_i_dzialaj - michal_pasterski.mp3");

            var imagePath = @"c:\@Downloads\ms.jpg";

            _task = Task.Factory.StartNew(() => new Server(port, feedTitle, filePaths, imagePath).Start());
        }

        private void btnStopServer_Click(object sender, RoutedEventArgs e)
        {

            // todo: cancelation service
            //server.Stop();
            MessageBox.Show(_task.IsCompleted.ToString());
        }

    }
}
