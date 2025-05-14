using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства.Clients
{
    public partial class EditClientWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";
        private Client _selectedClient;

        public Client EditedClient { get; private set; }

        public EditClientWindow(Client client)
        {
            InitializeComponent();
            _selectedClient = client;
            LoadClientData();
        }

        private void LoadClientData()
        {
            NameTextBox.Text = _selectedClient.name;
            ContactInfoTextBox.Text = _selectedClient.contact_info;
            IndustryTextBox.Text = _selectedClient.industry;
            ContractDatePicker.SelectedDate = _selectedClient.contract_date;

            foreach (ComboBoxItem item in ContractStatusComboBox.Items)
            {
                if (item.Content.ToString() == _selectedClient.contract_status)
                {
                    ContractStatusComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
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

            EditedClient = new Client
            {
                client_id = _selectedClient.client_id,
                name = NameTextBox.Text,
                contact_info = ContactInfoTextBox.Text,
                industry = IndustryTextBox.Text,
                contract_date = ContractDatePicker.SelectedDate.Value,
                contract_status = (ContractStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString()
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        UPDATE Clients 
                        SET name = @name,
                            contact_info = @contact_info,
                            industry = @industry,
                            contract_date = @contract_date,
                            contract_status = @contract_status
                        WHERE client_id = @client_id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", EditedClient.name);
                    cmd.Parameters.AddWithValue("@contact_info", EditedClient.contact_info);
                    cmd.Parameters.AddWithValue("@industry", EditedClient.industry);
                    cmd.Parameters.AddWithValue("@contract_date", EditedClient.contract_date);
                    cmd.Parameters.AddWithValue("@contract_status", EditedClient.contract_status);
                    cmd.Parameters.AddWithValue("@client_id", EditedClient.client_id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Данные клиента успешно обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось обновить данные клиента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}