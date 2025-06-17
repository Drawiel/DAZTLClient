using DAZTLClient.Services;
using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DAZTLClient.Windows
{
    /// <summary>
    /// Lógica de interacción para GUI_ArtistAddSong.xaml
    /// </summary>
    public partial class GUI_ArtistAddSong : Page
    {
        private string token;
        private readonly ContentService _contentService= new();
        private string imagePath;
        private string audioPath;
        public GUI_ArtistAddSong()
        {
            token = SessionManager.Instance.AccessToken;
            InitializeComponent();
        }


        private void BtnAddSongCover_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "Image Files|*.jpg;*.jpeg;*.png",
                    Title = "Selecciona una imagen"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    imagePath = openFileDialog.FileName;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al subir imagen: {ex.Message}");
            }

            MessageBox.Show($"Imagen cargada correctamente.");
        }

        private void BtnAddAudioFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "Audio Files|*.mp3;*.wav|All Files|*.*",
                    Title = "Selecciona el archivo de audio"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    audioPath = openFileDialog.FileName;
                    txtImage.Text = System.IO.Path.GetFileName(audioPath);
                    MessageBox.Show($"Archivo de audio cargado correctamente: {System.IO.Path.GetFileName(audioPath)}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el audio: {ex.Message}");
            }
        }

        private void BtnAddSaveSong_Click(object sender, RoutedEventArgs e)
        {
            _ = SaveSongAsync();
        }

        private async Task SaveSongAsync()
        {
            try
            {
                // Validaciones de la UI
                if (string.IsNullOrWhiteSpace(txtBoxSongName.Text))
                {
                    MessageBox.Show("Debe ingresar un nombre para la canción", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(audioPath))
                {
                    MessageBox.Show("Debe seleccionar un archivo de audio", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Mouse.OverrideCursor = Cursors.Wait;
                btnAddSavedSong.IsEnabled = false;
                btnAddSavedSong.Content = "Subiendo...";

                string songName = txtBoxSongName.Text.Trim();
                var result = await _contentService.UploadSongAsync(token, songName, audioPath, imagePath);

                if (result.Status == "success")
                {
                    MessageBox.Show("Canción subida exitosamente", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    txtBoxSongName.Text = string.Empty;
                    audioPath = string.Empty;
                    imagePath = string.Empty;
                    txtImage.Text = string.Empty;

                    NavigationService?.Navigate(new GUI_HomeArtist());
                }
                else
                {
                    MessageBox.Show($"Error al subir la canción: {result.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al subir la canción: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {

                Mouse.OverrideCursor = null;
                btnAddSavedSong.IsEnabled = true;
                btnAddSavedSong.Content = "Guardar Canción";
            }
        }


        private void BtnCancelUploadSong_Click(object sender, RoutedEventArgs e)
        {
            var confirmResult = MessageBox.Show(
                "¿Está seguro que desea cancelar la subida de la canción?\nLos cambios no guardados se perderán.",
                "Confirmar cancelación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmResult == MessageBoxResult.Yes)
            {
                txtBoxSongName.Text = string.Empty;
                audioPath = string.Empty;
                imagePath = string.Empty;

                if (NavigationService != null)
                {
                    NavigationService.Navigate(new GUI_HomeArtist());
                }
            }
        }

    }
}
