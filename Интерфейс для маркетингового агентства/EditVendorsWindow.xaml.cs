using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства
{
    public partial class EditVendorsWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";
        private Vendor _selectedVendor;

        public Vendor EditedVendor { get; set; }

        public EditVendorsWindow(Vendor vendor)
        {
            InitializeComponent();
            _selectedVendor = vendor;
            LoadVendorData();
        }

        private void LoadVendorData()
        {
            // Заполняем поля данными выбранного поставщика
            NameTextBox.Text = _selectedVendor.name;
            ServiceTypeTextBox.Text = _selectedVendor.service_type;
            ContactInfoTextBox.Text = _selectedVendor.contact_info;
            ContractTermsTextBox.Text = _selectedVendor.contract_terms;
        }

        private bool IsValidContactInfo(string input)
        {
            // Проверка, что контактная информация содержит email или телефон
            return input.Contains("@") || input.Any(char.IsDigit);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка введенных данных
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(ServiceTypeTextBox.Text) ||
                string.IsNullOrWhiteSpace(ContactInfoTextBox.Text) ||
                string.IsNullOrWhiteSpace(ContractTermsTextBox.Text))
            {
                MessageBox.Show("Заполните все обязательные поля.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка контактной информации
            if (!IsValidContactInfo(ContactInfoTextBox.Text))
            {
                MessageBox.Show("Укажите корректную контактную информацию (email или телефон).",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Создаем объект с обновленными данными
            EditedVendor = new Vendor
            {
                vendor_id = _selectedVendor.vendor_id,
                name = NameTextBox.Text,
                service_type = ServiceTypeTextBox.Text,
                contact_info = ContactInfoTextBox.Text,
                contract_terms = ContractTermsTextBox.Text
            };

            // Обновляем запись в БД
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        UPDATE Vendors 
                        SET name = @name,
                            service_type = @service_type,
                            contact_info = @contact_info,
                            contract_terms = @contract_terms
                        WHERE vendor_id = @vendor_id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", EditedVendor.name);
                    cmd.Parameters.AddWithValue("@service_type", EditedVendor.service_type);
                    cmd.Parameters.AddWithValue("@contact_info", EditedVendor.contact_info);
                    cmd.Parameters.AddWithValue("@contract_terms", EditedVendor.contract_terms);
                    cmd.Parameters.AddWithValue("@vendor_id", EditedVendor.vendor_id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Данные поставщика успешно обновлены.", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось обновить данные поставщика.", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении поставщика: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}