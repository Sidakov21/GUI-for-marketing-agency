using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
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


namespace Интерфейс_для_маркетингового_агентства.Campaigns
{
    /// <summary>
    /// Логика взаимодействия для AddCampaignWindow.xaml
    /// </summary>
    /// 

    public partial class AddCampaignWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";

        public Campaign NewCampaign { get; set; }


        public AddCampaignWindow()
        {
            InitializeComponent();
            LoadClients();
        }


        private void LoadClients()
        {
            List<Clients.Client> clients = new List<Clients.Client>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT client_id, name FROM Clients";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clients.Add(new Clients.Client
                        {
                            client_id = reader.GetInt32(0),
                            name = reader.GetString(1)
                        });
                    }
                }
            }

            ClientComboBox.ItemsSource = clients;
        }


        private bool IsValidDecimal(string input)
        {
            return decimal.TryParse(input,
                   NumberStyles.Any,
                   CultureInfo.InvariantCulture,
                   out _);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                StartDatePicker.SelectedDate == null ||
                EndDatePicker.SelectedDate == null ||
                string.IsNullOrWhiteSpace(BudgetTextBox.Text) ||
                StatusComboBox.SelectedItem == null ||
                ClientComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (StartDatePicker.SelectedDate > EndDatePicker.SelectedDate)
            {
                MessageBox.Show("Дата начала не может быть больше даты завершения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidDecimal(BudgetTextBox.Text) ||
                !decimal.TryParse(BudgetTextBox.Text, out decimal budgetValue))
            {
                MessageBox.Show("Бюджет должен содержать только цифры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = NameTextBox.Text;
            DateTime? start_date = StartDatePicker.SelectedDate;
            DateTime? end_date = EndDatePicker.SelectedDate;
            decimal budget = Convert.ToDecimal(BudgetTextBox.Text);
            string status = StatusComboBox.Text;
            int client_id = (int)ClientComboBox.SelectedValue;


            NewCampaign = new Campaign
            {
                name = NameTextBox.Text,
                start_date = StartDatePicker.SelectedDate.Value,
                end_date = EndDatePicker.SelectedDate.Value,
                budget = budget,
                status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                client_id = client_id
            };

            string query = "INSERT INTO Campaigns (name, start_date, end_date, budget, status, client_id) VALUES (@name, @start_date, @end_date, @budget, @status, @client_id) SELECT SCOPE_IDENTITY();";   // Получаем ID новой записи

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@start_date", start_date);
                        cmd.Parameters.AddWithValue("@end_date", end_date);
                        cmd.Parameters.AddWithValue("@budget", budget);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@client_id", client_id);

                        NewCampaign.campaign_id = Convert.ToInt32(cmd.ExecuteScalar());

                        MessageBox.Show("Кампания успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        this.DialogResult = true;
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении кампании: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
