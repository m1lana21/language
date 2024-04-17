using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                BirthdayDP.SelectedDate = currentClient.Birthday;
            }
            else
            {
                ComboGender.SelectedIndex = 0;
                //currentClient.ID = allClients.Max(p => p.ID) + 1;
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
            if(ComboGender.SelectedIndex == 0)
            {
                currentClient.GenderCode = "м";
            }
            else currentClient.GenderCode = "ж";
            StringBuilder errors = new StringBuilder();

            if(currentClient.PhotoPath != null)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(currentClient.PhotoPath);
                bitmap.EndInit();
                int imageSizeInBytes = bitmap.PixelWidth * bitmap.PixelHeight * (bitmap.Format.BitsPerPixel / 8);
                int maxSizeInBytes = 2 * 1024 * 1024; // 2 мегабайта
                if (imageSizeInBytes > maxSizeInBytes)
                    errors.AppendLine("Размер фото превышает 2 МБ");
            }

            if (string.IsNullOrWhiteSpace(FirstNameTB.Text))
                errors.AppendLine("Укажите имя");
            if(FirstNameTB.Text.Length > 50)
                errors.AppendLine("Имя не должно быть длиннее 50 символов");
            else
            {
                string text = FirstNameTB.Text;
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    if (!char.IsLetter(c) && c != ' ' && c != '-')
                    {
                        errors.AppendLine("Имя не должно содержать символы помимо букв, тире и пробела");
                        break;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(LastNameTB.Text))
                errors.AppendLine("Укажите фамилию");
            if (LastNameTB.Text.Length > 50)
                errors.AppendLine("Фамилия не должна быть длиннее 50 символов");
            else
            {
                string text1 = LastNameTB.Text;
                for (int i = 0; i < text1.Length; i++)
                {
                    char c = text1[i];
                    if (!char.IsLetter(c) && c != ' ' && c != '-' && LastNameTB.Text != null)
                    {
                        errors.AppendLine("Фамилия не должна содержать символы помимо букв, тире и пробела");
                        break;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(PatronymicTB.Text))
                errors.AppendLine("Укажите отчество");
            if (PatronymicTB.Text.Length > 50)
                errors.AppendLine("Отчество не должно быть длиннее 50 символов");
            else
            {
                string text2 = PatronymicTB.Text;
                for (int i = 0; i < text2.Length; i++)
                {
                    char c = text2[i];
                    if (!char.IsLetter(c) && c != ' ' && c != '-' && PatronymicTB.Text != null)
                    {
                        errors.AppendLine("Отчество не должнo содержать символы помимо букв, тире и пробела");
                        break;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(PhoneTB.Text))
                errors.AppendLine("Укажите номер телефона");
            else
            {
                Regex regex = new Regex(@"^[0-9+\-()\s]+$");
                if (!regex.IsMatch(PhoneTB.Text))
                {
                    errors.AppendLine("Номер телефона указан некорректно");
                }
            }
            
            if (string.IsNullOrWhiteSpace(EmailTB.Text))
            {
                errors.AppendLine("Укажите эл. почту");
            }
            else
            {
                Regex regex1 = new Regex(@"^[\w\.-]+@[\w\.-]+\.\w+$");
                if (!regex1.IsMatch(EmailTB.Text) && EmailTB.Text != null)
                {
                    errors.AppendLine("Адрес эл. почты указан некорректно");
                }
            }
                
            if (BirthdayDP.SelectedDate==null)
                errors.AppendLine("Укажите дату рождения");
            else currentClient.Birthday=BirthdayDP.SelectedDate;

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            else AkhmerovaLanguageEntities.GetContext().Client.Add(currentClient);
            try
            {
                AkhmerovaLanguageEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }


    }
}
