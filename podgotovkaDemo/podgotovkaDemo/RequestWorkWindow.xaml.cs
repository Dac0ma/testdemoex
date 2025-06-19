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
    /// Логика взаимодействия для RequestWorkWindow.xaml
    /// </summary>
    public partial class RequestWorkWindow : Window
    {
        int emp;
        public RequestWorkWindow(int requestId, int emp1)
        {
            InitializeComponent();
            emp=emp1;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            vsezayavki vsezayavki=new vsezayavki(emp);
            vsezayavki.Show();
            Close();
        }
    }
}
