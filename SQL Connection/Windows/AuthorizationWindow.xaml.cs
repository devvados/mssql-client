using System;
using System.Collections.Generic;
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
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace SQL_Connection
{
    /// <summary>
    /// Interaction logic for AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : MetroWindow
    {
        public event Action<bool?> LogIn;

        public AuthorizationWindow()
        {
            InitializeComponent();

            TBLogin.Focus();
        }

        private void BLogIn_Click(object sender, RoutedEventArgs e)
        {
            string login = TBLogin.Text, password = TBPassword.Password, loginToCompare, passwordToCompare;
            using (StreamReader sr = new StreamReader("admin.txt"))
            {
                loginToCompare = sr.ReadLine();
                passwordToCompare = sr.ReadLine();
            }
            if(login == loginToCompare && password == passwordToCompare)
            {
                LogIn(true);
                this.Close();
            }
            else
            {
                ShowMessageBox("Неверный Логин/Пароль. Попробуйте ввести еще раз или войдите как гость.");
            }
        }

        public async void ShowMessageBox(string text)
        {
            MetroDialogSettings ms = new MetroDialogSettings()
            {
                ColorScheme = MetroDialogColorScheme.Theme,
                AnimateShow = true,
                AnimateHide = true
            };
            await this.ShowMessageAsync("Ошибка", text, MessageDialogStyle.Affirmative, ms);
        }

        private void BAsGuest_Click(object sender, RoutedEventArgs e)
        {
            LogIn(false);
            this.Close();
        }
    }
}
