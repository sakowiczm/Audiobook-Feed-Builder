using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

/* todo:
 * - add multiple files at once, extend file filtering option
 * - files up/down, remove
 */

namespace FeedGenerator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Files = new ObservableCollection<string>();
            lstFiles.ItemsSource = Files;
        }

        private Task _task;
        public ObservableCollection<string> Files { get; set; }
        public string ImagePath { get; set; }

        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            int port = string.IsNullOrWhiteSpace(txtPort.Text) ? 6000 : Convert.ToInt32(txtPort.Text);
            string feedTitle = txtTitle.Text ?? "Default title";
            var filePaths = new List<string>(Files);

            _task = Task.Factory.StartNew(() => new Server(port, feedTitle, filePaths, ImagePath).Start());

            // todo: feed url should be returned by the Server
            txtGeneratedFeed.Text = string.Format(@"http://localhost:{0}/{1}", port, "feed.xml");
        }

        private void btnStopServer_Click(object sender, RoutedEventArgs e)
        {

            // todo: cancelation service
            //server.Stop();
            MessageBox.Show(_task.IsCompleted.ToString());
        }

        private void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".mp3";
            dlg.Filter = "Mp3 file (.mp3)|*.mp3";

            bool? result = dlg.ShowDialog();

            if(!result.HasValue || result == false)
                return;

            Files.Add(dlg.FileName);
        }

        private void btnAddCover_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Jpg file (.jpg)|*.jpg";

            bool? result = dlg.ShowDialog();

            if (!result.HasValue || result == false)
                return;

            ImagePath = dlg.FileName;
            imgCover.Source = new BitmapImage(new Uri(ImagePath));
        }

        private void btnClearCover_Click(object sender, RoutedEventArgs e)
        {
            ImagePath = null;
            imgCover.Source = null;
        }

        private void btnRemoveFile_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
