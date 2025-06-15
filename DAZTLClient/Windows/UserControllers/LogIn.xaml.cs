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
    /// Lógica de interacción para LogIn.xaml
    /// </summary>
    public partial class LogIn : UserControl {
        public event EventHandler SignUpRequested;
        private readonly UserService _userService = new UserService();

        public LogIn() {
            InitializeComponent();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e) {
            SignUpRequested?.Invoke(this, EventArgs.Empty);
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
               

            try
            {
                var username = txtBoxUsername.Text.Trim();
                var password = pssBoxPassword.Password;
                var result = await _userService.LoginAsync(username, password);
                MessageBox.Show(result, "Inicio de Sesion", MessageBoxButton.OK, MessageBoxImage.Information);

                if (result.Contains("exisoto"))
                {
                    //Manejar situacion de exito
                }
            }
            catch (Exception ex) { 
                MessageBox.Show($"Error de conexion {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}