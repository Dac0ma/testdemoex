using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для EditPartnerWindow.xaml
    /// </summary>
    public partial class EditPartnerWindow : Window
    {
        int idPart;
        string connectionString = @"Data Source=DESKTOP-DC3OGQJ; Initial Catalog=DemoUCheb;Integrated Security=True";
        public EditPartnerWindow(int partnerId)
        {
            idPart = partnerId;
            InitializeComponent();
            LoadPartners();
            LoadPartnerInfo(partnerId);
            // Получение текущей даты и времени
            DateTime currentDateTime = DateTime.Now;
            // Форматирование даты
            string formattedDate = currentDateTime.ToString("dd-MM-yyyy");
            DateToday_Label.Content = $"Сегодня: {formattedDate}";
        }

        private bool ValidInfo()
        {
            string partnerName = PartnerNameTextBox.Text;
            if (CompanyTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите тип партнера.");
                return false;
            }
            int partnerTypeId = ((KeyValuePair<int, string>)CompanyTypeComboBox.SelectedItem).Key;
            string directorName = DirectorTextBox.Text;
            string rating = RatingTextBox.Text;
            string phoneNumber = PhoneTextBox.Text;
            string email = EmailTextBox.Text;
            string address = AddressTextBox.Text;

            // Валидация номера телефона
            if (phoneNumber.StartsWith("+7"))
            {
                phoneNumber = phoneNumber.Substring(3); // Убираем "+7 "
            }
            if (phoneNumber.Length != 13)
            {
                MessageBox.Show("Номер телефона должен быть в формате 493 123 45 67");
                return false;
            }

            // Валидация других полей
            if (string.IsNullOrWhiteSpace(partnerName) || string.IsNullOrWhiteSpace(directorName) || string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
                return false;
            }

            return true;
        }

        private void LoadPartners()
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();
                    string query = "SELECT id, name FROM Company_types";
                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    SqlDataReader reader = command.ExecuteReader();
                    CompanyTypeComboBox.Items.Clear();

                    var companyTypes = new List<KeyValuePair<int, string>>();

                    while (reader.Read())
                    {
                        companyTypes.Add(new KeyValuePair<int, string>(
                            Convert.ToInt32(reader["id"]),
                            reader["name"].ToString()));
                    }

                    CompanyTypeComboBox.ItemsSource = companyTypes;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки типов партнёра: {ex.Message}");
                }
            }
        }

        private void LoadPartnerInfo(int partId)
        {
            string query = $"SELECT name, legal_address, firstname, secondname, patronymic, phone, email, rating, type_id FROM Partners WHERE id = @partId";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@partId", partId);
                SqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    if (reader.Read()) // Читаем первую запись
                    {
                        PartnerNameTextBox.Text = reader["name"].ToString();
                        AddressTextBox.Text = reader["legal_address"].ToString(); // Предполагается, что у вас есть LegalAddressTextBox
                        string firstName = reader["firstname"].ToString();
                        string secondName = reader["secondname"].ToString(); // Предполагается, что у вас есть SecondNameTextBox
                        string patronymic = reader["patronymic"].ToString(); // Предполагается, что у вас есть PatronymicTextBox

                        // Объединяем имя, фамилию и отчество
                        DirectorTextBox.Text = $"{firstName} {secondName} {patronymic}";
                        PhoneTextBox.Text = "+7 "+reader["phone"].ToString();
                        EmailTextBox.Text = reader["email"].ToString();
                        RatingTextBox.Text = reader["rating"].ToString();
                        int typeId = Convert.ToInt32(reader["type_id"]);
                        CompanyTypeComboBox.SelectedIndex = typeId - 1; // Устанавливаем выбранное значение
                    }
                    else
                    {
                        MessageBox.Show("Партнер не найден.");
                    }
                }
                catch(Exception ex) 
                { 
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SaveInfo_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ValidInfo() == true)
            {
                int PartnerId = idPart;
                string partnerName = PartnerNameTextBox.Text;
                int partnerTypeId = ((KeyValuePair<int, string>)CompanyTypeComboBox.SelectedItem).Key;
                string directorFullName = DirectorTextBox.Text;
                string rating = RatingTextBox.Text;
                string phoneNumber = PhoneTextBox.Text;
                string email = EmailTextBox.Text;
                string address = AddressTextBox.Text;

                string[] nameParts = directorFullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Проверяем количество частей
                if (nameParts.Length < 2 || nameParts.Length > 3)
                {
                    MessageBox.Show("Пожалуйста, введите Фамилию, Имя и Отчество через пробел.");
                    return; // Выход из метода, если формат неправильный
                }

                string firstName = nameParts[1]; // Имя
                string lastName = nameParts[0]; // Фамилия
                string patronymic = nameParts.Length == 3 ? nameParts[2] : ""; // Отчество (если есть)

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "UPDATE Partners SET " +
                                    "name = @partnerName, " +
                                    "legal_address = @address, " +
                                    "firstname = @firstname, " +
                                    "secondname = @secondname, " +
                                    "patronymic = @patronymic, " +
                                    "phone = @phone, " +
                                    "email = @email, " +
                                    "rating = @rating, " +
                                    "type_id = @typeId " +
                                    "WHERE id = @PartnerId";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@partnerName", partnerName);
                        cmd.Parameters.AddWithValue("@address", address);
                        cmd.Parameters.AddWithValue("@firstname", firstName);
                        cmd.Parameters.AddWithValue("@secondname", lastName);
                        cmd.Parameters.AddWithValue("@patronymic", patronymic);
                        cmd.Parameters.AddWithValue("@phone", phoneNumber.Substring(3));
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@rating", rating);
                        cmd.Parameters.AddWithValue("@typeId", partnerTypeId);
                        cmd.Parameters.AddWithValue("@PartnerId", PartnerId);


                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Данные партнера успешно обновлены");
                            ViewPartnersWindow viewPartnersWindow = new ViewPartnersWindow();
                            viewPartnersWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить данные партнера.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка добавления данных: {ex.Message}");
                    }
                }
            }
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            ViewPartnersWindow viewPartnersWindow = new ViewPartnersWindow();
            viewPartnersWindow.Show();
            this.Close();
        }

        private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void PhoneTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (PhoneTextBox.SelectionStart < 4 && (e.Key == Key.Back || e.Key == Key.Delete))
            {
                e.Handled = true;
            }
        }
        private static bool IsTextAllowed(string text)
        {
            // Проверка, что вводимые символы - это цифры
            return Regex.IsMatch(text, @"^[0-9]+$");
        }

        private void PhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PhoneTextBox.TextChanged -= PhoneTextBox_TextChanged;

            // Получаем текст без пробелов
            string input = PhoneTextBox.Text.Replace(" ", "");

            if (input.Length > 12)
            {
                input = input.Substring(0, 12);
            }

            // Форматируем номер телефона
            if (input.Length > 0)
            {
                // Добавляем пробелы в нужные места
                string formatted = "";

                for (int i = 0; i < input.Length; i++)
                {
                    if (i == 2 || i == 5 || i == 8 || i == 10) // Добавляем пробелы после 3 и 6 символов
                    {
                        formatted += " ";
                    }
                    formatted += input[i];
                }

                // Обновляем текст в TextBox
                PhoneTextBox.Text = formatted;
                // Устанавливаем курсор в конец текста
                PhoneTextBox.SelectionStart = PhoneTextBox.Text.Length;
            }

            // Восстанавливаем обработчик события
            PhoneTextBox.TextChanged += PhoneTextBox_TextChanged;
        }
    }
}
