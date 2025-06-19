using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
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



namespace podgotovkaDemo
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }
        //окно менеджера,
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string password = Password.Password;

            using (SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-DC3OGQJ; Initial Catalog=MasterPoll;Integrated Security=True"))
            {
                try
                {
                    connection.Open();

                    // Проверяем логин и пароль для пользователя
                    string query = "SELECT users.employee_id AS EmployeeId, employee.name AS EmployeeName,  posts.name AS PostName,  users.id AS UserId FROM users JOIN employee ON employee.id = users.employee_id join posts on posts.id=employee.post_id WHERE login = @Login AND password = @Password";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.Parameters.AddWithValue("@Password", password);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count == 1)
                    {
                        // Логика для пользователей
                        int employeeId = Convert.ToInt32(dt.Rows[0]["EmployeeId"]); // Используем алиас "EmployeeId"
                        string employeeName = Convert.ToString(dt.Rows[0]["EmployeeName"]); // Используем алиас "EmployeeName"
                        string rol = Convert.ToString(dt.Rows[0]["PostName"]);
                        ShowGreeting(employeeName,rol);
                        var userWindow = new UserWindow(employeeId);
                        userWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        // Проверяем логин и пароль для партнера
                        string query1 = "SELECT partner.id,director.name FROM partner join director on director.id=partner.director_id WHERE loginpart = @Login AND passwordpart = @Password";
                        SqlCommand cmd1 = new SqlCommand(query1, connection);
                        cmd1.Parameters.AddWithValue("@Login", login);
                        cmd1.Parameters.AddWithValue("@Password", password);

                        SqlDataAdapter partnerAdapter = new SqlDataAdapter(cmd1);
                        DataTable dtPartner = new DataTable();
                        partnerAdapter.Fill(dtPartner);

                        if (dtPartner.Rows.Count == 1)
                        {
                            int partnerId = Convert.ToInt32(dtPartner.Rows[0]["id"]);
                            string name = Convert.ToString(dtPartner.Rows[0]["name"]);
                            string rol = "Партнер";
                            ShowGreeting(name,rol);
                            var partnerWindow = new PartnerWindow(partnerId);
                            partnerWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Неправильный логин или пароль");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }
        }

        private void ShowGreeting(string name,string rol)
        {
            DateTime now = DateTime.Now;
            if (now.Hour >= 6 && now.Hour < 12)
            {
                MessageBox.Show($"Доброе утро, {rol} {name}!");
            }
            else if (now.Hour >= 12 && now.Hour < 18)
            {
                MessageBox.Show($"Добрый день, {rol} {name}!");
            }
            else
            {
                MessageBox.Show($"Добрый вечер, {rol} {name}!");
            }
        }


        private void reg_Click(object sender, RoutedEventArgs e)
        {
            regwin regwin= new regwin();
            regwin.Show();
            Close();
        }
    }
}
 