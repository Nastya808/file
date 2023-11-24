using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            string fileUrl = urlTextBox.Text;
            if (string.IsNullOrEmpty(fileUrl))
            {
                MessageBox.Show("Enter a valid file URL!");
                return;
            }

            try
            {
                await DownloadFileAsync(new Uri(fileUrl));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading file: {ex.Message}");
            }
        }

        private async Task DownloadFileAsync(Uri uri)
        {
            downloadButton.IsEnabled = false;
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                byte[] fileData = await webClient.DownloadDataTaskAsync(uri);
                string fileName = System.IO.Path.GetFileName(uri.ToString());
                if (fileName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
                {
                    for (int i = 0; i < fileName.Length; i++)
                    {
                        if (System.IO.Path.GetInvalidFileNameChars().Contains(fileName[i]))
                        {
                            fileName = fileName.Remove(i, 1);
                        }
                    }
                }
                SaveFileData(fileData, fileName);
            }
            downloadButton.IsEnabled = true;
        }

        private void SaveFileData(byte[] fileData, string fileName)
        {
            string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            System.IO.File.WriteAllBytes(filePath, fileData);
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadProgressBar.Value = e.ProgressPercentage;
        }
    }
}
