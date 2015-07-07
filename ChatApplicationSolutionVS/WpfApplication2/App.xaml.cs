using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        private void onStartup(object sender, StartupEventArgs e)
        {
            Application.Current.Properties["IsServer"] = false;
            Application.Current.Properties["IPAddress"] = "none";
            Application.Current.Properties["IPWindowIsClosed"] = false;
        }
    }
}
