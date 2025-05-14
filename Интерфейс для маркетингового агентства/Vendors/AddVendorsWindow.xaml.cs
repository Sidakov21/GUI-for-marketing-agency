using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства.Vendors
{
    public partial class AddVendorsWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";

        public Vendor NewVendor { get; set; }

        public AddVendorsWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(ServiceTypeTextBox.Text) ||
                string.IsNullOrWhiteSpace(ContactInfoTextBox.Text) ||
                string.IsNullOrWhiteSpace(ContractTermsTextBox.Text))
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

            NewVendor = new Vendor
            {
                name = NameTextBox.Text,
                service_type = ServiceTypeTextBox.Text,
                contact_info = ContactInfoTextBox.Text,
                contract_terms = ContractTermsTextBox.Text
            };

            string query = @"INSERT INTO Vendors (name, service_type, contact_info, contract_terms) 
                           VALUES (@name, @service_type, @contact_info, @contract_terms);
                           SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", NewVendor.name);
                        cmd.Parameters.AddWithValue("@service_type", NewVendor.service_type);
                        cmd.Parameters.AddWithValue("@contact_info", NewVendor.contact_info);
                        cmd.Parameters.AddWithValue("@contract_terms", NewVendor.contract_terms);

                        NewVendor.vendor_id = Convert.ToInt32(cmd.ExecuteScalar());

                        MessageBox.Show("Поставщик успешно добавлен.", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        this.DialogResult = true;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении поставщика: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool IsValidContactInfo(string input)
        {
            return input.Contains("@") || input.Any(char.IsDigit);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}