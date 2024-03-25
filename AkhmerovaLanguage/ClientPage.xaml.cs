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

namespace AkhmerovaLanguage
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        public ClientPage()
        {
            InitializeComponent();
            var currentClient = AkhmerovaLanguageEntities.GetContext().Client.ToList();
            ClientListView.ItemsSource = currentClient;
            RecordsMax.Text = currentClient.Count.ToString();
            
            var currentClientService = AkhmerovaLanguageEntities.GetContext().ClientService.ToList();
            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var currentClientService = AkhmerovaLanguageEntities.GetContext().ClientService.ToList();
            var currentClient = ClientListView.SelectedItem as Client;
            var currentVisits = currentClientService.Where(p => p.ClientID == currentClient.ID).ToList();
            if (currentVisits.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, так как существует информация о посещениях");

            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        AkhmerovaLanguageEntities.GetContext().Client.Remove(currentClient);
                        AkhmerovaLanguageEntities.GetContext().SaveChanges();
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        
    }
}
