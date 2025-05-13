using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства
{

    public class Vendor
    {
        public int vendor_id { get; set; }
        public string name { get; set; }
        public string service_type { get; set; }
        public string contact_info { get; set; }
        public string contract_terms { get; set; }
    }

    public partial class VendorsWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";
        private List<Vendor> vendors = new List<Vendor>();

        public VendorsWindow()
        {
            InitializeComponent();
            LoadVendors();
        }

        private void LoadVendors()
        {
            vendors.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Vendors";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        vendors.Add(new Vendor
                        {
                            vendor_id = reader.GetInt32(0),
                            name = reader.GetString(1),
                            service_type = reader.GetString(2),
                            contact_info = reader.GetString(3),
                            contract_terms = reader.GetString(4)
                        });
                    }

                    VendorsDataGrid.ItemsSource = vendors;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
                }
            }
        }

        private void AddVendorButton_Click(object sender, RoutedEventArgs e)
        {
            AddVendorsWindow addVendorWindow = new AddVendorsWindow();
            addVendorWindow.Owner = this;

            if (addVendorWindow.ShowDialog() == true)
            {
                vendors.Add(addVendorWindow.NewVendor);
                VendorsDataGrid.Items.Refresh();
            }
        }

        private void EditVendorButton_Click(object sender, RoutedEventArgs e)
        {
            if (VendorsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите поставщика для редактирования.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Vendor selectedVendor = (Vendor)VendorsDataGrid.SelectedItem;
            EditVendorsWindow editVendorWindow = new EditVendorsWindow(selectedVendor);
            editVendorWindow.Owner = this;

            if (editVendorWindow.ShowDialog() == true)
            {
                int index = vendors.FindIndex(v => v.vendor_id == editVendorWindow.EditedVendor.vendor_id);
                if (index != -1)
                {
                    vendors[index] = editVendorWindow.EditedVendor;
                    VendorsDataGrid.Items.Refresh();
                }
            }
        }

        private void DeleteVendorButton_Click(object sender, RoutedEventArgs e)
        {
            if (VendorsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите поставщика для удаления.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Vendor selectedVendor = (Vendor)VendorsDataGrid.SelectedItem;

            MessageBoxResult result = MessageBox.Show(
                $"Вы точно хотите удалить поставщика '{selectedVendor.name}'?",
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
                        string query = "DELETE FROM Vendors WHERE vendor_id = @id";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", selectedVendor.vendor_id);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                vendors.Remove(selectedVendor);
                                VendorsDataGrid.Items.Refresh();

                                MessageBox.Show("Поставщик успешно удален", "Успех",
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var startWindow = new StartWindow();
            startWindow.Show();
            this.Close();
        }
    }
}