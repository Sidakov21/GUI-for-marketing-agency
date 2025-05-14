using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства.Payments
{
    public partial class AddPaymentsWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";

        public Payment NewPayment { get; set; }

        public AddPaymentsWindow()
        {
            InitializeComponent();
            PaymentDatePicker.SelectedDate = DateTime.Today;
            LoadCampaigns();
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


        private bool IsValidDecimal(string input)
        {
            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidDecimal(AmountTextBox.Text) ||
                PaymentDatePicker.SelectedDate == null ||
                PaymentTypeComboBox.SelectedItem == null ||
                CampaignComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля корректно.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidDecimal(AmountTextBox.Text) ||
                !decimal.TryParse(AmountTextBox.Text, out decimal amountValue))
            {
                MessageBox.Show("Бюджет должен содержать только цифры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int selectedCampaignId = (int)CampaignComboBox.SelectedValue;


            NewPayment = new Payment
            {
                amount = decimal.Parse(AmountTextBox.Text),
                payment_data = PaymentDatePicker.SelectedDate.Value,
                payment_type = (PaymentTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                campaign_id = selectedCampaignId
            };

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