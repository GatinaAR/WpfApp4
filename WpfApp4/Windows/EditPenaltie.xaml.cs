using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    /// Логика взаимодействия для EditPenaltie.xaml
    /// </summary>
    public partial class EditPenaltie : Window
    {
        penalties currentpenal = new penalties();
        byte[] image_bytes;
        private penalties _currentPenaltie = new penalties();
        List<string> ps = new List<string>();
        List<int> licenc = new List<int>();
        List<string> cars = new List<string>();
        public EditPenaltie()
        {
            InitializeComponent();
            Add.Visibility = Visibility.Visible;
            foreach (var p in upEntities.GetContext().status_penalties.ToList().Distinct())
            {
                if (!string.IsNullOrEmpty(p.name_status_penaltie))
                {
                    ps.Add(p.name_status_penaltie);
                }
            }
            foreach (var l in upEntities.GetContext().licence.ToList().Distinct())
            {
                licenc.Add(l.id_licence);
            }
            foreach (var c in upEntities.GetContext().car.ToList().Distinct())
            {
                if (!string.IsNullOrEmpty(c.VIN))
                {
                    cars.Add(c.VIN);
                }
            }
            car.ItemsSource = cars;
            status.ItemsSource = ps;
            lic.ItemsSource = licenc;
        }
        public EditPenaltie(penalties penaltie)
        {
            InitializeComponent();
            currentpenal = penaltie;
            Edit.Visibility = Visibility.Visible;
            if (penaltie.photo_car != null)
            {
                byte[] imageData = penaltie.photo_car;
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(imageData))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                }
                ava.Source = bitmapImage;
                BlurRadius.Radius = 0;
            }
            foreach (var p in upEntities.GetContext().status_penalties.ToList().Distinct())
            {
                if (!string.IsNullOrEmpty(p.name_status_penaltie))
                {
                    ps.Add(p.name_status_penaltie);
                }
            }
            foreach (var c in upEntities.GetContext().car.ToList().Distinct())
            {
                if (!string.IsNullOrEmpty(c.VIN))
                {
                    cars.Add(c.VIN);
                }
            }
            foreach (var l in upEntities.GetContext().licence.ToList().Distinct())
            {
                licenc.Add(l.id_licence);
            }
            status_penalties st = upEntities.GetContext().status_penalties.FirstOrDefault(s => s.id_status_penaltie == penaltie.id_status_penaltie);
            status.SelectedItem = st.name_status_penaltie;
            status.ItemsSource = ps;
            lic.ItemsSource = licenc;
            car.ItemsSource = cars;

            if (penaltie == null) return;
            _currentPenaltie = penaltie;
            DataContext = _currentPenaltie;

        }
        private void Back(object sender, RoutedEventArgs e)
        {
            PenaltiesWindow mainWindow = new PenaltiesWindow();
            mainWindow.Show();
            this.Close();
        }
        private void AddPen(object sender, RoutedEventArgs e)
        {
            using (upEntities context = new upEntities())
            {
                int id_status = 1;
                status_penalties st = context.status_penalties.FirstOrDefault(sp => sp.name_status_penaltie == status.SelectedItem);
                if (st != null)
                {
                    id_status = st.id_status_penaltie;
                }
                decimal result;
                bool isValidDecimal = decimal.TryParse(sum.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
                if (isValidDecimal)
                {
                    if (Datepic.SelectedDate != null && !string.IsNullOrWhiteSpace(time.Text))
                    {
                        string dateTimeString = $"{Datepic.SelectedDate.Value.ToString("yyyy-MM-dd")} {time.Text}";

                        if (DateTime.TryParse(dateTimeString, out DateTime dateTime))
                        {
                            if (!string.IsNullOrWhiteSpace(sum.Text))
                                try
                                {
                                    penalties p = new penalties
                                    {
                                        date_time = dateTime,
                                        id_status_penaltie = id_status,
                                        summa = Convert.ToDecimal(sum.Text),
                                        id_licence = Convert.ToInt16(lic.SelectedItem),
                                        VIN_car = Convert.ToString(car.SelectedItem),
                                        photo_car = image_bytes
                                    };
                                    context.penalties.Add(p);
                                    context.SaveChanges();
                                    MessageBox.Show("Штраф добавлен");
                                    PenaltiesWindow mainWindow = new PenaltiesWindow();
                                    mainWindow.Show();
                                    this.Close();
                                }
                                catch
                                {
                                }

                        }
                        else
                        {
                            MessageBox.Show("Неверный формат времени. Пожалуйста, используйте формат HH:mm.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Введите дату и время");
                    }

                }
                else
                {
                    MessageBox.Show("Неверное введенное значение!");
                }

            }
        }
        private void EditPen(object sender, RoutedEventArgs e)
        {

            using (upEntities context = new upEntities())
            {
                penalties pe = context.penalties.Find(currentpenal.id_penaltie);
                if (image_bytes != null)
                {
                    pe.photo_car = image_bytes;
                    context.SaveChanges();
                }
                if (Datepic.SelectedDate != null && !string.IsNullOrWhiteSpace(time.Text))
                {
                    string dateTimeString = $"{Datepic.SelectedDate.Value.ToString("yyyy-MM-dd")} {time.Text}";

                    if (DateTime.TryParse(dateTimeString, out DateTime dateTime))
                    {

                        pe.date_time = dateTime;
                        context.SaveChanges();
                    }
                    else
                    {
                        MessageBox.Show("Неверный формат времени. Пожалуйста, используйте формат HH:mm.");
                    }
                }
            }
            upEntities.GetContext().SaveChanges();
            MessageBox.Show("Штраф изменен");
            PenaltiesWindow mainWindow = new PenaltiesWindow();
            mainWindow.Show();
            this.Close();
        }
        private void addPhoto(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            try
            {
                image_bytes = File.ReadAllBytes(openFileDialog.FileName);

            }
            catch (Exception ex) { }
        }
    
}
}
