using DAZTLClient.Models;
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

namespace DAZTLClient.Windows.UserControllers
{
    /// <summary>
    /// Lógica de interacción para SingUp.xaml
    /// </summary>
    public partial class SingUp : UserControl
    {
        public event EventHandler LogInRequested;
        private readonly UserService _userService = new UserService();

        public SingUp()
        {
            InitializeComponent();

        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            LogInRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnHomeListener_Click(object sender, RoutedEventArgs e)
        {
            var homePage = new HomeListeners();

            NavigationService navigationService = NavigationService.GetNavigationService(this);

            if (navigationService != null)
            {
                navigationService.Navigate(homePage);
            }
            else
            {
                MessageBox.Show("No se encontró el servicio de navegación.");
            }
        }

        private async void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            var password = pssBoxPaswordOne.Password;
            var confirmPassword = pssBoxPaswordTwo.Password;

            if (password != confirmPassword)
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string selectedRole = null;
            if (rdBtnListener.IsChecked == true)
            {
                selectedRole = "listener";
            }
            else if (rdBtnArtist.IsChecked == true)
            {
                selectedRole = "artist";
            }

            if (string.IsNullOrEmpty(selectedRole))
            {
                MessageBox.Show("Por favor, selecciona un tipo de usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtBoxUsername.Text) ||
                string.IsNullOrWhiteSpace(txtBoxEmail.Text) ||
                string.IsNullOrWhiteSpace(txtBoxName.Text) ||
                string.IsNullOrWhiteSpace(txtBoxLastName.Text))
            {
                MessageBox.Show("Por favor, completa todos los campos obligatorios.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            

            try
            {
                if (rdBtnListener.IsChecked == true)
                {
                    var request = new RegisterRequest
                    {
                        Username = txtBoxUsername.Text.Trim(),
                        Email = txtBoxEmail.Text.Trim(),
                        Password = password,
                        FirstName = txtBoxName.Text.Trim(),
                        LastName = txtBoxLastName.Text.Trim(),
                        Role = selectedRole

                    };
                    var result = await _userService.RegisterAsync(request);
                    MessageBox.Show(result, "Registro", MessageBoxButton.OK, MessageBoxImage.Information);
                } else if (rdBtnArtist.IsChecked == true)
                {
                    var request = new RegisterArtistRequest
                    {
                        Username = txtBoxUsername.Text.Trim(),
                        Email = txtBoxEmail.Text.Trim(),
                        Password = password,
                        FirstName = txtBoxName.Text.Trim(),
                        LastName = txtBoxLastName.Text.Trim(),
                        Bio = "Nueva biografia"

                    };

                    var result = await _userService.RegisterArtistAsync(request);
                    MessageBox.Show(result, "Registro", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de conexion {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }

}