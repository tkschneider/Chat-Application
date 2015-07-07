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
    /// 
    /// Interaction logic for ClientServerDialog.xaml
    /// 
    public partial class ClientServerDialog : Window
    {
        

        public ClientServerDialog()
        {
            InitializeComponent();
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            
            
            if (ServerButton.IsChecked == true)
            {
                Application.Current.Properties["IsServer"] = true;
                this.Close();
            }

            if (ClientButton.IsChecked == true)
            {
                Application.Current.Properties["IsServer"] = false;
                this.Close();
            }
            
            if(ClientButton.IsChecked == false && ServerButton.IsChecked == false)
            {
                MessageBox.Show("No selection made!");
                return;
            }
            
        }

        private void ShowIPWindow(object sender, EventArgs e)
        {
            if (ClientButton.IsChecked == true)
            {
                IPWindow window = new IPWindow();
                window.Show();

            }
        }

    }
}
