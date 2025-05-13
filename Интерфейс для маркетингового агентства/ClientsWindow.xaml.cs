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


namespace Интерфейс_для_маркетингового_агентства
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public class Client
    {
        public int client_id { get; set; }
        public string name { get; set; }
        public string contact_info { get; set; }
        public string industry { get; set; }
        public DateTime contract_date { get; set; }
        public string contract_status { get; set; }
    }


    public partial class ClientsWindow : Window
    {

        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";

        private List<Client> clients = new List<Client>();


        public ClientsWindow()
        {
            InitializeComponent();
            LoadClients(); // Загружаем данные при запуске
        }


        private void LoadClients()
        {
            clients.Clear(); // Очищаем список перед загрузкой

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Clients";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        clients.Add(new Client
                        {
                            client_id = reader.GetInt32(0),
                            name = reader.GetString(1),
                            contact_info = reader.GetString(2),
                            industry = reader.GetString(3),
                            contract_date = reader.GetDateTime(4),
                            contract_status = reader.GetString(5)
                        });
                    }

                    // Устанавливаем источник данных для DataGrid
                    ClientDataGrid.ItemsSource = clients;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
                }
            }
        }



        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            AddClientWindow addClientWindow = new AddClientWindow();
            addClientWindow.Owner = this;

            if (addClientWindow.ShowDialog() == true)
            {
                clients.Add(addClientWindow.NewClient);
                ClientDataGrid.Items.Refresh();
            }
        }

        private void EditClientButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите кампанию для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Client selectedClient = (Client)ClientDataGrid.SelectedItem;

            EditClientWindow editClientWindow = new EditClientWindow(selectedClient);
            editClientWindow.Owner = this;

            if (editClientWindow.ShowDialog() == true)
            {
                int index = clients.FindIndex(c => c.client_id == editClientWindow.EditedClient.client_id);
                if (index != -1)
                {
                    clients[index] = editClientWindow.EditedClient; // Полная замена объекта
                    ClientDataGrid.Items.Refresh();
                }
            }

        }

        private void DeleteClientButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите кампанию для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем выбранную кампанию
            Client selectedClient = (Client)ClientDataGrid.SelectedItem;

            // Подтверждение удаления
            MessageBoxResult result = MessageBox.Show(
                $"Вы точно хотите удалить кампанию '{selectedClient.name}'?",
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
                        string query = "DELETE FROM Clients WHERE client_id = @id";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", selectedClient.client_id);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                // Удаляем из локальной коллекции
                                clients.Remove(selectedClient);
                                ClientDataGrid.Items.Refresh();

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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var startWindow = new StartWindow();
            startWindow.Show();

            this.Close();
        }
    }
}
