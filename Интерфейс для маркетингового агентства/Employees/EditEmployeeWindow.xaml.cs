using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства.Employees
{
    public partial class EditEmployeeWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";
        private Employee _originalEmployee;

        public Employee EditedEmployee { get; private set; }

        public EditEmployeeWindow(Employee employeeToEdit)
        {
            InitializeComponent();
            _originalEmployee = employeeToEdit;
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            FullNameTextBox.Text = _originalEmployee.full_name;
            PositionTextBox.Text = _originalEmployee.position;
            ContactInfoTextBox.Text = _originalEmployee.contact_info;
            SalaryTextBox.Text = _originalEmployee.salary.ToString("N2");
        }

        private bool IsValidSalary(string input)
        {
            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        private bool IsValidContactInfo(string input)
        {
            return input.Contains("@") || input.Any(char.IsDigit);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
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

            EditedEmployee = new Employee
            {
                employee_id = _originalEmployee.employee_id,
                full_name = FullNameTextBox.Text,
                position = PositionTextBox.Text,
                contact_info = ContactInfoTextBox.Text,
                salary = salaryValue
            };

            string query = @"UPDATE Employees 
                           SET full_name = @full_name,
                               position = @position,
                               contact_info = @contact_info,
                               salary = @salary
                           WHERE employee_id = @employee_id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@full_name", EditedEmployee.full_name);
                        cmd.Parameters.AddWithValue("@position", EditedEmployee.position);
                        cmd.Parameters.AddWithValue("@contact_info", EditedEmployee.contact_info);
                        cmd.Parameters.AddWithValue("@salary", EditedEmployee.salary);
                        cmd.Parameters.AddWithValue("@employee_id", EditedEmployee.employee_id);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Данные сотрудника успешно обновлены.", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            this.DialogResult = true;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить данные сотрудника.", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении сотрудника: {ex.Message}",
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