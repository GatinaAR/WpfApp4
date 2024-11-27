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
    /// Логика взаимодействия для PenaltiesWindow.xaml
    /// </summary>
    public partial class PenaltiesWindow : Window
    {
        private penalties selectedPenaltie;
        public PenaltiesWindow()
        {
            InitializeComponent();
            penaltieGrid.ItemsSource = upEntities.GetContext().penalties.ToList();
        }
        private void Editpenaltie(object sender, RoutedEventArgs e)
        {
            var item = (penalties)penaltieGrid.SelectedItem;

            new EditPenaltie(item).Show();
            this.Close();

        }
        private void Addpenaltie(object sender, RoutedEventArgs e)
        {
            new EditPenaltie().Show();
            this.Close();
        }
        private void PenaltiesGrid_SelectionChanged(object sender, RoutedEventArgs e)
        {
            selectedPenaltie = penaltieGrid.SelectedItem as penalties;
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            new AuthorizationWindow().Show();
            this.Close();
        }
        private void Delpenaltie(object sender, RoutedEventArgs e)
        {
            if (selectedPenaltie == null)
            {
                MessageBox.Show("Пожалуйста, выберите запись для удаления.");
                return;
            }

            using (var context = new upEntities())
            {
                try
                {
                    var carToDelete = context.penalties.FirstOrDefault(c => c.id_penaltie == selectedPenaltie.id_penaltie);

                    if (carToDelete != null)
                    {
                        context.penalties.Remove(carToDelete); // Удаляем запись
                        context.SaveChanges(); // Сохраняем изменения
                        MessageBox.Show("Запись успешно удалена.");
                        new PenaltiesWindow().Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Запись не найдена.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при удалении записи: {ex.Message}");
                }
            }
        }
    }
}
