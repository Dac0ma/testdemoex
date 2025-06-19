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
    /// Логика взаимодействия для EditPartnerWindow.xaml
    /// </summary>
    public partial class EditPartnerWindow : Window
    {
        int emp;
        int partid;
        public EditPartnerWindow(int partnerId, int emp1)
        {
            InitializeComponent();
            emp = emp1;
            partid = partnerId;
            LoadPartnerTypes();
            LoadPartnerData();
        }
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
        private void LoadPartnerData()
        {

            try
            {

                using (SqlConnection sqlConnection = new SqlConnection(connection))
                {
                    sqlConnection.Open();

                    string query = @"
                SELECT 
                    partner.name, partner.email, partner.phone, partner.inn, 
                    partner.partner_type_id, partner.loginpart, partner.passwordpart, 
                    director.surname, director.name as namep, director.patronymic 
                FROM partner
                INNER JOIN director ON partner.director_id = director.id
                WHERE partner.id = @PartnerId";

                    SqlCommand cmd = new SqlCommand(query, sqlConnection);
                    cmd.Parameters.AddWithValue("@PartnerId", partid);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {

                        namekomp.Text = reader["name"].ToString();
                        email.Text = reader["email"].ToString();
                        nomer.Text = reader["phone"].ToString();
                        innk.Text = reader["inn"].ToString();
                        Login.Text = reader["loginpart"].ToString();
                        Password.Text = reader["passwordpart"].ToString();

                        patranomik.Text = reader["surname"].ToString();
                        nameder.Text = reader["namep"].ToString();
                        otchedir.Text = reader["patronymic"].ToString();

                        // Установка значения в ComboBox
                        typekomp.SelectedValue = Convert.ToInt32(reader["partner_type_id"]);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных партнёра: {ex.Message}");
            }
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
                    var items = new List<object>();
                    while (reader.Read())
                    {
                        items.Add(new { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                    }

                    // Настройка ComboBox
                    typekomp.ItemsSource = items; // Используем ItemsSource для привязки
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

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            prosmotrPart prosmotr = new prosmotrPart(emp);
            prosmotr.Show();
            Close();
        }

        private void savebtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(connection))
                    {
                        sqlConnection.Open();

                        // Обновляем данные директора
                        string updateDirectorQuery = @"
                UPDATE director 
                SET surname = @Surname, name = @Name, patronymic = @Patronymic 
                WHERE id = (
                    SELECT director_id 
                    FROM partner 
                    WHERE id = @PartnerId)";

                        SqlCommand directorCmd = new SqlCommand(updateDirectorQuery, sqlConnection);
                        directorCmd.Parameters.AddWithValue("@Surname", patranomik.Text);
                        directorCmd.Parameters.AddWithValue("@Name", nameder.Text);
                        directorCmd.Parameters.AddWithValue("@Patronymic", otchedir.Text);
                        directorCmd.Parameters.AddWithValue("@PartnerId", partid);
                        directorCmd.ExecuteNonQuery();

                        // Обновляем данные партнёра
                        string updatePartnerQuery = @"
                UPDATE partner 
                SET 
                    name = @Name, email = @Email, phone = @Phone, 
                    inn = @INN, partner_type_id = @PartnerTypeId, 
                    loginpart = @Login, passwordpart = @Password
                WHERE id = @PartnerId";

                        SqlCommand partnerCmd = new SqlCommand(updatePartnerQuery, sqlConnection);
                        partnerCmd.Parameters.AddWithValue("@Name", namekomp.Text);
                        partnerCmd.Parameters.AddWithValue("@Email", email.Text);
                        partnerCmd.Parameters.AddWithValue("@Phone", nomer.Text);
                        partnerCmd.Parameters.AddWithValue("@INN", innk.Text);
                        partnerCmd.Parameters.AddWithValue("@PartnerTypeId", typekomp.SelectedValue);
                        partnerCmd.Parameters.AddWithValue("@Login", Login.Text);
                        partnerCmd.Parameters.AddWithValue("@Password", Password.Text);
                        partnerCmd.Parameters.AddWithValue("@PartnerId", partid);
                        partnerCmd.ExecuteNonQuery();

                        MessageBox.Show("Данные успешно обновлены!");
                        prosmotrPart prosmotr = new prosmotrPart(emp);
                        prosmotr.Show();
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения данных: {ex.Message}");
                }
            }
        }
    }
}
