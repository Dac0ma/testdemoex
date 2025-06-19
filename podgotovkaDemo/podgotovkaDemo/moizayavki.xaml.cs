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
    /// Логика взаимодействия для moizayavki.xaml
    /// </summary>
    public partial class moizayavki : Window
    {
        int partner_id;
        public moizayavki(int partid)
        {
            InitializeComponent();
            partner_id = partid;
            LoadPartnerRequests();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            PartnerWindow partnerWindow = new PartnerWindow(partner_id);
            partnerWindow.Show();
            Close();
        }
        private void LoadPartnerRequests()
        {
            string connectionString = @"Data Source=DESKTOP-DC3OGQJ; Initial Catalog=MasterPoll;Integrated Security=True"; // Замените на свои данные подключения
            string query = @"
        SELECT 
            r.id, 
            r.order_date, 
            r.status, 
            r.total_amount, 
            r.prepayment,
            p.name AS ProductName,
            op.qty,
            op.delivery_date
        FROM order_request r
        JOIN order_product op ON r.id = op.order_id
        JOIN product p ON op.product_id = p.id
        WHERE r.partner_id = @PartnerId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PartnerId", partner_id);

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int requestId = reader.GetInt32(0);
                        DateTime orderDate = reader.GetDateTime(1);
                        string status = reader.GetString(2);
                        decimal totalAmount = reader.GetDecimal(3);
                        decimal prepayment = reader.GetDecimal(4);
                        string productName = reader.GetString(5);
                        int quantity = reader.GetInt32(6);
                        DateTime? deliveryDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7);

                        AddRequestToUI(requestId, orderDate, status, totalAmount, prepayment, productName, quantity, deliveryDate);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}");
                }
            }
        }


        private void AddRequestToUI(int requestId, DateTime orderDate, string status, decimal totalAmount, decimal prepayment, string productName, int quantity, DateTime? deliveryDate)
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
            requestPanel.Children.Add(new TextBlock { Text = $"ID заявки: {requestId}", FontSize = 18, FontWeight = FontWeights.Bold });
            requestPanel.Children.Add(new TextBlock { Text = $"Дата заявки: {orderDate:d}", FontSize = 16 });
            requestPanel.Children.Add(new TextBlock { Text = $"Статус: {status}", FontSize = 16 });
            requestPanel.Children.Add(new TextBlock { Text = $"Сумма: {totalAmount:C}", FontSize = 16 });
            requestPanel.Children.Add(new TextBlock { Text = $"Предоплата: {prepayment:C}", FontSize = 16 });
            requestPanel.Children.Add(new TextBlock { Text = $"Продукт: {productName}", FontSize = 16 });
            requestPanel.Children.Add(new TextBlock { Text = $"Количество: {quantity}", FontSize = 16 });
            requestPanel.Children.Add(new TextBlock { Text = $"Дата доставки: {(deliveryDate.HasValue ? deliveryDate.Value.ToShortDateString() : "Не указана")}", FontSize = 16 });

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


    }
}
