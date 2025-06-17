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
        private string actualUsername;
        private string actualBio;
        private string actualImageUrl;

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
            txtBio.IsReadOnly = false;
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
                    actualUsername = artistProfile.Username;
                    txtBio.Text = artistProfile.Bio;
                    actualBio = artistProfile.Bio;
                    string imageUrl = artistProfile.ProfileImageUrl;
                    actualImageUrl = artistProfile.ProfileImageUrl;
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(imageUrl);
                        bitmap.EndInit();
                        imgProfilePicture.Source = bitmap;
                    }
                    
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el perfil de artista: {ex.Message}");
            }
        }

        private async void BtnUploadPhoto_Click(object sender, RoutedEventArgs e)
        {
            try
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

                    btnUploadPhoto.IsEnabled = false;
                    btnUploadPhoto.Content = "Subiendo...";

                    string result = await _userService.UploadProfileImageAsync(token, imagePath);
                    MessageBox.Show(result);

                    if (result.Contains("Imagen actualizada") || result.Contains("success"))
                    {
                        LoadArtistProfile();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al subir imagen: {ex.Message}");
            }
            finally
            {
                btnUploadPhoto.IsEnabled = true;
                btnUploadPhoto.Content = "Subir Foto";
            }

        }
        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                if(!ValidateFields()){ return ; }
            }
            _ = SaveChanges();
        }

        private async Task SaveChanges()
        {
            try
            {
                btnSave.IsEnabled = false;
                btnSave.Content = "Guardando...";

                string username = !string.IsNullOrWhiteSpace(txtUsername.Text) ? txtUsername.Text : null;
                string password = !string.IsNullOrWhiteSpace(txtPassword.Text) ? txtPassword.Text : null;
                string bio = !string.IsNullOrWhiteSpace(txtBio.Text) ? txtBio.Text : null;

                var result = await _userService.UpdateArtistProfileAsync(
                    token, 
                    username, 
                    password,
                    bio
                );

                if (result.Contains("correctamente"))
                {
                    txtUsername.IsReadOnly = true;
                    txtPassword.IsReadOnly = true;
                    txtBio.IsReadOnly = true;

                    txtPassword.Clear();

                    btnSave.Visibility = Visibility.Collapsed;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnUploadPhoto.Visibility = Visibility.Collapsed;
                    MessageBox.Show(result);

                    LoadArtistProfile();
                } else if (result.Contains("error")){
                    MessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                btnSave.IsEnabled = true;
                btnSave.Content = "Guardar";
            }


        }

        private bool ValidateFields()
        {
            if(string.IsNullOrEmpty(txtUsername.Text)) 
            {
                MessageBox.Show("El nombre del perfil no puede ser vacio");
                return false; 
            }

            if (Utils.FielValidator.IsValidPassword(txtPassword.Text))
            {
                MessageBox.Show("El formato de la contraseña es incorrecto");
                return false;
            }

            return true;
        }
        private void BtnSeeAlbumsArtist_Click(object sender, RoutedEventArgs e) {

        }
    }
}




