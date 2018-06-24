using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FtpApp
{
    public partial class MainWindow : Window
    {
        public string CurrentAddress { get; set; }
        public List<DirectoryItem> CurrentDirectories { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        // Смена сервера
        private void ChangeServerClick(object sender, RoutedEventArgs e)
        {
            ConnectWindow connectWindow = new ConnectWindow(this);
            connectWindow.ShowDialog();

            DirectoriesListBox.ItemsSource = CurrentDirectories;
            CurrentDirText.Text = CurrentAddress;
        }

        // Скачивание файла или переход в другую директорию
        private async void ItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DirectoryItem selectedItem = DirectoriesListBox.SelectedItem as DirectoryItem;

            try
            {
                DirectoriesListBox.IsEnabled = false;

                if (selectedItem.IsDirectory)
                {
                    string path = null;

                    if (selectedItem.Name == "." || selectedItem.Name == "..")
                        path = CurrentAddress.Remove(CurrentAddress.LastIndexOf('/'));
                    else
                        path = CurrentAddress + "/" + selectedItem.Name;

                    DirectoriesListBox.ItemsSource = await FtpHelper.GetDirectoryList(path);
                    CurrentAddress = CurrentDirText.Text = path;
                }
                else
                {
                    string path = CurrentAddress + "/" + selectedItem.Name;
                    await FtpHelper.DownloadFile(path, selectedItem.Name);

                    MessageBox.Show($"Файл \"{selectedItem.Name}\" успешно сохранен");
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                DirectoriesListBox.IsEnabled = true;
            }
        }

        // Загрузка файла на сервер
        private async void UploadFileClick(object sender, RoutedEventArgs e)
        {
            if (CurrentAddress != null)
                try
                {
                    DirectoriesListBox.IsEnabled = false;

                    if (CurrentAddress != null)
                    {
                        string filePath = String.Empty;

                        OpenFileDialog fileDialog = new OpenFileDialog();

                        if (fileDialog.ShowDialog() == true)
                        {
                            filePath = fileDialog.FileName;
                            FileInfo info = new FileInfo(filePath);

                            await FtpHelper.UploadFile(filePath, CurrentAddress, info.Name);
                        }
                    }

                    CurrentDirectories = await FtpHelper.GetDirectoryList(CurrentAddress);
                    DirectoriesListBox.ItemsSource = CurrentDirectories;
                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    DirectoriesListBox.IsEnabled = true;
                }
        }
    }
}
