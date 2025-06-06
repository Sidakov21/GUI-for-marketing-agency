﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    /// Логика взаимодействия для EditCampaignWindow.xaml
    /// </summary>
    public partial class EditCampaignWindow : Window
    {

        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";
        private Campaign _selectedCampaign;

        public Campaign EditedCampaign { get; private set; }

        public EditCampaignWindow(Campaign campaign)
        {
            InitializeComponent();
            _selectedCampaign = campaign;
            LoadCampaignData(); 
            LoadClients();
        }

        private void LoadCampaignData()
        {
            NameTextBox.Text = _selectedCampaign.name;
            StartDatePicker.SelectedDate = _selectedCampaign.start_date;
            EndDatePicker.SelectedDate = _selectedCampaign.end_date;
            BudgetTextBox.Text = _selectedCampaign.budget.ToString();
            ClientComboBox.SelectedValue = _selectedCampaign.client_id;

            foreach (ComboBoxItem item in StatusComboBox.Items)
            {
                if (item.Content.ToString() == _selectedCampaign.status)
                {
                    StatusComboBox.SelectedItem = item;
                    break;
                }
            }
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
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

            EditedCampaign = new Campaign
            {
                campaign_id = _selectedCampaign.campaign_id,
                name = NameTextBox.Text,
                start_date = StartDatePicker.SelectedDate.Value,
                end_date = EndDatePicker.SelectedDate.Value,
                budget = Convert.ToDecimal(BudgetTextBox.Text),
                status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                client_id = (int)ClientComboBox.SelectedValue
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        UPDATE Campaigns 
                        SET name = @name,
                            start_date = @start_date,
                            end_date = @end_date,
                            budget = @budget,
                            status = @status,
                            client_id = @client_id
                        WHERE campaign_id = @campaign_id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", EditedCampaign.name);
                    cmd.Parameters.AddWithValue("@start_date", EditedCampaign.start_date);
                    cmd.Parameters.AddWithValue("@end_date", EditedCampaign.end_date);
                    cmd.Parameters.AddWithValue("@budget", EditedCampaign.budget);
                    cmd.Parameters.AddWithValue("@status", EditedCampaign.status);
                    cmd.Parameters.AddWithValue("@client_id", EditedCampaign.client_id);
                    cmd.Parameters.AddWithValue("@campaign_id", EditedCampaign.campaign_id);

                    cmd.ExecuteNonQuery();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении кампании: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
