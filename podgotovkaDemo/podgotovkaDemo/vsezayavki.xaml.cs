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
    /// Логика взаимодействия для vsezayavki.xaml
    /// </summary>
    public partial class vsezayavki : Window
    {
        int emp1;
        public vsezayavki(int emp)
        {
            emp1= emp;
            InitializeComponent();
            LoadAllPartnerRequests();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            UserWindow userWindow=new UserWindow(emp1);
            userWindow.Show();
            Close();
        }

        private void LoadAllPartnerRequests()
        {
            string connectionString = @"Data Source=DESKTOP-DC3OGQJ; Initial Catalog=MasterPoll;Integrated Security=True"; // Замените на свои данные подключения
            string query = @"
            SELECT 
                r.id AS RequestId, 
                r.order_date AS OrderDate, 
                r.total_amount AS TotalAmount, 
                p.name AS ProductName,
                pa.name AS PartnerName
            FROM order_request r
            JOIN order_product op ON r.id = op.order_id
            JOIN product p ON op.product_id = p.id
            JOIN partner pa ON r.partner_id = pa.id"; // Предполагается, что у вас есть связь между заявками и партнерами

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int requestId = reader.GetInt32(0);
                        DateTime orderDate = reader.GetDateTime(1);
                        decimal totalAmount = reader.GetDecimal(2);
                        string productName = reader.GetString(3);
                        string partnerName = reader.GetString(4);

                        AddRequestToUI(requestId, orderDate, totalAmount, productName, partnerName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}");
                }
            }
        }

        private void AddRequestToUI(int requestId, DateTime orderDate, decimal totalAmount, string productName, string partnerName)
        {
            // Создаем StackPanel для одной заявки
            StackPanel requestPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(10),
                Background = Brushes.White,
                Width = 1600
            };

            // Добавляем текстовые блоки для данных
            requestPanel.Children.Add(new TextBlock { Text = $"Имя партнера: {partnerName}", FontSize = 18, FontWeight = FontWeights.Bold });
            requestPanel.Children.Add(new TextBlock { Text = $"ID заявки: {requestId}", FontSize = 16 });
            requestPanel.Children.Add(new TextBlock { Text = $"Сумма: {totalAmount:C}", FontSize = 16 });
            requestPanel.Children.Add(new TextBlock { Text = $"Продукт: {productName}", FontSize = 16 });
            requestPanel.Children.Add(new TextBlock { Text = $"Дата заявки: {orderDate:d}", FontSize = 16 });

            // Создаем кнопку для работы с заявкой
            Button workButton = new Button
            {
                Content = "Работа с заявкой",
                Background = new SolidColorBrush(Colors.LightBlue),
                Margin = new Thickness(0, 5, 0, 0)
            };

            // Передаем ID заявки через обработчик события
            workButton.Click += (s, e) => OpenRequestWorkWindow(requestId,emp1);

            requestPanel.Children.Add(workButton);

            // Обернуть StackPanel в Border
            Border border = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Child = requestPanel
            };

            // Добавляем Border в общий StackPanel
            partnerStackPanel.Children.Add(border);
        }

        private void OpenRequestWorkWindow(int requestId, int emp1)
        {
            try
            {
                // Здесь вы можете открыть окно работы с заявкой, передав ID заявки
                RequestWorkWindow workWindow = new RequestWorkWindow(requestId,emp1); // Предполагаем, что у вас есть окно RequestWorkWindow
                workWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна работы с заявкой: {ex.Message}");
            }
        }
    }
} 
       