using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства.CampaignResults
{
    public partial class EditResultWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";
        private CampaignResult _selectedResult;

        public CampaignResult EditedResult { get; set; }

        public EditResultWindow(CampaignResult result)
        {
            InitializeComponent();
            LoadCampaigns();
            _selectedResult = result;
            LoadResultData();
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

        private void LoadResultData()
        {
            ImpressionsTextBox.Text = _selectedResult.impressions.ToString();
            ClicksTextBox.Text = _selectedResult.clicks.ToString();
            ConversionsTextBox.Text = _selectedResult.conversions.ToString();
            LeadCountTextBox.Text = _selectedResult.lead_count.ToString();
            RoiTextBox.Text = _selectedResult.roi.ToString(CultureInfo.InvariantCulture);
            CostTextBox.Text = _selectedResult.cost.ToString(CultureInfo.InvariantCulture);
            EndMonthPicker.SelectedDate = _selectedResult.end_month;
           CampaignComboBox.SelectedValue = _selectedResult.campaign_id;
        }

        private bool IsValidInteger(string input) => int.TryParse(input, out _);

        private bool IsValidDecimal(string input) =>
            decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out _);

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidInteger(ImpressionsTextBox.Text) ||
                !IsValidInteger(ClicksTextBox.Text) ||
                !IsValidInteger(ConversionsTextBox.Text) ||
                !IsValidInteger(LeadCountTextBox.Text) ||
                !IsValidDecimal(RoiTextBox.Text) ||
                !IsValidDecimal(CostTextBox.Text) ||
                EndMonthPicker.SelectedDate == null ||
                CampaignComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditedResult = new CampaignResult
            {
                result_id = _selectedResult.result_id,
                impressions = int.Parse(ImpressionsTextBox.Text),
                clicks = int.Parse(ClicksTextBox.Text),
                conversions = int.Parse(ConversionsTextBox.Text),
                lead_count = int.Parse(LeadCountTextBox.Text),
                roi = decimal.Parse(RoiTextBox.Text, CultureInfo.InvariantCulture),
                cost = decimal.Parse(CostTextBox.Text, CultureInfo.InvariantCulture),
                end_month = EndMonthPicker.SelectedDate.Value,
                campaign_id = (int)CampaignComboBox.SelectedValue
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        UPDATE Campaign_Results 
                        SET impressions = @impressions,
                            clicks = @clicks,
                            conversions = @conversions,
                            lead_count = @lead_count,
                            roi = @roi,
                            cost = @cost,
                            end_month = @end_month,
                            campaign_id = @campaign_id
                        WHERE result_id = @result_id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@impressions", EditedResult.impressions);
                        cmd.Parameters.AddWithValue("@clicks", EditedResult.clicks);
                        cmd.Parameters.AddWithValue("@conversions", EditedResult.conversions);
                        cmd.Parameters.AddWithValue("@lead_count", EditedResult.lead_count);
                        cmd.Parameters.AddWithValue("@roi", EditedResult.roi);
                        cmd.Parameters.AddWithValue("@cost", EditedResult.cost);
                        cmd.Parameters.AddWithValue("@end_month", EditedResult.end_month);
                        cmd.Parameters.AddWithValue("@campaign_id", EditedResult.campaign_id);
                        cmd.Parameters.AddWithValue("@result_id", EditedResult.result_id);

                        int affectedRows = cmd.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            MessageBox.Show("Результат кампании обновлён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            DialogResult = true;
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Обновление не выполнено. Запись не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении результата: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
