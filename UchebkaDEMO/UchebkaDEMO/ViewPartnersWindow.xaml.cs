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

namespace UchebkaDEMO
{
    /// <summary>
    /// Логика взаимодействия для ViewPartnersWindow.xaml
    /// </summary>
    public partial class ViewPartnersWindow : Window
    {
        public ViewPartnersWindow()
        {
            InitializeComponent();
            // Получение текущей даты и времени
            DateTime currentDateTime = DateTime.Now;
            // Форматирование даты
            string formattedDate = currentDateTime.ToString("dd-MM-yyyy");
            TodayDateLabel.Content = $"Сегодня: {formattedDate}";

            LoadInfo();
        }

        private void LoadInfo(string filter = null, string typeComp = null, string comboBoxFilter = null)
        {
            this.DataContext = this;
            string connectionString = @"Data Source=DESKTOP-DC3OGQJ; Initial Catalog=DemoUCheb;Integrated Security=True"; // Замените на вашу строку подключения к БД
            string query = @"
                SELECT p.id,
                    p.name, 
                    p.firstname, 
                    p.secondname, 
                    p.patronymic, 
                    p.phone, 
                    p.rating, 
                    COALESCE(SUM(po.qty), 0) AS Volume, 
                    ct.name AS Company_type 
                FROM 
                    Partners p 
                LEFT JOIN 
                    Orders o ON p.id = o.partner_id 
                lEFT JOIN 
                    Product_orders po ON o.id = po.order_id 
                LEFT JOIN 
                    Company_types ct ON ct.id = p.type_id";

            if (!string.IsNullOrEmpty(filter))
            {
                query += " WHERE p.name LIKE @Filter OR p.firstname LIKE @Filter" +
                    " OR p.secondname LIKE @Filter OR p.patronymic LIKE @Filter" +
                    " OR p.phone LIKE @Filter " +
                    "GROUP BY p.id, p.name, p.firstname, p.secondname, p.patronymic, p.phone, p.rating, ct.name";
            }
            else if (!string.IsNullOrEmpty(typeComp))
            {
                query += " WHERE ct.name = @compType " +
                "GROUP BY p.id, p.name, p.firstname, p.secondname, p.patronymic, p.phone, p.rating, ct.name";
             
            }
            else if (!string.IsNullOrEmpty(comboBoxFilter))
            {
                if (comboBoxFilter == "По возрастанию рейтинга") 
                {
                    query += " GROUP BY p.id, p.name, p.firstname, p.secondname, p.patronymic, p.phone, p.rating, ct.name " +
                        "ORDER BY rating ASC";
                }
                else if (comboBoxFilter == "По убыванию рейтинга") 
                {
                    query += " GROUP BY p.id, p.name, p.firstname, p.secondname, p.patronymic, p.phone, p.rating, ct.name " +
                        "ORDER BY rating DESC";
                }
            }
            else
            {
                query += " GROUP BY p.id, p.name, p.firstname, p.secondname, p.patronymic, p.phone, p.rating, ct.name";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);

                    if (!string.IsNullOrEmpty(filter))
                    {
                        command.Parameters.AddWithValue("@Filter", "%" + filter + "%");
                    }
                    else if (!string.IsNullOrEmpty(typeComp))
                    {
                        command.Parameters.AddWithValue("@compType", typeComp);
                    }

                    SqlDataReader reader = command.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        long totalSold = Convert.ToInt64(reader["Volume"]);
                        string discount = CalcDiscount(totalSold);
                        StackPanel partnerPanel = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            Margin = new Thickness(10),
                            Background = new SolidColorBrush(Colors.White)
                        };

                        // Получите идентификатор партнера (предполагается, что он есть в выборке)
                        int partnerId = Convert.ToInt32(reader["id"]); // Замените "id" на фактическое имя столбца

                        // Назначьте обработчик события MouseDown
                        partnerPanel.MouseDown += (s, e) => PartnerStackPanel_MouseDown(s, e, partnerId);

                        Grid grid = new Grid
                        {
                                Margin = new Thickness(5,5,5,0),
                        };

                        grid.Children.Add(new TextBlock
                        {
                            Text = $"{reader["Company_type"]}| {reader["name"]}",
                            FontSize = 16,
                            FontWeight = FontWeights.Bold,
                            HorizontalAlignment = HorizontalAlignment.Left,
                        });

                        grid.Children.Add(new TextBlock
                        {
                            Text = discount,
                            FontSize = 16,
                            FontWeight = FontWeights.Bold,
                            HorizontalAlignment = HorizontalAlignment.Right,
                        });
                        
                        partnerPanel.Children.Add(grid);

                        partnerPanel.Children.Add(new TextBlock
                        {
                            Text = $"Директор: {reader["firstname"]} {reader["secondname"]} {reader["patronymic"]}",
                            FontSize = 12,
                            Margin = new Thickness(5,0,5,0),
                        });

                        partnerPanel.Children.Add(new TextBlock
                        {
                            Text = $"+7 {reader["phone"]}",
                            FontSize = 12,
                            Margin = new Thickness(5, 0, 5, 0),
                        });

                        partnerPanel.Children.Add(new TextBlock
                        {
                            Text = $"Рейтинг: {reader["rating"]}",
                            FontSize = 12,
                            Margin = new Thickness(5, 0, 5, 5),
                        });

                        PartnerStackPanel.Children.Add(partnerPanel);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex}");
            }
        }

        public static string CalcDiscount(Int64 volume)
        {
            string discount;
            if (volume < 10000)
            {
                discount = "0%"; // 0%
            }
            else if (volume >= 10000 && volume < 50000)
            {
                discount = "5%"; // 5%
            }
            else if (volume >= 50000 && volume < 300000)
            {
                discount = "10%"; // 10%
            }
            else
            {
                discount = "15%"; // 15%
            }
            return discount;
        } 

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void AddInfo_Button_Click(object sender, RoutedEventArgs e)
        {
            AddPartnerWindow addPartnerWindow = new AddPartnerWindow();
            addPartnerWindow.Show();
            this.Close();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterComboBox.SelectedItem = null;
            CompanyTypeComboBox.SelectedItem = null;
            CompanyTypeComboBox.Visibility = Visibility.Collapsed;
            PartnerStackPanel.Children.Clear();
            LoadInfo(SearchTextBox.Text);
        }

        private void CompanyTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CompanyTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedText = selectedItem.Content.ToString();
                PartnerStackPanel.Children.Clear();
                LoadInfo(null, selectedText);
            }
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (selectedItem.Content.ToString() == "По типу")
                {
                    CompanyTypeComboBox.Visibility = Visibility.Visible;
                }
                else if (selectedItem.Content.ToString() != "По типу"
                    && !string.IsNullOrEmpty(selectedItem.Content.ToString()))
                {
                    string selectedText = selectedItem.Content.ToString();
                    PartnerStackPanel.Children.Clear();
                    LoadInfo(null, null, selectedText);
                }
                else { CompanyTypeComboBox.Visibility = Visibility.Collapsed; }
            }
        }

        private void PartnerStackPanel_MouseDown(object sender, MouseButtonEventArgs e, int partId)
        {
            EditPartnerWindow editWindow = new EditPartnerWindow(partId);
            editWindow.Show();
            this.Close();
        }
    }
}
