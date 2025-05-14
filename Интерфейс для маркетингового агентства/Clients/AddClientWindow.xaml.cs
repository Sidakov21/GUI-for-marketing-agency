using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства.Clients
{
    public partial class AddClientWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";

        public Client NewClient { get; set; }

        public AddClientWindow()
        {
            InitializeComponent();
            ContractDatePicker.SelectedDate = DateTime.Today;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(ContactInfoTextBox.Text) ||
                string.IsNullOrWhiteSpace(IndustryTextBox.Text) ||
                ContractDatePicker.SelectedDate == null ||
                ContractStatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

         
            NewClient = new Client
            {
                name = NameTextBox.Text,
                contact_info = ContactInfoTextBox.Text,
                industry = IndustryTextBox.Text,
                contract_date = ContractDatePicker.SelectedDate.Value,
                contract_status = (ContractStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString()
            };

            string query = @"INSERT INTO Clients (name, contact_info, industry, contract_date, contract_status) 
                            VALUES (@name, @contact_info, @industry, @contract_date, @contract_status);
                            SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", NewClient.name);
                        cmd.Parameters.AddWithValue("@contact_info", NewClient.contact_info);
                        cmd.Parameters.AddWithValue("@industry", NewClient.industry);
                        cmd.Parameters.AddWithValue("@contract_date", NewClient.contract_date);
                        cmd.Parameters.AddWithValue("@contract_status", NewClient.contract_status);

                        NewClient.client_id = Convert.ToInt32(cmd.ExecuteScalar());

                        MessageBox.Show("Клиент успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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