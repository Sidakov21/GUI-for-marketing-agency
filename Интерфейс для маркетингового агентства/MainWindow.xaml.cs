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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Интерфейс_для_маркетингового_агентства
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public class Campaign
    {
        public int CampaignId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public string Status { get; set; }
    }


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddCampaignButton_Click(object sender, RoutedEventArgs e)
        {
            AddCampaignWindow addCampaignWindow = new AddCampaignWindow();

            addCampaignWindow.Owner = this; 
            addCampaignWindow.ShowDialog();
        }

        private void EditCampaignButton_Click(object sender, RoutedEventArgs e)
        {
            if (CampaignsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите кампанию для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            EditCampaignWindow addCampaignWindow = new EditCampaignWindow();

            addCampaignWindow.Owner = this;
            addCampaignWindow.ShowDialog();
        }

        private void DeleteCampaignButton_Click(object sender, RoutedEventArgs e)
        {
            if (CampaignsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите кампанию для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

        }
    }
}
