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
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public int emp;
        public UserWindow(int employeeId)
        {
            InitializeComponent();
            emp = employeeId;

        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void partner_Click(object sender, RoutedEventArgs e)
        {
            prosmotrPart prosmotr = new prosmotrPart(emp);
            prosmotr.Show();
            Close();
        }

        private void zayavka_Click(object sender, RoutedEventArgs e)
        {
            vsezayavki vsezayavki = new vsezayavki(emp);
            vsezayavki.Show();
            Close();
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            regpart regpart = new regpart(emp);
            regpart.Show();
            Close();
        }

        private void istor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void otchet_Click(object sender, RoutedEventArgs e)
        {
            otchet otchet = new otchet(emp);
            otchet.Show();
            Close();
        }
    }
}
