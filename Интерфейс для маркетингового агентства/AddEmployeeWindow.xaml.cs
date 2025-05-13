using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства
{
    public partial class AddEmployeeWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";

        public Employee NewEmployee { get; set; }

        public AddEmployeeWindow()
        {
            InitializeComponent();
        }

        private bool IsValidSalary(string input)
        {
            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        private bool IsValidContactInfo(string input)
        {
            // Проверка на email или телефон
            return input.Contains("@") || input.Any(char.IsDigit);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PositionTextBox.Text) ||
                string.IsNullOrWhiteSpace(ContactInfoTextBox.Text) ||
                string.IsNullOrWhiteSpace(SalaryTextBox.Text))
            {
                MessageBox.Show("Заполните все обязательные поля.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidContactInfo(ContactInfoTextBox.Text))
            {
                MessageBox.Show("Укажите корректную контактную информацию (email или телефон).",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidSalary(SalaryTextBox.Text) || !decimal.TryParse(SalaryTextBox.Text, out decimal salaryValue))
            {
                MessageBox.Show("Укажите корректное значение зарплаты.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Создаем нового сотрудника
            NewEmployee = new Employee
            {
                full_name = FullNameTextBox.Text,
                position = PositionTextBox.Text,
                contact_info = ContactInfoTextBox.Text,
                salary = salaryValue
            };

            // SQL-запрос на добавление
            string query = @"INSERT INTO Employees (full_name, position, contact_info, salary) 
                           VALUES (@full_name, @position, @contact_info, @salary);
                           SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@full_name", NewEmployee.full_name);
                        cmd.Parameters.AddWithValue("@position", NewEmployee.position);
                        cmd.Parameters.AddWithValue("@contact_info", NewEmployee.contact_info);
                        cmd.Parameters.AddWithValue("@salary", NewEmployee.salary);

                        // Получаем ID нового сотрудника
                        NewEmployee.employee_id = Convert.ToInt32(cmd.ExecuteScalar());

                        MessageBox.Show("Сотрудник успешно добавлен.", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}