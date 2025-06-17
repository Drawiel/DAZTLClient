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

namespace DAZTLClient.Windows
{
    /// <summary>
    /// Interaction logic for GUI_HomeAdmin.xaml
    /// </summary>
    public partial class GUI_HomeAdmin : Page
    {
        private readonly UserService _userService = new();
        private readonly ContentService _contentService = new();
        private readonly ReportExportService _exportService = new();
        private Daztl.AdminReportResponse _currentReport;
        private string token;
        public GUI_HomeAdmin()
        {
            InitializeComponent();
            token = SessionManager.Instance.AccessToken;
        }

        private async void BtnUsersReport_Click(object sender, RoutedEventArgs e)
        {
            await LoadReport("users", "Usuarios");
        }
        private async void BtnArtistsReport_Click(object sender, RoutedEventArgs e)
        {
            await LoadReport("artists", "Artistas");
        }
        private async void BtnListenersReport_Click(object sender, RoutedEventArgs e)
        {
            await LoadReport("listeners", "Oyentes");
        }
        private async void BtnSongsReport_Click(object sender, RoutedEventArgs e)
        {
            await LoadReport("songs", "Canciones");
        }
        private async void BtnAlbumsReport_Click(object sender, RoutedEventArgs e)
        {
            await LoadReport("albums", "Álbumes");
        }
        private async Task LoadReport(string reportType, string reportName)
        {
            try
            {
                SetButtonsEnabled(false);

                _currentReport = await _contentService.GetAdminReportAsync(token, reportType);

                if (_currentReport.Status == "success")
                {
                    dataGridReportData.ItemsSource = _currentReport.Data;

                    // Configurar columnas específicas según el tipo de reporte
                    ConfigureDataGridColumns(reportType);

                    btnExport.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show($"Error al cargar reporte: {_currentReport.Message}");
                    btnExport.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                btnExport.IsEnabled = false;
            }
            finally
            {
                SetButtonsEnabled(true);
            }
        }

        private void ConfigureDataGridColumns(string reportType)
        {
            // Limpiar columnas existentes
            dataGridReportData.Columns.Clear();

            // Configurar columnas según el tipo de reporte
            switch (reportType.ToLower())
            {
                case "users":
                    ConfigureUsersColumns();
                    break;
                case "artists":
                    ConfigureArtistsColumns();
                    break;
                case "listeners":
                    ConfigureListenersColumns();
                    break;
                case "songs":
                    ConfigureSongsColumns();
                    break;
                case "albums":
                    ConfigureAlbumsColumns();
                    break;
            }
        }

        private void ConfigureUsersColumns()
        {
            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("Id"),
                Width = new DataGridLength(60)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Nombre de Usuario",
                Binding = new Binding("Username"),
                Width = new DataGridLength(150)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Email",
                Binding = new Binding("Email"),
                Width = new DataGridLength(200)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Nombre",
                Binding = new Binding("FirstName"),
                Width = new DataGridLength(120)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Apellido",
                Binding = new Binding("LastName"),
                Width = new DataGridLength(120)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Rol",
                Binding = new Binding("Role"),
                Width = new DataGridLength(80)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Fecha de Registro",
                Binding = new Binding("DateJoined"),
                Width = new DataGridLength(150)
            });
        }

        private void ConfigureArtistsColumns()
        {
            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("Id"),
                Width = new DataGridLength(60)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Nombre de Usuario",
                Binding = new Binding("Username"),
                Width = new DataGridLength(150)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Email",
                Binding = new Binding("Email"),
                Width = new DataGridLength(200)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Nombre",
                Binding = new Binding("FirstName"),
                Width = new DataGridLength(120)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Apellido",
                Binding = new Binding("LastName"),
                Width = new DataGridLength(120)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Fecha de Registro",
                Binding = new Binding("DateJoined"),
                Width = new DataGridLength(150)
            });
        }

        private void ConfigureListenersColumns()
        {
            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("Id"),
                Width = new DataGridLength(60)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Nombre de Usuario",
                Binding = new Binding("Username"),
                Width = new DataGridLength(150)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Email",
                Binding = new Binding("Email"),
                Width = new DataGridLength(200)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Nombre",
                Binding = new Binding("FirstName"),
                Width = new DataGridLength(120)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Apellido",
                Binding = new Binding("LastName"),
                Width = new DataGridLength(120)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Fecha de Registro",
                Binding = new Binding("DateJoined"),
                Width = new DataGridLength(150)
            });
        }

        private void ConfigureSongsColumns()
        {
            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("Id"),
                Width = new DataGridLength(60)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Título",
                Binding = new Binding("Title"),
                Width = new DataGridLength(250)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Fecha de Lanzamiento",
                Binding = new Binding("ReleaseDate"),
                Width = new DataGridLength(150)
            });
        }

        private void ConfigureAlbumsColumns()
        {
            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("Id"),
                Width = new DataGridLength(60)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Título",
                Binding = new Binding("Title"),
                Width = new DataGridLength(300)
            });
        }


        private void BtnExportCSV_Click(object sender, RoutedEventArgs e)
        {
            if (_currentReport != null && _currentReport.Data.Count > 0)
            {
                _exportService.ExportToCSV(_currentReport);
            }
            else
            {
                MessageBox.Show("No hay datos para exportar");
            }
        }
        private void BtnCloseSession_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new GUI_LogIn());
                SessionManager.Instance.EndSession();
                MusicPlayerService.Instance.Stop();
            }
        }

        private void SetButtonsEnabled(bool enabled)
        {
            btnUsersReport.IsEnabled = enabled;
            btnArtistsReport.IsEnabled = enabled;
            btnListenersReport.IsEnabled = enabled;
            btnSongsReport.IsEnabled = enabled;
            btnAlbumsReport.IsEnabled = enabled;
        }
    }
}
