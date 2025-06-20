using DAZTLClient.Services;
using DAZTLClient.Windows.UserControllers;
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

namespace DAZTLClient.Windows {
    /// <summary>
    /// Lógica de interacción para GUI_ListenersProfile.xaml
    /// </summary>
    public partial class GUI_ListenersProfile : Page {
        private List<Notification> notifications = new List<Notification>();
        private UserService _userService = new();
        private string token;
        private string imagePath;

        private string currentFilesURL = "http://localhost:8000/media/";

        public GUI_ListenersProfile() {
            InitializeComponent();
            token = SessionManager.Instance.AccessToken;
            SimulateNotifications();
            LoadListenerProfile();

        }
        private void AccountButton_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            if(button?.ContextMenu != null) {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void BtnGoToHome_Click_(object sender, RoutedEventArgs e) {
            if(this.NavigationService != null) {
                NavigationService.Navigate(new HomeListeners());
            }
        }
        private void SimulateNotifications() {
            for(int i = 1; i <= 10; i++) {
                notifications.Add(new Notification {
                    Title = $"Notificación {i}",

                });
            }

            LoadNotifications();
        }

        private void LoadNotifications() {
            NotificationList.Children.Clear();

            foreach(var notification in notifications) {
                var btn = new Button {
                    Width = 380,
                    Height = 70,
                    Margin = new Thickness(0, 5, 0, 5),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Content = notification.Title,
                    Background = (Brush)new BrushConverter().ConvertFromString("#202123"),
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Tag = notification 
                };

                btn.Click += (s, e) => {
                    var noti = (Notification)((Button)s).Tag;
                    MessageBox.Show($"Navegar a: {noti.Title}");
                    LoadNotifications(); 
                };

                NotificationList.Children.Add(btn);
            }
        }

        private void NotificationButton_Click(object sender, RoutedEventArgs e) {
            NotificationPopup.IsOpen = true;


            LoadNotifications();
        }


        public class Notification {
            public string Title { get; set; }
        }


        private void BtnGoToCreatePlaylist_Click(object sender, RoutedEventArgs e) {

        }
        private void BtnEditUsername_Click(object sender, RoutedEventArgs e) {
            txtUsername.IsReadOnly = false;
            txtUsername.Focus();
            txtPassword.IsReadOnly = false;
            txtEmail.IsReadOnly = false;
            txtFirstName.IsReadOnly = false;
            txtLastName.IsReadOnly = false;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            btnUploadPhoto.Visibility = Visibility.Visible;
        }

        private void BtnCloseSession_Click(object sender, RoutedEventArgs e) { 
        
        }

        private async void LoadListenerProfile()
        {
            try
            {
                var profile = await _userService.GetListenerProfileAsync(token);
                if (profile != null)
                {
                    txtUsername.Text = profile.Username;
                    txtEmail.Text = profile.Email;
                    txtFirstName.Text = profile.FirstName;
                    txtLastName.Text = profile.LastName;

                    string imageUrl = currentFilesURL + profile.ProfileImageUrl;
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
                        LoadListenerProfile();
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

        private void BtnCancelChanges_Click(object sender, RoutedEventArgs e)
        {
            btnSave.Visibility = Visibility.Collapsed;
            btnCancel.Visibility = Visibility.Collapsed;
            btnUploadPhoto.Visibility = Visibility.Collapsed;
            txtPassword.Clear();
            LoadListenerProfile();
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtUsername.Text) && string.IsNullOrEmpty(txtEmail.Text) && string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("No pueden existir campos vacios a excepcion de la contraseña");
                return false;
            }

            if (!Utils.FielValidator.IsValidPassword(txtPassword.Text))
            {
                MessageBox.Show("El formato de la contraseña es incorrecto");
                return false;
            }

            return true;
        }

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                if (!ValidateFields()) { return; }
            }
            else if (!Utils.FielValidator.IsValidPassword(txtPassword.Text))
            {
                if (!ValidateFields()) { return; }
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
                string email = !string.IsNullOrWhiteSpace(txtEmail.Text) ? txtEmail.Text : null;
                string firstName = !string.IsNullOrWhiteSpace(txtFirstName.Text) ? txtFirstName.Text : null;
                string lastName = !string.IsNullOrWhiteSpace(txtLastName.Text) ? txtLastName.Text : null;

                var result = await _userService.UpdateProfileAsync(
                    token,
                    email,
                    firstName, 
                    lastName,
                    username,
                    password
                );

                if (result.Contains("correctamente"))
                {
                    txtUsername.IsReadOnly = true;
                    txtPassword.IsReadOnly = true;
                    txtEmail.IsReadOnly = true;
                    txtFirstName.IsReadOnly = true;
                    txtLastName.IsReadOnly = true;

                    txtPassword.Clear();

                    btnSave.Visibility = Visibility.Collapsed;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnUploadPhoto.Visibility = Visibility.Collapsed;
                    MessageBox.Show(result);

                    LoadListenerProfile();
                }
                else if (result.Contains("error"))
                {
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
    }
}



