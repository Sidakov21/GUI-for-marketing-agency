using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства.Payments
{
    public class Payment
    {
        public int payment_id { get; set; }
        public decimal amount { get; set; }
        public DateTime payment_data { get; set; }
        public string payment_type { get; set; }

        public int campaign_id { get; set; }
        public string campaign_name { get; set; }
    }

    public partial class PaymentsWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";
        private List<Payment> payments = new List<Payment>();

        public PaymentsWindow()
        {
            InitializeComponent();
            LoadPayments();
        }

        private void LoadPayments()
        {
            payments.Clear(); 

            PaymentsDataGrid.ItemsSource = null;
            PaymentsDataGrid.ItemsSource = payments;


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                                SELECT p.payment_id, p.amount, p.payment_data, p.payment_type,
                                        p.campaign_id, c.name AS campaign_name
                                FROM Payments p
                                JOIN Campaigns c ON p.campaign_id = c.campaign_id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        payments.Add(new Payment
                        {
                            payment_id = reader.GetInt32(0),
                            amount = reader.GetDecimal(1),
                            payment_data = reader.GetDateTime(2),
                            payment_type = reader.GetString(3),
                            campaign_id = reader.GetInt32(4),
                            campaign_name = reader.GetString(5)
                        });
                    }

                    PaymentsDataGrid.ItemsSource = payments;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки платежей: " + ex.Message);
                }
            }
        }

        private void AddPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            AddPaymentsWindow addPaymentWindow = new AddPaymentsWindow();
            addPaymentWindow.Owner = this;

            if (addPaymentWindow.ShowDialog() == true)
            {
                payments.Add(addPaymentWindow.NewPayment);
                LoadPayments();
            }
        }

        private void EditPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (PaymentsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите платеж для редактирования.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Payment selectedPayment = (Payment)PaymentsDataGrid.SelectedItem;

            EditPaymentsWindow editPaymentWindow = new EditPaymentsWindow(selectedPayment);
            editPaymentWindow.Owner = this;

            if (editPaymentWindow.ShowDialog() == true)
            {
                int index = payments.FindIndex(p => p.payment_id == editPaymentWindow.EditedPayment.payment_id);
                if (index != -1)
                {
                    payments[index] = editPaymentWindow.EditedPayment;
                }
                LoadPayments();
            }
        }

        private void DeletePaymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (PaymentsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите платеж для удаления.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Payment selectedPayment = (Payment)PaymentsDataGrid.SelectedItem;

            MessageBoxResult result = MessageBox.Show(
                $"Вы точно хотите удалить платеж №{selectedPayment.payment_id}?",
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
                        string query = "DELETE FROM Payments WHERE payment_id = @id";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", selectedPayment.payment_id);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                payments.Remove(selectedPayment);
                                LoadPayments();

                                MessageBox.Show("Платеж успешно удален", "Успех",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении платежа: {ex.Message}", "Ошибка",
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