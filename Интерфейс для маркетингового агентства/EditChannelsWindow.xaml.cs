using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace Интерфейс_для_маркетингового_агентства
{
    /// <summary>
    /// Логика взаимодействия для EditChannelsWindow.xaml
    /// </summary>
    public partial class EditChannelsWindow : Window
    {
        private string connectionString = "Server=ASUSVIVOBOOK15;Database=База данных для маркетингового агентства;Trusted_Connection=True;TrustServerCertificate=True;";
        private Channel _selectedChannel;

        public Channel EditedChannel { get; private set; }

        public EditChannelsWindow(Channel channel)
        {
            InitializeComponent();
            _selectedChannel = channel;
            LoadChannelData();
        }

        private void LoadChannelData()
        {
            NameTextBox.Text = _selectedChannel.name;

            foreach (ComboBoxItem item in TypeComboBox.Items)
            {
                if (item.Content.ToString() == _selectedChannel.type)
                {
                    TypeComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || TypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditedChannel = new Channel
            {
                channel_id = _selectedChannel.channel_id,
                name = NameTextBox.Text.Trim(),
                type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString()
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        UPDATE Channels
                        SET name = @name,
                            type = @type
                        WHERE channel_id = @channel_id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", EditedChannel.name);
                    cmd.Parameters.AddWithValue("@type", EditedChannel.type);
                    cmd.Parameters.AddWithValue("@channel_id", EditedChannel.channel_id);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Канал успешно обновлён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении канала: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
