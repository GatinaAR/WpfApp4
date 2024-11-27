using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using System.Windows.Shapes;

namespace WpfApp4.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditDriver.xaml
    /// </summary>
    public partial class EditDriver : Window
    {
        private string PhotoFileName { get; set; }

        List<string> cities = new List<string>();
        private driver _currentDriver = new driver();
        public byte[] imageData = null;
        public EditDriver()
        {
            InitializeComponent();
            foreach (var city in upEntities.GetContext().driver.ToList().Distinct())
            {
                if (!string.IsNullOrEmpty(city.address_reg_city))
                {
                    cities.Add(city.address_reg_city);
                }
            }
            life_city.ItemsSource = cities;
            reg_city.ItemsSource = cities;
            Add.Visibility = Visibility.Visible;
        }
        public EditDriver(driver driver)
        {
            InitializeComponent();
            Edit.Visibility = Visibility.Visible;
            if (driver.photo != null)
            {
                BitmapImage bitmap = new BitmapImage(new Uri("C:/Users/Анжела/source/repos/WpfApp4/WpfApp4/img/" + driver.photo));
                if (bitmap != null)
                {
                    BlurRadius.Radius = 0;
                    ava.Source = bitmap;
                }

            }
            foreach (var city in upEntities.GetContext().driver.ToList().Distinct())
            {
                cities.Add(city.address_reg_city);
            }
            reg_city.ItemsSource = cities;

            if (driver == null) return;
            _currentDriver = driver;
            DataContext = _currentDriver;
        }
        public static bool IsValidEmail(string email)
        {

            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        //public static bool IsValidPhone(string phone)
        //{ 
        //if (string.IsNullOrWhiteSpace(phone))
        //    return false;

        //string pattern = @"^1̣0̣"; // Пример для 10-значного номера
        //return Regex.IsMatch(phone, pattern);
        //}
        private void addPhoto(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
                Title = "Выберите фото"
            };

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                string fileName = System.IO.Path.GetFileName(selectedFilePath);
                string targetDirectory = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\Анжела\\source\\repos\\WpfApp4\\WpfApp4\\img\\");
                string targetFilePath = System.IO.Path.Combine(targetDirectory, fileName);

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                //

                File.Copy(selectedFilePath, targetFilePath, true);
                string fileNameForDatabase = fileName;
                PhotoFileName = fileNameForDatabase;

                photo.IsEnabled = true;
                photo.Text = PhotoFileName;
            }
        }
        private void Back(object sender, RoutedEventArgs e)
        {
            DriverWindow mainWindow = new DriverWindow();
            mainWindow.Show();
            this.Close();
        }
        //IsValidEmail(email.Text)&&
        private void AddDriver(object sender, RoutedEventArgs e)
        {

            using (upEntities context = new upEntities())
            {

                //MessageBoxResult result = MessageBox.Show("Вы уверены, что не хотите выбрать фото?","Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question);
                //if (result == MessageBoxResult.Yes)
                //{
                if (IsValidEmail(email.Text) && !string.IsNullOrEmpty(phone.Text) && !string.IsNullOrEmpty
                (phone.Text) && !string.IsNullOrEmpty(name.Text) && !string.IsNullOrEmpty(middle.Text)
                && !string.IsNullOrEmpty(company.Text) && !string.IsNullOrEmpty(jobname.Text) &&
                !string.IsNullOrEmpty(serial.Text) && !string.IsNullOrEmpty(number.Text) &&
                !string.IsNullOrEmpty(reg.Text) &&
                !string.IsNullOrEmpty(life.Text))
                {
                    driver d = new driver
                    {
                        name = name.Text,
                        middlename = middle.Text,
                        passport_serial = serial.Text,
                        passport_number = number.Text,
                        address_life = life.Text,
                        address_life_city = life_city.SelectedItem as string,
                        address_reg_city = reg_city.SelectedItem as string,
                        company = company.Text,
                        jobname = jobname.Text,
                        address_reg = reg.Text,
                        phone = phone.Text,
                        email = email.Text,
                        description = description.Text,
                        photo = PhotoFileName
                    };
                    context.driver.Add(d);
                    context.SaveChanges();
                    MessageBox.Show("Водитель добавлен");
                    DriverWindow mainWindow = new DriverWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не все поля ввода заполнены правильно!");
                }

                //}
                //else
                //{
                //    MessageBox.Show("Выберите фото.");
                //}
                // if (selectedFilePath != null && targetFilePath!=null)
                //{

                //}

            }

        }
        private void Editdriver(object sender, RoutedEventArgs e)
        {

            //ava.Source=
            upEntities.GetContext().SaveChanges();
            MessageBox.Show("Водитель изменен");
            DriverWindow mainWindow = new DriverWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
