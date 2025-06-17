using System;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace DAZTLClient.Services
{
    public class ReportExportService
    {
        public bool ExportToCSV(Daztl.AdminReportResponse reportData, string defaultFileName = null)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    Title = "Guardar reporte como CSV",
                    FileName = defaultFileName ?? $"Reporte_{reportData.ReportType}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    string csvContent = GenerateCSVContent(reportData);
                    File.WriteAllText(filePath, csvContent, Encoding.UTF8);

                    MessageBox.Show($"Reporte exportado exitosamente a:\n{filePath}",
                                  "Exportación Exitosa",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar reporte: {ex.Message}",
                              "Error de Exportación",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                return false;
            }
        }

        private string GenerateCSVContent(Daztl.AdminReportResponse reportData)
        {
            StringBuilder csv = new StringBuilder();

            csv.AppendLine($"# Reporte de {GetReportTitle(reportData.ReportType)}");
            csv.AppendLine($"# Generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            csv.AppendLine($"# Total de registros: {reportData.TotalCount}");
            csv.AppendLine();

            switch (reportData.ReportType.ToLower())
            {
                case "users":
                    return GenerateUsersCSV(reportData, csv);
                case "artists":
                    return GenerateArtistsCSV(reportData, csv);
                case "listeners":
                    return GenerateListenersCSV(reportData, csv);
                case "songs":
                    return GenerateSongsCSV(reportData, csv);
                case "albums":
                    return GenerateAlbumsCSV(reportData, csv);
                default:
                    return GenerateGenericCSV(reportData, csv);
            }
        }

        private string GenerateUsersCSV(Daztl.AdminReportResponse reportData, StringBuilder csv)
        {
            csv.AppendLine("ID,Nombre de Usuario,Email,Nombre,Apellido,Rol,Fecha de Registro");

            foreach (var item in reportData.Data)
            {
                csv.AppendLine($"{item.Id}," +
                              $"\"{EscapeCSV(item.Username)}\"," +
                              $"\"{EscapeCSV(item.Email)}\"," +
                              $"\"{EscapeCSV(item.FirstName)}\"," +
                              $"\"{EscapeCSV(item.LastName)}\"," +
                              $"\"{EscapeCSV(item.Role)}\"," +
                              $"\"{FormatDate(item.DateJoined)}\"");
            }

            return csv.ToString();
        }

        private string GenerateArtistsCSV(Daztl.AdminReportResponse reportData, StringBuilder csv)
        {
            csv.AppendLine("ID,Nombre de Usuario,Email,Nombre,Apellido,Fecha de Registro");

            foreach (var item in reportData.Data)
            {
                csv.AppendLine($"{item.Id}," +
                              $"\"{EscapeCSV(item.Username)}\"," +
                              $"\"{EscapeCSV(item.Email)}\"," +
                              $"\"{EscapeCSV(item.FirstName)}\"," +
                              $"\"{EscapeCSV(item.LastName)}\"," +
                              $"\"{FormatDate(item.DateJoined)}\"");
            }

            return csv.ToString();
        }

        private string GenerateListenersCSV(Daztl.AdminReportResponse reportData, StringBuilder csv)
        {
            csv.AppendLine("ID,Nombre de Usuario,Email,Nombre,Apellido,Fecha de Registro");

            foreach (var item in reportData.Data)
            {
                csv.AppendLine($"{item.Id}," +
                              $"\"{EscapeCSV(item.Username)}\"," +
                              $"\"{EscapeCSV(item.Email)}\"," +
                              $"\"{EscapeCSV(item.FirstName)}\"," +
                              $"\"{EscapeCSV(item.LastName)}\"," +
                              $"\"{FormatDate(item.DateJoined)}\"");
            }

            return csv.ToString();
        }

        private string GenerateSongsCSV(Daztl.AdminReportResponse reportData, StringBuilder csv)
        {
            csv.AppendLine("ID,Título,Fecha de Lanzamiento");

            foreach (var item in reportData.Data)
            {
                csv.AppendLine($"{item.Id}," +
                              $"\"{EscapeCSV(item.Title)}\"," +
                              $"\"{FormatDate(item.ReleaseDate)}\"");
            }

            return csv.ToString();
        }

        private string GenerateAlbumsCSV(Daztl.AdminReportResponse reportData, StringBuilder csv)
        {
            csv.AppendLine("ID,Título");

            foreach (var item in reportData.Data)
            {
                csv.AppendLine($"{item.Id}," +
                              $"\"{EscapeCSV(item.Title)}\"");
            }

            return csv.ToString();
        }

        private string GenerateGenericCSV(Daztl.AdminReportResponse reportData, StringBuilder csv)
        {
            csv.AppendLine("ID,Información");

            foreach (var item in reportData.Data)
            {
                csv.AppendLine($"{item.Id},\"{EscapeCSV(item.Username ?? item.Title)}\"");
            }

            return csv.ToString();
        }

        private string EscapeCSV(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            return value.Replace("\"", "\"\"");
        }

        private string FormatDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return "";

            if (DateTime.TryParse(dateString, out DateTime date))
            {
                return date.ToString("dd/MM/yyyy HH:mm");
            }

            return dateString;
        }

        private string GetReportTitle(string reportType)
        {
            return reportType.ToLower() switch
            {
                "users" => "Usuarios",
                "artists" => "Artistas",
                "listeners" => "Oyentes",
                "songs" => "Canciones",
                "albums" => "Álbumes",
                _ => "Datos"
            };
        }

        public bool ExportToCSV(Daztl.ArtistReportResponse reportData, string defaultFileName = null)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    Title = "Guardar reporte como CSV",
                    FileName = defaultFileName ?? $"Reporte_{reportData.ReportType}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    string csvContent = GenerateArtistCSVContent(reportData);
                    File.WriteAllText(filePath, csvContent, Encoding.UTF8);

                    MessageBox.Show($"Reporte exportado exitosamente a:\n{filePath}",
                                  "Exportación Exitosa",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar reporte: {ex.Message}",
                              "Error de Exportación",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                return false;
            }
        }

        private string GenerateArtistCSVContent(Daztl.ArtistReportResponse reportData)
        {
            StringBuilder csv = new StringBuilder();

            csv.AppendLine($"# Reporte de {GetArtistReportTitle(reportData.ReportType)}");
            csv.AppendLine($"# Generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            csv.AppendLine($"# Total de registros: {reportData.TotalCount}");
            csv.AppendLine();

            switch (reportData.ReportType.ToLower())
            {
                case "artist_songs":
                    return GenerateArtistSongsCSV(reportData, csv);
                case "artist_albums":
                    return GenerateArtistAlbumsCSV(reportData, csv);
                default:
                    return csv.ToString();
            }
        }

        private string GenerateArtistSongsCSV(Daztl.ArtistReportResponse reportData, StringBuilder csv)
        {
            csv.AppendLine("ID,Título,Fecha de Lanzamiento");

            foreach (var item in reportData.Data)
            {
                csv.AppendLine($"{item.Id}," +
                              $"\"{EscapeCSV(item.Title)}\"," +
                              $"\"{FormatDate(item.ReleaseDate)}\"");
            }

            return csv.ToString();
        }

        private string GenerateArtistAlbumsCSV(Daztl.ArtistReportResponse reportData, StringBuilder csv)
        {
            csv.AppendLine("ID,Título");

            foreach (var item in reportData.Data)
            {
                csv.AppendLine($"{item.Id}," +
                              $"\"{EscapeCSV(item.Title)}\"");
            }

            return csv.ToString();
        }

        private string GetArtistReportTitle(string reportType)
        {
            return reportType.ToLower() switch
            {
                "artist_songs" => "Mis Canciones",
                "artist_albums" => "Mis Álbumes",
                _ => "Datos"
            };
        }

    }
}
