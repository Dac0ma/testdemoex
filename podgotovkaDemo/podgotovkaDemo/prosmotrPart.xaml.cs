using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace podgotovkaDemo
{
    /// <summary>
    /// Логика взаимодействия для prosmotrPart.xaml
    /// </summary>
    public partial class prosmotrPart : Window
    {
        int emp1;
        public prosmotrPart(int emp)
        {
            InitializeComponent();
            emp1= emp;
            LoadPartners();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            UserWindow userWindow = new UserWindow(emp1);
            userWindow.Show();
            Close();
        }

        private void LoadPartners()
        {
            string connectionString = @"Data Source=DESKTOP-DC3OGQJ; Initial Catalog=MasterPoll;Integrated Security=True"; // Замените на вашу строку подключения к БД
            string query = "SELECT p.id, p.name, p.email, p.phone, p.address, p.rating, d.surname, d.name AS director_name, d.patronymic, pa.name as paname " +
                           "FROM partner p " +
                           "LEFT JOIN director d ON p.director_id = d.id " +
                           "JOIN partner_type pa on pa.id=p.partner_type_id";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        // Создаем StackPanel для одного партнера
                        StackPanel partnerPanel = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            Margin = new Thickness(10),
                            Background = new SolidColorBrush(Colors.White)
                        };

                        // Добавляем информацию о партнере
                        partnerPanel.Children.Add(new TextBlock
                        {
                            Text = $"{reader["paname"]}|{reader["name"]}",
                            FontSize = 16,
                            FontWeight = FontWeights.Bold
                        });

                        partnerPanel.Children.Add(new TextBlock
                        {
                            Text = $"Email: {reader["email"]}",
                            FontSize = 14
                        });

                        partnerPanel.Children.Add(new TextBlock
                        {
                            Text = $"Телефон: {reader["phone"]}",
                            FontSize = 14
                        });

                        partnerPanel.Children.Add(new TextBlock
                        {
                            Text = $"Адрес: {reader["address"]}",
                            FontSize = 14
                        });

                        partnerPanel.Children.Add(new TextBlock
                        {
                            Text = $"Рейтинг: {reader["rating"]}",
                            FontSize = 14
                        });

                        partnerPanel.Children.Add(new TextBlock
                        {
                            Text = $"Директор: {reader["surname"]} {reader["director_name"]} {reader["patronymic"]}",
                            FontSize = 14
                        });

                        // Создаем кнопку для редактирования
                        Button editButton = new Button
                        {
                            Content = "Редактировать",
                            Background = new SolidColorBrush(Colors.LightBlue),
                            Margin = new Thickness(0, 5, 0, 0)
                        };

                        // Передаем ID партнера через обработчик события
                        int partnerId = Convert.ToInt32(reader["id"]);
                        editButton.Click += (s, e) => EditPartner(partnerId);

                        partnerPanel.Children.Add(editButton);

                        // Добавляем StackPanel в основной ScrollViewer
                        partnerStackPanel.Children.Add(partnerPanel);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }


        private void EditPartner(int partnerId)
        {
            try
            {
                EditPartnerWindow editWindow = new EditPartnerWindow(partnerId, emp1); // Предполагаем, что у вас есть окно EditPartnerWindow
                editWindow.Show();
               Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна редактирования: {ex.Message}");
            }
        }


    }
}
