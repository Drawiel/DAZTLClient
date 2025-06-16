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
using System.Windows.Shapes;

namespace DAZTLClient.Windows
{
    /// <summary>
    /// Interaction logic for CreatePlaylistWindow.xaml
    /// </summary>
    public partial class CreatePlaylistWindow : Window
    {
        public CreatePlaylistWindow()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
