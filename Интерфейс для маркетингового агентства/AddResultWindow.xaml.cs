using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства
{
    public partial class AddResultWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";

        public CampaignResult NewResult { get; set; }

        public AddResultWindow()
        {
            InitializeComponent();
            EndMonthPicker.SelectedDate = DateTime.Today; // Установка текущей даты по умолчанию
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
            // Проверка полей
            if (!IsValidInteger(ImpressionsTextBox.Text) ||
                !IsValidInteger(ClicksTextBox.Text) ||
                !IsValidInteger(ConversionsTextBox.Text) ||
                !IsValidInteger(LeadCountTextBox.Text) ||
                !IsValidDecimal(RoiTextBox.Text) ||
                !IsValidDecimal(CostTextBox.Text) ||
                EndMonthPicker.SelectedDate == null ||
                !IsValidInteger(CampaignIdTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Создание объекта результата
            NewResult = new CampaignResult
            {
                impressions = int.Parse(ImpressionsTextBox.Text),
                clicks = int.Parse(ClicksTextBox.Text),
                conversions = int.Parse(ConversionsTextBox.Text),
                lead_count = int.Parse(LeadCountTextBox.Text),
                roi = decimal.Parse(RoiTextBox.Text, CultureInfo.InvariantCulture),
                cost = decimal.Parse(CostTextBox.Text, CultureInfo.InvariantCulture),
                end_month = EndMonthPicker.SelectedDate.Value,
                campaign_id = int.Parse(CampaignIdTextBox.Text)
            };

            // SQL-запрос на добавление
            string query = @"INSERT INTO Campaign_Results 
                             (impressions, clicks, conversions, lead_count, roi, cost, end_month, campaign_id)
                             VALUES (@impressions, @clicks, @conversions, @lead_count, @roi, @cost, @end_month, @campaign_id);
                             SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@impressions", NewResult.impressions);
                        cmd.Parameters.AddWithValue("@clicks", NewResult.clicks);
                        cmd.Parameters.AddWithValue("@conversions", NewResult.conversions);
                        cmd.Parameters.AddWithValue("@lead_count", NewResult.lead_count);
                        cmd.Parameters.AddWithValue("@roi", NewResult.roi);
                        cmd.Parameters.AddWithValue("@cost", NewResult.cost);
                        cmd.Parameters.AddWithValue("@end_month", NewResult.end_month);
                        cmd.Parameters.AddWithValue("@campaign_id", NewResult.campaign_id);

                        NewResult.result_id = Convert.ToInt32(cmd.ExecuteScalar());

                        MessageBox.Show("Результат кампании успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении результата: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
