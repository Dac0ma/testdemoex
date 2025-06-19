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

namespace podgotovkaDemo
{
    /// <summary>
    /// Логика взаимодействия для PartnerWindow.xaml
    /// </summary>
    public partial class PartnerWindow : Window
    {
        int partid;
        public PartnerWindow(int partnerId)
        {
            InitializeComponent();

            partid= partnerId;
        }

        private void partner_Click(object sender, RoutedEventArgs e)
        {
            createzayavki createzayavki = new createzayavki(partid);
            createzayavki.Show();
            Close();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();   
        }

        private void zayvki_Click(object sender, RoutedEventArgs e)
        {
            moizayavki moizayavki = new moizayavki(partid);
            moizayavki.Show();
            Close();
        }
    }
}
