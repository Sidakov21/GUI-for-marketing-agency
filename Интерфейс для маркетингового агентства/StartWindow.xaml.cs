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

namespace Интерфейс_для_маркетингового_агентства
{
    /// <summary>
    /// Логика взаимодействия для StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        public void CampaignsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow("Кампании");
        }

        public void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow("Клиенты");
        }

        public void EmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow("Сотрудники");
        }

        public void PaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow("Платежи");
        }

        public void ChannelsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow("Каналы");
        }

        public void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow("Отчёты");
        }


        private void CounterpartiesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow("Контрагенты");
        }

        public void OpenWindow(string windowName)
        {
            Window window = null;

            switch (windowName)
            {
                case "Кампании":
                    window = new MainWindow();
                    break;
                case "Клиенты":
                    window = new ClientsWindow();
                    break;
                case "Сотрудники":
                    window = new EmployeesWindow();
                    break;
                case "Платежи":
                    window = new PaymentsWindow();
                    break;
                case "Каналы":
                    window = new ChannelsWindow();
                    break;
                case "Отчёты":
                    window = new CampaignResultsWindow();
                    break;
                case "Контрагенты":
                    window = new VendorsWindow();
                    break;
            }

            if (window != null)
            {
                window.Show();
                this.Close();
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
