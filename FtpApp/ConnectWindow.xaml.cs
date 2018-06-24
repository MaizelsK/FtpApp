using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace FtpApp
{
    public partial class ConnectWindow : Window
    {
        MainWindow mainWindow;

        public ConnectWindow(MainWindow window)
        {
            InitializeComponent();
            mainWindow = window;

            AddressTextBox.Text = "ftp://ftp.dlptest.com";
            UserNameTextBox.Text = "dlpuser@dlptest.com";
            PasswordTextBox.Password = "eiTqR7EMZD5zy7M";
            SslCheckBox.IsChecked = false;

            ConnectionProgressText.Visibility = Visibility.Hidden;
        }

        private async void ConnectBtnClick(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                ConnectionProgressText.Visibility = Visibility.Visible;

                string address = AddressTextBox.Text;
                string userName = UserNameTextBox.Text;
                string password = PasswordTextBox.Password;
                bool enableSsl = (bool)SslCheckBox.IsChecked;

                FtpHelper.SetFtpServerParameters(userName, password, enableSsl);

                try
                {
                    mainWindow.CurrentAddress = address;
                    mainWindow.CurrentDirectories = await FtpHelper.GetDirectoryList(address);
                    this.Close();
                }
                catch (WebException ex)
                {
                    ErrorTextBlock.Text = ex.Message;
                }
                catch (IOException ex)
                {
                    ErrorTextBlock.Text = ex.Message;
                }
                finally
                {
                    ConnectionProgressText.Visibility = Visibility.Hidden;
                }
            }
        }

        // Валидация формы
        private bool ValidateForm()
        {
            ErrorTextBlock.Text = String.Empty;

            if (String.IsNullOrWhiteSpace(AddressTextBox.Text))
            {
                ErrorTextBlock.Text = "Введите адрес";
                return false;
            }

            return true;
        }

        // Очитска элементов
        private void ClearForm()
        {
            AddressTextBox.Text = String.Empty;
            UserNameTextBox.Text = String.Empty;
            PasswordTextBox.Password = String.Empty;
            SslCheckBox.IsChecked = false;
        }
    }
}
