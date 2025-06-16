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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Net.Http;

namespace DAZTLClient.Windows {
    /// <summary>
    /// Lógica de interacción para GUI_ArtistProfile.xaml
    /// </summary>
    public partial class GUI_ArtistProfile : Page
    {

        private string token;
        private readonly UserService _userService = new();
        private string imagePath;

        public GUI_ArtistProfile()
        {
            InitializeComponent();
            token = SessionManager.Instance.AccessToken;
            LoadArtistProfile();

        }
        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cerrar sesión...");
        }

        private void BtnGoToHome_Click_(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new GUI_HomeArtist());
            }
        }
        private void BtnSeeAlbumsArtist_Click_(object sender, RoutedEventArgs e)
        {
        }

        private void BtnEditUsername_Click(object sender, RoutedEventArgs e)
        {
            txtUsername.IsReadOnly = false;
            txtUsername.Focus();
            txtPassword.IsReadOnly = false;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            btnUploadPhoto.Visibility = Visibility.Visible;
        }

        private void BtnGotoAddSong_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnGotoAddAlbum_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSeeAlbumsArtist_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnGoToHome_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void LoadArtistProfile()
        {
            try
            {
                var artistProfile = await _userService.GetArtistProfileAsync(token);
                if (artistProfile != null)
                {
                    txtUsername.Text = artistProfile.Username;
                    txtBio.Text = artistProfile.Bio;
                    string imageUrl = artistProfile.ProfileImageUrl;
                    imageUrl = imageUrl.Replace("localhost", "10.0.2.2");
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(imageUrl);
                        bitmap.EndInit();
                        imgProfilePicture.Source = bitmap;
                    }
                    else
                    {

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el perfil de artista: {ex.Message}");
            }
        }

        private void BtnUploadPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Selecciona una imagen de perfil"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                imagePath = openFileDialog.FileName;
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath);
                bitmap.EndInit();
                imgProfilePicture.Source = bitmap;
            }
        }

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {

        }

        private async Task UploadProfilePicture(string imagePath)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var fileStream = new FileStream(imagePath, FileMode.Open);
                    var content = new MultipartFormDataContent();
                    content.Add(new StreamContent(fileStream), "profile_picture", System.IO.Path.GetFileName(imagePath));
                    var response = await httpClient.PostAsync("http://10.0.2.2:8000/api/profile/upload_picture/", content);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Foto de perfil actualizada");
                    }
                    else
                    {
                        MessageBox.Show("Error al actualizar imagen");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al subir la imagen: {ex.Message}");
            }
        }
    }
}




