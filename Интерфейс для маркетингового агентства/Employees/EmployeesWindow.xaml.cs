using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства.Employees
{
    public class Employee
    {
        public int employee_id { get; set; }
        public string full_name { get; set; }
        public string position { get; set; }
        public string contact_info { get; set; }
        public decimal salary { get; set; }
    }

    public partial class EmployeesWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";
        private List<Employee> employees = new List<Employee>();

        public EmployeesWindow()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            employees.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Employees";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            employee_id = reader.GetInt32(0),
                            full_name = reader.GetString(1),
                            position = reader.GetString(2),
                            contact_info = reader.GetString(3),
                            salary = reader.GetDecimal(4)
                        });
                    }

                    EmployeesDataGrid.ItemsSource = employees;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
                }
            }
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            AddEmployeeWindow addWindow = new AddEmployeeWindow();
            addWindow.Owner = this;

            if (addWindow.ShowDialog() == true)
            {
                employees.Add(addWindow.NewEmployee);
                EmployeesDataGrid.Items.Refresh();
            }
        }

        private void EditEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Employee selectedEmployee = (Employee)EmployeesDataGrid.SelectedItem;
            EditEmployeeWindow editWindow = new EditEmployeeWindow(selectedEmployee);
            editWindow.Owner = this;

            if (editWindow.ShowDialog() == true)
            {
                int index = employees.FindIndex(emp => emp.employee_id == editWindow.EditedEmployee.employee_id);
                if (index != -1)
                {
                    employees[index] = editWindow.EditedEmployee;
                    EmployeesDataGrid.Items.Refresh();
                }
            }
        }

        private void DeleteEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника для удаления.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Employee selectedEmployee = (Employee)EmployeesDataGrid.SelectedItem;

            MessageBoxResult result = MessageBox.Show(
                $"Вы точно хотите удалить сотрудника '{selectedEmployee.full_name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM Employees WHERE employee_id = @id";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", selectedEmployee.employee_id);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                employees.Remove(selectedEmployee);
                                EmployeesDataGrid.Items.Refresh();

                                MessageBox.Show("Сотрудник успешно удален", "Успех",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var startWindow = new StartWindow();
            startWindow.Show();
            this.Close();
        }
    }
}