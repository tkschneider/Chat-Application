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

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for IPWindow.xaml
    /// </summary>
    public partial class IPWindow : Window
    {

        public IPWindow()
        {
            InitializeComponent();
        }

        private void IPButtonClicked(object sender, RoutedEventArgs e)
        {
            String input = IPBox.Text;
            Application.Current.Properties["IPAddress"] = input;
            this.Close();
        }

        private void OnIPWindowClose(object sender, EventArgs e)
        {
            Application.Current.Properties["IPWindowIsClosed"] = true;
        }
        
    }
}
