using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства
{
    public class Payment
    {
        public int payment_id { get; set; }
        public decimal amount { get; set; }
        public DateTime payment_data { get; set; }
        public string payment_type { get; set; }
        public int campaign_id { get; set; }
    }

    public partial class PaymentsWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";
        private List<Payment> payments = new List<Payment>();

        public PaymentsWindow()
        {
            InitializeComponent();
            LoadPayments(); // Загружаем данные при запуске
        }

        private void LoadPayments()
        {
            payments.Clear(); // Очищаем список перед загрузкой

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Payments";
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
                            campaign_id = reader.GetInt32(4)
                        });
                    }

                    // Устанавливаем источник данных для DataGrid
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
                // Добавляем новый платеж в список и обновляем DataGrid
                payments.Add(addPaymentWindow.NewPayment);
                PaymentsDataGrid.Items.Refresh();
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

            // Получаем выбранный платеж
            Payment selectedPayment = (Payment)PaymentsDataGrid.SelectedItem;

            EditPaymentsWindow editPaymentWindow = new EditPaymentsWindow(selectedPayment);
            editPaymentWindow.Owner = this;

            if (editPaymentWindow.ShowDialog() == true)
            {
                int index = payments.FindIndex(p => p.payment_id == editPaymentWindow.EditedPayment.payment_id);
                if (index != -1)
                {
                    payments[index] = editPaymentWindow.EditedPayment;
                    PaymentsDataGrid.Items.Refresh();
                }
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

            // Получаем выбранный платеж
            Payment selectedPayment = (Payment)PaymentsDataGrid.SelectedItem;

            // Подтверждение удаления
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
                                // Удаляем из локальной коллекции
                                payments.Remove(selectedPayment);
                                PaymentsDataGrid.Items.Refresh();

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