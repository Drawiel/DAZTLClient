using Daztl;
using DAZTLClient.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static DAZTLClient.Windows.GUI_ArtistAddAlbum;

namespace DAZTLClient.Windows
{
    /// <summary>
    /// Lógica de interacción para GUI_ArtistAddAlbum.xaml
    /// </summary>
    public partial class GUI_ArtistAddAlbum : Page
    {
        private string token;
        private readonly ContentService _contentService = new();
        private ObservableCollection<SongFile> songFiles;
        private string albumCoverPath;
        public GUI_ArtistAddAlbum()
        {
            token = SessionManager.Instance.AccessToken;
            InitializeComponent();
            InitializeDataGrid();
        }

        public class SongFile
        {
            public int Number { get; set; }
            public string Url { get; set; }
            public string FullPath { get; set; }
            public string FileName { get; set; }
        }

        private void InitializeDataGrid()
        {
            songFiles = new ObservableCollection<SongFile>();
            FilesTable.ItemsSource = songFiles;
        }

        private void TxtBoxAlbumTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            string text = textBox.Text;
            if (!Regex.IsMatch(text, @"^[a-zA-Z0-9\s\-_]*$"))
            {
                MessageBox.Show("El título no puede contener caracteres especiales.", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                textBox.Text = Regex.Replace(text, @"[^a-zA-Z0-9\s\-_]", "");
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private void BtnAddAlbum_Click(object sender, RoutedEventArgs e)
        {
            _ = AddAlbumAsync();
        }

        private async Task AddAlbumAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtBoxTitle.Text))
                {
                    MessageBox.Show("Debe ingresar un título para el álbum.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (songFiles.Count == 0)
                {
                    MessageBox.Show("Debe agregar al menos una canción al álbum.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                Mouse.OverrideCursor = Cursors.Wait;
                btnAddAlbum.IsEnabled = false;
                btnAddAlbum.Content = "Subiendo...";
                string albumTitle = txtBoxTitle.Text.Trim();

                var songPaths = songFiles.Select(sf => sf.FullPath).ToList();
                var songTitles = songFiles.Select(sf => sf.FileName).ToList();

                if (!File.Exists(albumCoverPath))
                {
                    MessageBox.Show("La portada del álbum no existe", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = await _contentService.UploadAlbumAsync(
                    token,
                    albumTitle,
                    albumCoverPath,
                    songPaths
                );
                if (result.Status == "success")
                {
                    MessageBox.Show("Álbum subido exitosamente", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    ClearForm();
                    NavigationService?.Navigate(new GUI_HomeArtist());
                }
                else
                {
                    MessageBox.Show($"Error al subir el álbum: {result.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al subir el álbum: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
                btnAddAlbum.IsEnabled = true;
                btnAddAlbum.Content = "Agregar";
            }
        }

        private void BtnCancelAddAlbum_Click(object sender, RoutedEventArgs e)
        {
            var confirmResult = MessageBox.Show(
                "¿Está seguro que desea cancelar la creación del álbum?\nLos cambios no guardados se perderán.",
                "Confirmar cancelación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (confirmResult == MessageBoxResult.Yes)
            {
                ClearForm();
                NavigationService?.Navigate(new GUI_HomeArtist());
            }
        }

        private void ClearForm()
        {
            txtBoxTitle.Text = string.Empty;
            txtSongUri.Text = string.Empty;
            albumCoverPath = string.Empty;
            songFiles.Clear();
            imgAlbumCover.Source = new BitmapImage(new Uri("/Multimedia/default_cover.png", UriKind.Relative));
        }


        private void BtnAddSongFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "Audio Files|*.mp3",
                    Title = "Selecciona archivos de audio",
                    Multiselect = true
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (string filePath in openFileDialog.FileNames)
                    {
                        if (!songFiles.Any(sf => sf.FullPath == filePath))
                        {
                            var songFile = new SongFile
                            {
                                Number = songFiles.Count + 1,
                                Url = System.IO.Path.GetFileName(filePath),
                                FullPath = filePath,
                                FileName = System.IO.Path.GetFileNameWithoutExtension(filePath)
                            };
                            songFiles.Add(songFile);
                        }
                    }
                    MessageBox.Show($"{openFileDialog.FileNames.Length} archivo(s) agregado(s).", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar archivos: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGoToProfile_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new GUI_ArtistProfile());
            }
        }

        private void BtnGoToAddSong_Click(object sender, RoutedEventArgs e)
        {

            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new GUI_ArtistAddSong());
            }
        }

        private void BtnAddImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                    Title = "Selecciona la portada del álbum"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    albumCoverPath = openFileDialog.FileName;

                    // Cargar y mostrar la imagen seleccionada
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(albumCoverPath);
                    bitmap.EndInit();
                    imgAlbumCover.Source = bitmap;

                    MessageBox.Show("Portada cargada correctamente", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar imagen: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}