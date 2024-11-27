
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp4.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        public int idrole;
        private const string LockFile = "lock_state.json";
        private const string CorrectPassword = "password123"; // Правильный пароль
        private LockState _lockState;
        public AuthorizationWindow()
        {
            InitializeComponent();
            LoadLockState();
        }
        public void authorizationBut(object sender, RoutedEventArgs e)
        {
            //Windows.DriverWindow dr = new Windows.DriverWindow();
            //      dr.Show();
            //      this.Close();
            if (IsBlocked())
            {
                MessageBox.Show("Вы заблокированы. Попробуйте снова позже.");
                return;
            }
            if (!string.IsNullOrWhiteSpace(password.Text) && !string.IsNullOrWhiteSpace(login.Text))
            {
                using (upEntities context = new upEntities())
                {
                    bool Autentification(string password, string login)
                    {
                        users users = context.users.FirstOrDefault(m => m.password_user == password && m.login_user == login);
                        if (users != null)
                        {
                            idrole = (int)users.id_role;
                            return context.users.Any(m => m.password_user == password && m.login_user == login);
                        }
                        else
                        {
                            return false;
                        }


                    }
                    if (Autentification(password.Text, login.Text))
                    {
                        _lockState.Attempts = 0;
                        SaveLockState();
                        if (idrole == 2)
                        {
                            Windows.DriverWindow dr = new Windows.DriverWindow();
                            dr.Show();
                            this.Close();
                        }
                        else if (idrole == 1)
                        {
                            Windows.PenaltiesWindow dr = new Windows.PenaltiesWindow();
                            dr.Show();
                            this.Close();
                        }

                    }
                    else
                    {
                        _lockState.Attempts++;
                        MessageBox.Show("Вам отказано в доступе" + password.Text);
                        if (_lockState.Attempts >= 3)
                        {
                            _lockState.BlockedUntil = DateTime.Now.AddMinutes(1); // Блокировка на 1 минуту
                            MessageBox.Show("Вы заблокированы на 1 минуту.");

                        }

                        SaveLockState();
                    }

                }
            }
            else
            {
                _lockState.Attempts++;
                if (_lockState.Attempts >= 3)
                {
                    _lockState.BlockedUntil = DateTime.Now.AddMinutes(1); // Блокировка на 1 минуту
                    MessageBox.Show("Вы заблокированы на 1 минуту.");
                }

                SaveLockState();
                MessageBox.Show("Введите правильные данные" + login.Text);
            }
        }
        private void LoadLockState()
        {
            if (File.Exists(LockFile))
            {
                var json = File.ReadAllText(LockFile);
                _lockState = JsonSerializer.Deserialize<LockState>(json);
            }
            else
            {
                _lockState = new LockState { Attempts = 0, BlockedUntil = DateTime.MinValue };
            }
        }

        private void SaveLockState()
        {
            var json = JsonSerializer.Serialize(_lockState);
            File.WriteAllText(LockFile, json);
        }

        private bool IsBlocked()
        {
            if (_lockState.Attempts >= 3 && DateTime.Now >= _lockState.BlockedUntil)
            {
                _lockState.Attempts = 0; // Сброс попыток
                _lockState.BlockedUntil = DateTime.MinValue; // Сброс времени блокировки
                SaveLockState(); // Сохраняем изменения
                return false; // Теперь не заблокирован
            }
            return _lockState.Attempts >= 3;
        }
    }
    public class LockState
    {
        public int Attempts { get; set; }
        public DateTime BlockedUntil { get; set; }
    }
}
