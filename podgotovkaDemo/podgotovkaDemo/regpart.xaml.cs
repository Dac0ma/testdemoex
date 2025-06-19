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

namespace podgotovkaDemo
{
    /// <summary>
    /// Логика взаимодействия для regpart.xaml
    /// </summary>
    public partial class regpart : Window
    {
        int emp1;
        public regpart(int emp)
        {
            InitializeComponent();
            LoadPartnerTypes();
            emp1 = emp;
        }

        private void LoadPartnerTypes()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connection))
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT id, name FROM partner_type", sqlConnection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        typekomp.Items.Add(new { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                    }
                    typekomp.DisplayMemberPath = "Name";
                    typekomp.SelectedValuePath = "Id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов компании: {ex.Message}");
            }
        }
        string connection = @"Data Source=DESKTOP-DC3OGQJ; Initial Catalog=MasterPoll;Integrated Security=True";

        private bool ValidateInput()
        {
            // Проверка email
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email.Text, emailPattern))
            {
                MessageBox.Show("Некорректный Email");
                return false;
            }

            // Проверка логина
            if (Login.Text.Length < 8)
            {
                MessageBox.Show("Логин должен содержать минимум 8 символов");
                return false;
            }

            // Проверка пароля
            string passwordPattern = @"^(?=.*[A-Z])(?=.*\d).{8,}$";
            if (!Regex.IsMatch(Password.Text, passwordPattern))
            {
                MessageBox.Show("Пароль должен быть не менее 8 символов, содержать минимум одну цифру и одну заглавную букву");
                return false;
            }

            // Проверка телефона
            string phonePattern = @"^\+7\d{10}$";
            if (!Regex.IsMatch(nomer.Text, phonePattern))
            {
                MessageBox.Show("Телефон должен начинаться с +7 и содержать 10 цифр после");
                return false;
            }

            // Проверка выбранного типа компании
            if (typekomp.SelectedValue == null)
            {
                MessageBox.Show("Выберите тип компании");
                return false;
            }

            return true;
        }

        private void reg_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connection))
                {
                    sqlConnection.Open();

                    // Шаг 1: Добавляем директора и получаем его ID
                    string insertDirectorQuery = "INSERT INTO director (surname, name, patronymic) OUTPUT INSERTED.id VALUES (@Firstname, @Lastname, @Patronymic)";
                    SqlCommand directorCmd = new SqlCommand(insertDirectorQuery, sqlConnection);
                    directorCmd.Parameters.AddWithValue("@Firstname", patranomik.Text);
                    directorCmd.Parameters.AddWithValue("@Lastname", nameder.Text);
                    directorCmd.Parameters.AddWithValue("@Patronymic", otchedir.Text);

                    // Получаем ID добавленного директора
                    int directorId = (int)directorCmd.ExecuteScalar();

                    // Шаг 2: Добавляем партнера с указанным ID директора
                    string insertPartnerQuery = "INSERT INTO partner (name, email, phone, address, inn, partner_type_id, director_id, loginpart, passwordpart,rating) " +
                                                "VALUES (@Name, @Email, @Phone, '', @INN, @PartnerTypeId, @DirectorId, @Login, @Password,5)";
                    SqlCommand partnerCmd = new SqlCommand(insertPartnerQuery, sqlConnection);
                    partnerCmd.Parameters.AddWithValue("@Name", namekomp.Text);
                    partnerCmd.Parameters.AddWithValue("@Email", email.Text);
                    partnerCmd.Parameters.AddWithValue("@Phone", nomer.Text);
                    partnerCmd.Parameters.AddWithValue("@INN", innk.Text);
                    partnerCmd.Parameters.AddWithValue("@PartnerTypeId", typekomp.SelectedValue);
                    partnerCmd.Parameters.AddWithValue("@DirectorId", directorId); // Передаем ID директора
                    partnerCmd.Parameters.AddWithValue("@Login", Login.Text);
                    partnerCmd.Parameters.AddWithValue("@Password", Password.Text);

                    partnerCmd.ExecuteNonQuery();

                    MessageBox.Show("Регистрация партнера успешна!");
                    UserWindow userWindow = new UserWindow(emp1);
                    userWindow.Show();
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}");
            }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            UserWindow userWindow=new UserWindow(emp1);
            userWindow.Show();
            Close();
        }
    }
}
