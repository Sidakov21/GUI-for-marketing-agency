using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства.Payments
{
    public partial class EditPaymentsWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";
        private Payment _selectedPayment;

        public Payment EditedPayment { get; private set; }

        public EditPaymentsWindow(Payment payment)
        {
            InitializeComponent();
            LoadCampaigns();
            _selectedPayment = payment;
            LoadPaymentData();
        }


        private void LoadCampaigns()
        {
            var campaignList = new List<Campaigns.Campaign>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT campaign_id, name FROM Campaigns";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            campaignList.Add(new Campaigns.Campaign
                            {
                                campaign_id = reader.GetInt32(0),
                                name = reader.GetString(1)
                            });
                        }
                    }

                    CampaignComboBox.ItemsSource = campaignList;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки кампаний: " + ex.Message);
                }
            }
        }

        private void LoadPaymentData()
        {
            AmountTextBox.Text = _selectedPayment.amount.ToString("N2");
            PaymentDatePicker.SelectedDate = _selectedPayment.payment_data;
            CampaignComboBox.SelectedValue = _selectedPayment.campaign_id;

            foreach (ComboBoxItem item in PaymentTypeComboBox.Items)
            {
                if (item.Content.ToString() == _selectedPayment.payment_type)
                {
                    PaymentTypeComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private bool IsValidDecimal(string input)
        {
            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidDecimal(AmountTextBox.Text) ||
                PaymentDatePicker.SelectedDate == null ||
                PaymentTypeComboBox.SelectedItem == null ||
                PaymentTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля корректно.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditedPayment = new Payment
            {
                payment_id = _selectedPayment.payment_id,
                amount = decimal.Parse(AmountTextBox.Text),
                payment_data = PaymentDatePicker.SelectedDate.Value,
                payment_type = (PaymentTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                campaign_id = (int)CampaignComboBox.SelectedValue
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE Payments 
                                   SET amount = @amount,
                                       payment_data = @payment_data,
                                       payment_type = @payment_type,
                                       campaign_id = @campaign_id
                                   WHERE payment_id = @payment_id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@amount", EditedPayment.amount);
                    cmd.Parameters.AddWithValue("@payment_data", EditedPayment.payment_data);
                    cmd.Parameters.AddWithValue("@payment_type", EditedPayment.payment_type);
                    cmd.Parameters.AddWithValue("@campaign_id", EditedPayment.campaign_id);
                    cmd.Parameters.AddWithValue("@payment_id", EditedPayment.payment_id);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Платеж успешно обновлен.", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении платежа: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}