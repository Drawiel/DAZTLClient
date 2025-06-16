using DAZTLClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DAZTLClient.Windows
{
    /// <summary>
    /// Interaction logic for CreatePlaylistWindow.xaml
    /// </summary>
    public partial class CreatePlaylistWindow : Window
    {
        private string _imagePath = null;
        private readonly ContentService _contentService = new ContentService();
        public CreatePlaylistWindow()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            string? playlistName = TxtPlaylistName.Text?.Trim();

            if (string.IsNullOrEmpty(playlistName))
            {
                MessageBox.Show("El nombre de la playlist no puede estar vacío.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var result = await _contentService.CreatePlaylistAsync(playlistName, _imagePath);

                if (result.Status == "success")
                {
                    MessageBox.Show("Playlist creada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Error al crear playlist: {result.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Excepción al crear playlist: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            if (dlg.ShowDialog() == true)
            {
                _imagePath = dlg.FileName;
                LblImagePath.Text = System.IO.Path.GetFileName(_imagePath);
            }
        }
    }
}
