using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства
{
    public partial class AddPaymentsWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";

        public Payment NewPayment { get; set; }

        public AddPaymentsWindow()
        {
            InitializeComponent();
            PaymentDatePicker.SelectedDate = DateTime.Today; // Установка текущей даты по умолчанию
        }

        private bool IsValidDecimal(string input)
        {
            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        private bool IsValidInteger(string input)
        {
            return int.TryParse(input, out _);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка введенных данных
            if (!IsValidDecimal(AmountTextBox.Text) ||
                PaymentDatePicker.SelectedDate == null ||
                PaymentTypeComboBox.SelectedItem == null ||
                !IsValidInteger(CampaignIdTextBox.Text))
            {
                MessageBox.Show("Заполните все поля корректно.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка, что бюджет содержит только цифры
            if (!IsValidDecimal(AmountTextBox.Text) ||
                !decimal.TryParse(AmountTextBox.Text, out decimal amountValue))
            {
                MessageBox.Show("Бюджет должен содержать только цифры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Создаем новый объект Payment
            NewPayment = new Payment
            {
                amount = decimal.Parse(AmountTextBox.Text),
                payment_data = PaymentDatePicker.SelectedDate.Value,
                payment_type = (PaymentTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                campaign_id = int.Parse(CampaignIdTextBox.Text)
            };

            // SQL-запрос на добавление
            string query = @"INSERT INTO Payments (amount, payment_data, payment_type, campaign_id) 
                           VALUES (@amount, @payment_data, @payment_type, @campaign_id);
                           SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@amount", NewPayment.amount);
                        cmd.Parameters.AddWithValue("@payment_data", NewPayment.payment_data);
                        cmd.Parameters.AddWithValue("@payment_type", NewPayment.payment_type);
                        cmd.Parameters.AddWithValue("@campaign_id", NewPayment.campaign_id);

                        // Получаем ID новой записи
                        NewPayment.payment_id = Convert.ToInt32(cmd.ExecuteScalar());

                        MessageBox.Show("Платеж успешно добавлен.", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении платежа: {ex.Message}",
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