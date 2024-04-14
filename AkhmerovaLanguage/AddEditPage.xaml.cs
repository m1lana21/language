using Microsoft.Win32;
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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Client currentClient = new Client();
        public AddEditPage(Client SelectedClient)
        {
            InitializeComponent();
            var allClients = AkhmerovaLanguageEntities.GetContext().Client.ToList();
            if (SelectedClient != null)
            {
                this.currentClient = SelectedClient;
                if (currentClient.GenderCode == "м") ComboGender.SelectedIndex = 0;
                else ComboGender.SelectedIndex = 1;
                IDTextBox.IsReadOnly = true;
            }
            else
            {
                ComboGender.SelectedIndex = 0;
                IDTextBox.Visibility = Visibility.Hidden;
                IDTextBlock.Visibility = Visibility.Hidden;
                currentClient.RegistrationDate = DateTime.Now;
            }


            DataContext = currentClient;
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                currentClient.PhotoPath = myOpenFileDialog.FileName;
                ClientPhoto.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(currentClient.PhotoPath);
            bitmap.EndInit();
            int imageSizeInBytes = bitmap.PixelWidth * bitmap.PixelHeight * (bitmap.Format.BitsPerPixel / 8);
            int maxSizeInBytes = 2 * 1024 * 1024; // 2 мегабайта
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(FirstNameTB.Text))
                errors.AppendLine("Укажите имя");
            if(FirstNameTB.Text.Length > 50)
                errors.AppendLine("Имя не должно быть длиннее 50 символов");
            if (string.IsNullOrWhiteSpace(LastNameTB.Text))
                errors.AppendLine("Укажите фамилию");
            if (LastNameTB.Text.Length > 50)
                errors.AppendLine("Фамилия не должна быть длиннее 50 символов");
            if (string.IsNullOrWhiteSpace(PatronymicTB.Text))
                errors.AppendLine("Укажите отчество");
            if (PatronymicTB.Text.Length > 50)
                errors.AppendLine("Отчество не должно быть длиннее 50 символов");
            if (string.IsNullOrWhiteSpace(PhoneTB.Text))
                errors.AppendLine("Укажите номер телефона");
            if (string.IsNullOrWhiteSpace(EmailTB.Text))
                errors.AppendLine("Укажите эл. почту");
            if (string.IsNullOrWhiteSpace(PatronymicTB.Text))
                errors.AppendLine("Укажите отчество");
            if(imageSizeInBytes> maxSizeInBytes)
                errors.AppendLine("Размер фото превышает 2 МБ");
        }

        
    }
}
