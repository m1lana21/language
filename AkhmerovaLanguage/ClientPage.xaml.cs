using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace AkhmerovaLanguage
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;
        
        List<Client> CurrentPageList = new List<Client>();
        List<Client> TableList;
        public ClientPage()
        {
            InitializeComponent();
            var currentClient = AkhmerovaLanguageEntities.GetContext().Client.ToList();
            TBAllRecords.Text = " из " + currentClient.Count.ToString();
            ClientListView.ItemsSource = currentClient;
            RecordsOnPageBox.SelectedIndex = 0;
            FilterBox.SelectedIndex = 0;
            SortBox.SelectedIndex = 0;
            var currentClientService = AkhmerovaLanguageEntities.GetContext().ClientService.ToList();
            UpdateClients();
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
                UpdateClients();
            }
        }

        public void UpdateClients()
        {
            var currentClient = AkhmerovaLanguageEntities.GetContext().Client.ToList();
            currentClient = currentClient.Where(p => p.LastName.ToLower().Contains(SearchTextBox.Text.ToLower()) 
            || p.FirstName.ToLower().Contains(SearchTextBox.Text.ToLower()) 
            || p.Patronymic.ToLower().Contains(SearchTextBox.Text.ToLower())
            || p.Phone.ToLower().Contains(SearchTextBox.Text.ToLower())
            || p.Email.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();

            if(FilterBox.SelectedIndex==0) ClientListView.ItemsSource = currentClient;
            else if (FilterBox.SelectedIndex == 1) currentClient = currentClient.Where(p => p.GenderCode == "м").ToList();
            else if (FilterBox.SelectedIndex == 2) currentClient = currentClient.Where(p => p.GenderCode == "ж").ToList();

            if (SortBox.SelectedIndex == 0) ClientListView.ItemsSource = currentClient;
            else if (SortBox.SelectedIndex == 1) currentClient = currentClient.OrderBy(p => p.LastName).ToList();
            else if (SortBox.SelectedIndex == 2) currentClient = currentClient.OrderByDescending(p => p.MaxDate2).ToList();
            else if (SortBox.SelectedIndex == 3) currentClient = currentClient.OrderByDescending(p => p.arrivalcount).ToList();

            TableList = currentClient;
            ChangePage(0, 0);
            ClientListView.ItemsSource = currentClient;
            //TBCount.Text = min.ToString();
            TBCount.Text = currentClient.Count.ToString();
        }

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;
            int RecordsOnPage;
            switch (RecordsOnPageBox.SelectedIndex)
            {
                case 0:
                    RecordsOnPage = 10;
                break;
                case 1:
                    RecordsOnPage = 50;
                break;
                case 2:
                    RecordsOnPage = 200;
                break;
                case 3:
                    RecordsOnPage = CountRecords;
                break;
                default: RecordsOnPage = 10; 
                break;
            }
            
            
            if (CountRecords % RecordsOnPage > 0)
            {
                CountPage = CountRecords / RecordsOnPage + 1;
            }
            else
            {
                CountPage = CountRecords / RecordsOnPage;
            }

            Boolean Ifupdate = true;

            int min;
            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * RecordsOnPage + RecordsOnPage < CountRecords ? CurrentPage * RecordsOnPage + RecordsOnPage : CountRecords;
                    for (int i = CurrentPage * RecordsOnPage; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * RecordsOnPage + RecordsOnPage < CountRecords ? CurrentPage * RecordsOnPage + RecordsOnPage : CountRecords;
                            for (int i = CurrentPage * RecordsOnPage; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }

                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;

                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * RecordsOnPage + RecordsOnPage < CountRecords ? CurrentPage * RecordsOnPage + RecordsOnPage : CountRecords;
                            for (int i = CurrentPage * RecordsOnPage; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }


                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;


                }

            }
            if (Ifupdate)
            {
                PageListBox.Items.Clear();

                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;


                min = CurrentPage * RecordsOnPage + RecordsOnPage < CountRecords ? CurrentPage * RecordsOnPage + RecordsOnPage : CountRecords;
                
                //TBAllRecords.Text = " из " + CountRecords.ToString();

                ClientListView.ItemsSource = CurrentPageList;
                ClientListView.Items.Refresh();
                //RecordsCount.Text = min.ToString();

            }
            //UpdateClients();
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RecordsOnPageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClients();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void FilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }
    }
}
