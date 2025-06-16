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
using static DAZTLClient.Windows.GUI_ArtistAddAlbum;

namespace DAZTLClient.Windows {
    /// <summary>
    /// Lógica de interacción para GUI_ArtistAddAlbum.xaml
    /// </summary>
    public partial class GUI_ArtistAddAlbum : Page {
        private List<Archivo> archivos = new List<Archivo>();
        public GUI_ArtistAddAlbum() {
            InitializeComponent();
            // Agregar datos de prueba
            archivos.Add(new Archivo { Number = 1, Url = @"C:\Archivos\Cancion1.mp3" });
            archivos.Add(new Archivo { Number = 2, Url = @"C:\Archivos\Cancion2.mp3" });
            archivos.Add(new Archivo { Number = 3, Url = @"C:\Archivos\Cancion3.mp3" });
            archivos.Add(new Archivo { Number = 4, Url = @"C:\Archivos\Cancion3.mp3" });
            archivos.Add(new Archivo { Number = 5, Url = @"C:\Archivos\Cancion3.mp3" });
            archivos.Add(new Archivo { Number = 6, Url = @"C:\Archivos\Cancion3.mp3" });
            archivos.Add(new Archivo { Number = 7, Url = @"C:\Archivos\Cancion3.mp3" });
            archivos.Add(new Archivo { Number = 8, Url = @"C:\Archivos\Cancion3.mp3" });

            // Mostrar en la tabla
            FilesTable.ItemsSource = archivos;
        }
        public class Archivo {
            public int Number { get; set; }
            public string Url { get; set; }
        }

    }
}
