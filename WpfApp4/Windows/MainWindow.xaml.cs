using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp4.Windows;

namespace WpfApp4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<licence> licenses = new List<licence>();
        private List<licence> licenses2 = new List<licence>();
        private int selectedYear; // Выбранный год для фильтрации
        int id_dr;
         licence selectedLicense;
        int licenceId;
        public PlotModel ChartModel
        {
            get; private set;
        }

        public MainWindow()
        {
            InitializeComponent();
            LoadYears();
            LoadLicenses();
            LoadHistory();



        }

        private void LoadYears()
        {
            // Добавляем годы в ComboBox
            for (int year = System.DateTime.Now.Year; year >= 2000; year--) // Пример: от 2000 до текущего года
            {
                YearComboBox.Items.Add(year);
            }
            YearComboBox.SelectedItem = DateTime.Now.Year; // Устанавливаем текущий год по умолчанию
        }

        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YearComboBox.SelectedItem != null)
            {
                selectedYear = (int)YearComboBox.SelectedItem;
                LoadStatistics(); // Загружаем статистику по выбранному году
            }
        }

        private void LoadStatistics()
        {
            using (upEntities context = new upEntities())
            {
                var statistics = context.licence
                    .Where(l => l.licence_date.HasValue && l.licence_date.Value.Year == selectedYear) // Проверяем на null и фильтруем по году
                    .GroupBy(l => l.licence_date.Value.Month) // Группируем по месяцу
                    .Select(g => new
                    {
                        Month = g.Key, // Номер месяца
                        Count = g.Count() // Количество лицензий за месяц
                    })
                    .ToList();

               spisokDataGrid.ItemsSource = statistics; // Устанавливаем источник данных для DataGrid  
            }
        }



        private void LoadLicenses()
        {
            using (upEntities context = new upEntities())
            {
                var licenses = context.licence.ToList(); // Загрузка всех лицензий из базы данных
                LicenseDataGrid.ItemsSource = licenses; // Установка источника данных для DataGrid
                LicenseDataGrid1.ItemsSource = licenses; // Установка источника данных для второго DataGrid
            }
            
            }

        private void LoadHistory()
        {
            using (upEntities context = new upEntities())
            {
                var hist = context.history_hanges_statuses.ToList(); // Загрузка всех лицензий из базы данных
                HistoryDataGrid.ItemsSource = hist; // Установка источника данных для DataGrid
               
            }

        }



        private void LicenseDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LicenseDataGrid1.SelectedItem is licence selectedLicence)
            {
                // Получаем ID выбранной лицензии
                 licenceId = selectedLicence.id_licence;

                // Здесь вы можете использовать licenceId по своему усмотрению
                MessageBox.Show($"Выбранный ID лицензии: {licenceId}");
            }

        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            using (upEntities context = new upEntities())
            {

                // Находим лицензию в базе данных по её идентификатору
                var licenseToUpdate = context.licence.FirstOrDefault(l => l.id_licence == licenceId);
                if (licenseToUpdate != null)
                {
                    // Обновляем статус лицензии
                    licenseToUpdate.id_status = StatusComboBox1.SelectedIndex + 1;

                    // Создаем запись об изменении статуса
                    var statusChange = new history_hanges_statuses
                    {
                        date_time_change = DateTime.Now,
                        id_status = licenseToUpdate.id_status,
                        id_licence = licenseToUpdate.id_licence,
                        // comment = CommentTextBox.Text // Добавляем комментарий, если нужно
                    };

                    // Добавляем запись об изменении статуса в контекст
                    context.history_hanges_statuses.Add(statusChange);

                    // Сохраняем изменения в базе данных
                    int changes = context.SaveChanges();

                    // Проверяем, были ли изменения сохранены
                    if (changes > 0)
                    {
                        MessageBox.Show("Изменения успешно сохранены.");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось сохранить изменения.");
                    }

                    // Обновляем DataGrid
                    LicenseDataGrid.ItemsSource = context.licence.ToList(); // Загрузка обновленных данных


                }
            }
        }






    private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (upEntities context = new upEntities())
            {
                var driver = context.driver.FirstOrDefault(r => r.middlename == DriverName.Text);

                // Проверяем, найден ли объект
                if (driver != null)
                {
                    id_dr = driver.id; // Присваиваем id найденного объекта
                }
                 else if (driver == null)
                        {

                    // Если водитель не найден, открываем окно для добавления водителя
                    // AddDriverWindow addDriverWindow = new AddDriverWindow();
                    //  addDriverWindow.ShowDialog(); // Используем ShowDialog для модального окна
                    MessageBox.Show("Водитель не найден в системе.");
                    }

                var license = new licence
                {
                    licence_date = DateTime.Parse(IssueDate.Text),
                    expire_date = DateTime.Parse(ExpirationDate.Text),
                    categories = VehicleCategories.Text,
                    licence_series = DriverLicenseNumber.Text,
                    licence_number = DriverLicenseNumber2.Text,
                    Id_driver = id_dr,
                    id_status = StatusComboBox.SelectedIndex+1,
                };

                context.licence.Add(license);

                // Сохранение изменений в базе данных
                context.SaveChanges();

                LicenseDataGrid.Items.Refresh();

                // Генерация макета для печати
                GeneratePrintLayout(license);

                MessageBox.Show("Данные ВУ сохранены.");
            }
        
    }

        private void GeneratePrintLayout(licence license)
        {
            // Логика генерации макета для печати
            // Например, создание изображения на основе шаблона
            var renderTargetBitmap = new RenderTargetBitmap(800, 600, 96d, 96d, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(this); // Здесь нужно отрисовать нужный элемент

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            //using (var fileStream = new FileStream("C:\\Users\\Анжела\\source\\repos\\WpfApp4\\WpfApp4\\img\\driver_license_template.jpg", FileMode.Create))
            //{
            //    encoder.Save(fileStream);
            //}
        }


public SolidColorBrush StatusColor
        {
            get
            {
                // Предполагаем, что StatusComboBox - это ComboBox.
                string selectedValue = StatusComboBox.SelectedItem as string; // Или используйте SelectedValue, если у вас настроено

                switch (selectedValue)
                {
                    case "Активен":
                        return Brushes.Green;
                    case "Приостановлен":
                        return Brushes.Red;
                    case "Утратил силу":
                        return Brushes.Gray;
                    case "Изъят":
                        return Brushes.Red;
                    default:
                        return Brushes.Transparent;
                }
            }
        }


        private void GenerateTemplate_Click(object sender, RoutedEventArgs e)
        {
            // Генерация макета для печати по шаблону.
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(800, 450, 96d, 96d, System.Windows.Media.PixelFormats.Pbgra32);
            renderBitmap.Render(this);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = "DriverLicense",
                DefaultExt = ".jpeg",
                Filter = "JPEG Files (*.jpeg)|*.jpeg"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var fileStream = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create))
                {
                    BitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                    encoder.Save(fileStream);
                }
                MessageBox.Show("Макет успешно сохранен.");
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            DriverWindow mainWindow = new DriverWindow();
            mainWindow.Show();
            this.Close();
        }
        private driver FindDriver(string lastName, string firstName)
        {
            using (upEntities context = new upEntities())
            {
                return context.driver
                    .FirstOrDefault(d => d.middlename == lastName && d.name == firstName);
            }
        }

        private void TabItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WindowCar car= new WindowCar();
            car.Show();
        }
    }
}
