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
    /// Логика взаимодействия для createzayavki.xaml
    /// </summary>
    public partial class createzayavki : Window
    {
        int partnerid;
        public createzayavki(int partid)
        {
            InitializeComponent();
            partnerid = partid;
            LoadProducts();

        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            PartnerWindow partner = new PartnerWindow(partnerid);
            partner.Show();
            Close();
        }

        private void LoadProducts()
        {
            string connectionString = @"Data Source=DESKTOP-DC3OGQJ; Initial Catalog=MasterPoll;Integrated Security=True"; // Замените на свои данные подключения
            string query = "SELECT id, name FROM product";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    var products = new List<Product>();

                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }

                    product.ItemsSource = products;
                    product.DisplayMemberPath = "Name"; // Что будет отображаться
                    product.SelectedValuePath = "Id"; // Что будет передаваться как Value
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
                }
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (product.SelectedValue == null || string.IsNullOrEmpty(kolvo.Text))
            {
                MessageBox.Show("Выберите продукцию и введите количество.");
                return;
            }

            if (!int.TryParse(kolvo.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Количество должно быть положительным числом.");
                return;
            }

            int productId = (int)product.SelectedValue;

            string connectionString = @"Data Source=DESKTOP-DC3OGQJ; Initial Catalog=MasterPoll;Integrated Security=True"; // Замените на свои данные подключения

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Получение цены продукции
                    decimal minPrice = 0;
                    string getPriceQuery = "SELECT min_price FROM product WHERE id = @ProductId";
                    using (SqlCommand getPriceCommand = new SqlCommand(getPriceQuery, connection))
                    {
                        getPriceCommand.Parameters.AddWithValue("@ProductId", productId);
                        object result = getPriceCommand.ExecuteScalar();
                        if (result != null)
                        {
                            minPrice = (decimal)result;
                        }
                        else
                        {
                            MessageBox.Show("Продукция не найдена.");
                            return;
                        }
                    }

                    decimal totalAmount = minPrice * quantity;

                    // Вставка записи в order_request
                    string insertOrderRequestQuery = @"
                INSERT INTO order_request (partner_id, employee_id, order_date, status, total_amount, prepayment)
                OUTPUT INSERTED.id
                VALUES (@PartnerId, NULL, GETDATE(), @Status, @TotalAmount, 0)";
                    int orderId;
                    using (SqlCommand insertOrderRequestCommand = new SqlCommand(insertOrderRequestQuery, connection))
                    {
                        insertOrderRequestCommand.Parameters.AddWithValue("@PartnerId", partnerid);
                        insertOrderRequestCommand.Parameters.AddWithValue("@Status", "на рассмотрении");
                        insertOrderRequestCommand.Parameters.AddWithValue("@TotalAmount", totalAmount);

                        orderId = (int)insertOrderRequestCommand.ExecuteScalar();
                    }

                    // Вставка записи в order_product
                    string insertOrderProductQuery = @"
                INSERT INTO order_product (order_id, product_id, qty, delivery_date)
                VALUES (@OrderId, @ProductId, @Quantity, NULL)";
                    using (SqlCommand insertOrderProductCommand = new SqlCommand(insertOrderProductQuery, connection))
                    {
                        insertOrderProductCommand.Parameters.AddWithValue("@OrderId", orderId);
                        insertOrderProductCommand.Parameters.AddWithValue("@ProductId", productId);
                        insertOrderProductCommand.Parameters.AddWithValue("@Quantity", quantity);

                        insertOrderProductCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("Заявка успешно оформлена!");
                    PartnerWindow partner = new PartnerWindow(partnerid);
                    partner.Show();
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении заявки: {ex.Message}");
                }
            }
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

