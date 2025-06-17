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

namespace DAZTLClient.Windows {
    /// <summary>
    /// Lógica de interacción para GUI_HomeArtist.xaml
    /// </summary>
    public partial class GUI_HomeArtist : Page {
        private readonly ContentService _contentService = new();
        private readonly ReportExportService _exportService = new();
        private Daztl.ArtistReportResponse _currentReport;
        private string token;
        public GUI_HomeArtist() {
            InitializeComponent();
            token = SessionManager.Instance.AccessToken;
        }

        private void BtnGoToProfile_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new GUI_ArtistProfile());
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

        private async void BtnSongsReport_Click(object sender, RoutedEventArgs e)
        {
            await LoadReport("songs", "Mis Canciones");
        }

        private async void BtnAlbumsReport_Click(object sender, RoutedEventArgs e)
        {
            await LoadReport("albums", "Mis Álbumes");
        }

        private async Task LoadReport(string reportType, string reportName)
        {
            try
            {
                SetButtonsEnabled(false);
                txtReportInfo.Text = $"Cargando reporte de {reportName}...";
                _currentReport = await _contentService.GetArtistReportAsync(token, reportType);
                if (_currentReport.Status == "success")
                {
                    dataGridReportData.ItemsSource = _currentReport.Data;
                    ConfigureDataGridColumns(reportType);
                    txtReportInfo.Text = $"{reportName} - Total: {_currentReport.TotalCount} registros";
                    btnExport.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show($"Error al cargar reporte: {_currentReport.Message}");
                    txtReportInfo.Text = "Error al cargar reporte";
                    btnExport.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                txtReportInfo.Text = "Error al cargar reporte";
                btnExport.IsEnabled = false;
            }
            finally
            {
                SetButtonsEnabled(true);
            }
        }

        private void SetButtonsEnabled(bool enabled)
        {
            btnSongsReport.IsEnabled = enabled;
            btnAlbumsReport.IsEnabled = enabled;
        }

        private void ConfigureDataGridColumns(string reportType)
        {
            dataGridReportData.Columns.Clear();

            switch (reportType.ToLower())
            {
                case "songs":
                    ConfigureSongsColumns();
                    break;
                case "albums":
                    ConfigureAlbumsColumns();
                    break;
            }
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
                Width = new DataGridLength(300)
            });

            dataGridReportData.Columns.Add(new DataGridTextColumn
            {
                Header = "Fecha de Lanzamiento",
                Binding = new Binding("ReleaseDate"),
                Width = new DataGridLength(180)
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
                Width = new DataGridLength(400)
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

        private void BtnGoToAddSong_Click(object sender, RoutedEventArgs e)
        {

            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new GUI_ArtistAddSong());
            }
        }

    }
}
