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
using WpfApp4.Windows;

namespace WpfApp4
{
    /// <summary>
    /// Логика взаимодействия для WindowCar.xaml
    /// </summary>
    public partial class WindowCar : Window
    {
        string carId;
        private car selectedCar;
        public WindowCar()
        {
            InitializeComponent();
            LoadCar();
             LoadCarColors();
            
            LoadRegion();
        }

        private void LoadCarColors()
        {
            using (var context = new upEntities())
            {
                var colors = context.car_colors.ToList(); // Получаем список цветов из базы данных
                carComboBox.ItemsSource = colors.Select(c => new ComboBoxItem
                {
                    Content = c.Name_EN,
                    Tag = c.Color_num // Сохраняем номер цвета в Tag
                }).ToList();

                carComboBox1.ItemsSource = colors.Select(c => new ComboBoxItem
                {
                    Content = c.Name_EN,
                    Tag = c.Color_num // Сохраняем номер цвета в Tag
                }).ToList();
            }
        }

        private void LoadRegion()
        {
            using (var context = new upEntities())
            {
                var reg = context.region.ToList(); // Получаем список цветов из базы данных
                regionComboBox.ItemsSource = reg.Select(c => new ComboBoxItem
                {
                    Content = c.Region_name,
                    Tag = c.Code_sub // Сохраняем номер цвета в Tag
                }).ToList();


                regionComboBox1.ItemsSource = reg.Select(c => new ComboBoxItem
                {
                    Content = c.Region_name,
                    Tag = c.Code_sub // Сохраняем номер цвета в Tag
                }).ToList();
            }
        }


        private void LoadCar()
        {
            using (upEntities context = new upEntities())
            {
                var cars = context.car.ToList(); // Загрузка всех лицензий из базы данных
                CarDataGrid.ItemsSource = cars; // Установка источника данных для DataGrid
                CarEditDataGrid.ItemsSource = cars;
                CarDelDataGrid.ItemsSource = cars;
            }



        }


    

        private void Button_Click(object sender, RoutedEventArgs e)
        {
         
                using (upEntities context = new upEntities())
                {
                   

                    // Получаем данные из текстовых полей и комбобоксов
                string vin = DriverName.Text;
                string manufacturer = DriverFullName.Text;
                string model = Model.Text; // Модель
                string year= YearProd.Text;

                
                int colorNumber = (carComboBox.SelectedItem as ComboBoxItem)?.Tag is int colorNum ? colorNum : 0; // Номер цвета
                int weight = int.Parse(Weight.Text); // Вес
                int engineType = EngineComboBox.SelectedIndex + 1;
                int driveType = DriveComboBox.SelectedIndex + 1; // Привод
                
                int regionCode = (regionComboBox.SelectedItem as ComboBoxItem)?.Tag is int reg ? reg : 0; // Код региона

                // Создаем новый объект записи
                var car = new car // Замените YourEntity на вашу модель
                {
                    VIN = vin,
                    Manufacturer = manufacturer,
                    Model = model,
                    Weight = weight,
                    Year = year,
                    id_engine = engineType,
                    id_type_of_drive = driveType,
                    Color_num = colorNumber,
                    code_region = regionCode
                };

                // Добавляем запись в контекст и сохраняем изменения
                context.car.Add(car); // Замените YourEntities на вашу DbSet
                context.SaveChanges();
                CarDataGrid.Items.Refresh();

                MessageBox.Show("Запись о новой машине успешно добавлена.");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (upEntities context = new upEntities())
            {

                // Находим МАШИНУ в базе данных по её идентификатору
                var carToUpdate = context.car.FirstOrDefault(l => l.VIN == carId);
                if (carToUpdate != null)
                {
                    // Обновляем статус лицензии
                   
                    string manufacturer = DriverFullName1.Text;
                    string model = Model1.Text; // Модель
                    string year = YearProd1.Text;


                    int colorNumber = (carComboBox1.SelectedItem as ComboBoxItem)?.Tag is int colorNum ? colorNum : 0; // Номер цвета
                    int weight = int.Parse(Weight1.Text); // Вес
                    int engineType = EngineComboBox1.SelectedIndex + 1;
                    int driveType = DriveComboBox1.SelectedIndex + 1; // Привод

                    int regionCode = (regionComboBox1.SelectedItem as ComboBoxItem)?.Tag is int reg ? reg : 0; // Код региона

                    
                   carToUpdate.Manufacturer = manufacturer;
                    carToUpdate.Model = model;
                    carToUpdate.Weight = weight;
                    carToUpdate.Year = year;
                    carToUpdate.id_engine = engineType;
                    carToUpdate.id_type_of_drive = driveType;
                    carToUpdate.Color_num = colorNumber;
                    carToUpdate.code_region = regionCode;


                     context.SaveChanges();
                    CarEditDataGrid.Items.Refresh();

                    MessageBox.Show("Запись успешно изменена.");



                }
            }

        }

        private void CarEditDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
                if (CarEditDataGrid.SelectedItem is car selectedcar)
                {
                    // Получаем ID выбранной лицензии
                    carId = selectedcar.VIN;

                    // Здесь вы можете использовать licenceId по своему усмотрению
                    MessageBox.Show($"Выбранный ID машины {carId}");
                }

            }

        private void CarDelDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCar = CarDelDataGrid.SelectedItem as car;
        }
        private void Back(object sender, RoutedEventArgs e)
        {
            DriverWindow mainWindow = new DriverWindow();
            mainWindow.Show();
            this.Close();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (selectedCar == null)
            {
                MessageBox.Show("Пожалуйста, выберите запись для удаления.");
                return;
            }

            using (var context = new upEntities())
            {
                try
                {
                    // Находим запись в базе данных по VIN
                    var carToDelete = context.car.FirstOrDefault(c => c.VIN == selectedCar.VIN);

                    if (carToDelete != null)
                    {
                        context.car.Remove(carToDelete); // Удаляем запись
                        context.SaveChanges(); // Сохраняем изменения
                        MessageBox.Show("Запись успешно удалена.");
                        RefreshCarDataGrid(); // Метод для обновления DataGrid (реализуйте его)
                    }
                    else
                    {
                        MessageBox.Show("Запись не найдена.");
                    }
                }
                catch (Exception ex)
                {
                    // Обработка исключений
                    MessageBox.Show($"Произошла ошибка при удалении записи: {ex.Message}");
                }
            }
        }

        private void RefreshCarDataGrid()
        {
            using (var context = new upEntities())
            {
                CarDelDataGrid.ItemsSource = context.car.ToList(); // Замените car на вашу модель
            }
        }
    }

}
