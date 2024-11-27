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

namespace WpfApp4.Windows
{
    /// <summary>
    /// Логика взаимодействия для DriverWindow.xaml
    /// </summary>
    public partial class DriverWindow : Window
    {
        private driver selectedDriv;
        public DriverWindow()
        {
            InitializeComponent();
            driversGrid.ItemsSource = upEntities.GetContext().driver.ToList();
        }
        private void EditWindow(object sender, RoutedEventArgs e)
        {
            var item = (driver)driversGrid.SelectedItem;
            new EditDriver(item).Show();
            this.Close();

        }
        private void Licences_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();

        }
        private void Cars_Click(object sender, RoutedEventArgs e)
        {
            new WindowCar().Show();
            this.Close();

        }
        private void AddDriver(object sender, RoutedEventArgs e)
        {
            new EditDriver().Show();
            this.Close();

        }
        private void DriverGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            selectedDriv= driversGrid.SelectedItem as driver;
        }
            private void DelDriver(object sender, RoutedEventArgs e)
        {
            using (var context = new upEntities())
            {
                try
                {
                    // Находим запись в базе данных по VIN
                    var carToDelete = context.driver.FirstOrDefault(c => c.id == selectedDriv.id);

                    if (carToDelete != null)
                    {
                        context.driver.Remove(carToDelete); // Удаляем запись
                        context.SaveChanges(); // Сохраняем изменения
                        MessageBox.Show("Выбранный водитель удален");
                        new DriverWindow().Show();
                        this.Close();
                        // RefreshCarDataGrid(); // Метод для обновления DataGrid (реализуйте его)
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
    }
}
