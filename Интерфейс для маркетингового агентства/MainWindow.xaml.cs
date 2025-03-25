using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public class Campaign
    {
        public int campaign_id { get; set; }
        public string name { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public decimal budget { get; set; }
        public string status { get; set; }
        public int client_id { get; set; }
    }


    public partial class MainWindow : Window
    {

        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";

        private List<Campaign> campaigns = new List<Campaign>();


        public MainWindow()
        {
            InitializeComponent();
            LoadCampaigns(); // Загружаем данные при запуске
        }


        private void LoadCampaigns()
        {
            campaigns.Clear(); // Очищаем список перед загрузкой

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Campaigns";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        campaigns.Add(new Campaign
                        {
                            campaign_id = reader.GetInt32(0),
                            name = reader.GetString(1),
                            start_date = reader.GetDateTime(2),
                            end_date = reader.GetDateTime(3),
                            budget = reader.GetDecimal(4),
                            status = reader.GetString(5),
                            client_id = reader.GetInt32(6)
                        });
                    }

                    // Устанавливаем источник данных для DataGrid
                    CampaignsDataGrid.ItemsSource = campaigns;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
                }
            }
        }



        private void AddCampaignButton_Click(object sender, RoutedEventArgs e)
        {
            AddCampaignWindow addCampaignWindow = new AddCampaignWindow();
            addCampaignWindow.Owner = this;

            if (addCampaignWindow.ShowDialog() == true)
            {
                // Добавляем новую кампанию в список и обновляем DataGrid
                campaigns.Add(addCampaignWindow.NewCampaign);
                CampaignsDataGrid.Items.Refresh();
            }
        }

        private void EditCampaignButton_Click(object sender, RoutedEventArgs e)
        {
            if (CampaignsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите кампанию для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем выбранную кампанию
            Campaign selectedCampaign = (Campaign)CampaignsDataGrid.SelectedItem;

            EditCampaignWindow editCampaignWindow = new EditCampaignWindow(selectedCampaign);
            editCampaignWindow.Owner = this;

           if (editCampaignWindow.ShowDialog() == true)
           {
                int index = campaigns.FindIndex(c => c.campaign_id == editCampaignWindow.EditedCampaign.campaign_id);
                if (index != -1)
                {
                    campaigns[index] = editCampaignWindow.EditedCampaign; // Полная замена объекта
                    CampaignsDataGrid.Items.Refresh();
                }
            }

        }

        private void DeleteCampaignButton_Click(object sender, RoutedEventArgs e)
        {
            if (CampaignsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите кампанию для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем выбранную кампанию
            Campaign selectedCampaign = (Campaign)CampaignsDataGrid.SelectedItem;

            // Подтверждение удаления
            MessageBoxResult result = MessageBox.Show(
                $"Вы точно хотите удалить кампанию '{selectedCampaign.name}'?",
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
                        string query = "DELETE FROM Campaigns WHERE campaign_id = @id";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", selectedCampaign.campaign_id);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                // Удаляем из локальной коллекции
                                campaigns.Remove(selectedCampaign);
                                CampaignsDataGrid.Items.Refresh();

                                MessageBox.Show("Кампания успешно удалена", "Успех",
                                              MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
