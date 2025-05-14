using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства.CampaignResults
{
    /// <summary>
    /// Логика взаимодействия для AddResultWindow.xaml
    /// </summary>
    /// 

    public class CampaignResult
    {
        public int result_id { get; set; }
        public int impressions { get; set; }
        public int clicks { get; set; }
        public int conversions { get; set; }
        public int lead_count { get; set; }
        public decimal roi { get; set; }
        public decimal cost { get; set; }
        public DateTime end_month { get; set; }

        public int campaign_id { get; set; }
        public string campaign_name { get; set; }
    }

    public partial class CampaignResultsWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";
        private List<CampaignResult> results = new List<CampaignResult>();

        public CampaignResultsWindow()
        {
            InitializeComponent();
            LoadResults();
        }

        private void LoadResults()
        {
            results.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT r.result_id, r.impressions, r.clicks, r.conversions, r.lead_count,
                                    r.roi, r.cost, r.end_month, r.campaign_id, c.name AS campaign_name
                             FROM Campaign_Results r
                             JOIN Campaigns c ON r.campaign_id = c.campaign_id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        results.Add(new CampaignResult
                        {
                            result_id = reader.GetInt32(0),
                            impressions = reader.GetInt32(1),
                            clicks = reader.GetInt32(2),
                            conversions = reader.GetInt32(3),
                            lead_count = reader.GetInt32(4),
                            roi = reader.GetDecimal(5),
                            cost = reader.GetDecimal(6),
                            end_month = reader.GetDateTime(7),
                            campaign_id = reader.GetInt32(8),
                            campaign_name = reader.GetString(9)
                        });
                    }

                    ResultsDataGrid.ItemsSource = null;
                    ResultsDataGrid.ItemsSource = results;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки результатов: " + ex.Message);
                }
            }
        }


        private void AddResultButton_Click(object sender, RoutedEventArgs e)
        {
            AddResultWindow addWindow = new AddResultWindow();
            if (addWindow.ShowDialog() == true)
            {
                results.Add(addWindow.NewResult);
                LoadResults();
            }
        }

        private void EditResultButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResultsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите результат для редактирования.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CampaignResult selectedResult = (CampaignResult)ResultsDataGrid.SelectedItem;
            EditResultWindow editWindow = new EditResultWindow(selectedResult);
            if (editWindow.ShowDialog() == true)
            {
                int index = results.FindIndex(r => r.result_id == editWindow.EditedResult.result_id);
                if (index != -1)
                {
                    results[index] = editWindow.EditedResult;
                    LoadResults();
                }
            }
        }

        private void DeleteResultButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResultsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите результат для удаления.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CampaignResult selectedResult = (CampaignResult)ResultsDataGrid.SelectedItem;

            MessageBoxResult result = MessageBox.Show(
                $"Вы точно хотите удалить результат №{selectedResult.result_id}?",
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
                        string query = "DELETE FROM Campaign_Results WHERE result_id = @id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", selectedResult.result_id);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            results.Remove(selectedResult);
                            LoadResults();
                            MessageBox.Show("Результат успешно удален.", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении результата: {ex.Message}", "Ошибка",
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
