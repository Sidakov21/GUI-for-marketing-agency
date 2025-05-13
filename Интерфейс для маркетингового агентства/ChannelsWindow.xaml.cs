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
    /// Логика взаимодействия для ChannelsWindow.xaml
    /// </summary>
    /// 

    public class Channel
    {
        public int channel_id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public partial class ChannelsWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";
        private List<Channel> channels = new List<Channel>();

        public ChannelsWindow()
        {
            InitializeComponent();
            LoadChannels();
        }

        private void LoadChannels()
        {
            channels.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Channels";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        channels.Add(new Channel
                        {
                            channel_id = reader.GetInt32(0),
                            name = reader.GetString(1),
                            type = reader.GetString(2)
                        });
                    }

                    ChannelsDataGrid.ItemsSource = channels;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки каналов: " + ex.Message);
                }
            }
        }

        private void AddChannelButton_Click(object sender, RoutedEventArgs e)
        {
            AddChannelsWindow addWindow = new AddChannelsWindow();
            if (addWindow.ShowDialog() == true)
            {
                channels.Add(addWindow.NewChannel);
                ChannelsDataGrid.Items.Refresh();
            }
        }

        private void EditChannelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChannelsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите канал для редактирования.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Channel selectedChannel = (Channel)ChannelsDataGrid.SelectedItem;
            EditChannelsWindow editWindow = new EditChannelsWindow(selectedChannel);
            if (editWindow.ShowDialog() == true)
            {
                int index = channels.FindIndex(c => c.channel_id == editWindow.EditedChannel.channel_id);
                if (index != -1)
                {
                    channels[index] = editWindow.EditedChannel;
                    ChannelsDataGrid.Items.Refresh();
                }
            }
        }

        private void DeleteChannelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChannelsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите канал для удаления.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Channel selectedChannel = (Channel)ChannelsDataGrid.SelectedItem;

            MessageBoxResult result = MessageBox.Show(
                $"Вы точно хотите удалить канал '{selectedChannel.name}'?",
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
                        string query = "DELETE FROM Channels WHERE channel_id = @id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", selectedChannel.channel_id);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            channels.Remove(selectedChannel);
                            ChannelsDataGrid.Items.Refresh();
                            MessageBox.Show("Канал успешно удален.", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении канала: {ex.Message}", "Ошибка",
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
