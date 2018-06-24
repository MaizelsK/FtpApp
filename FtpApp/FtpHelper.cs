using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FtpApp
{
    public class FtpHelper
    {
        static public string UserName { get; set; }
        static public string Password { get; set; }
        static public bool EnableSsl { get; set; }

        static async public Task<List<DirectoryItem>> GetDirectoryList(string path)
        {
            List<DirectoryItem> directoryList = new List<DirectoryItem>();

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = new NetworkCredential(UserName, Password);
            request.EnableSsl = EnableSsl;

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    List<string> directories = new List<string>();

                    // Получение всех директорий
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string oneDirectory = reader.ReadLine();

                        while (!string.IsNullOrEmpty(oneDirectory))
                        {
                            directories.Add(oneDirectory);
                            oneDirectory = reader.ReadLine();
                        }
                    }

                    directoryList = ParseDirectories(path, directories);
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
            catch (IOException ex)
            {
                throw ex;
            }

            return directoryList;
        }

        static async public Task DownloadFile(string path, string fileName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(UserName, Password);
            request.EnableSsl = EnableSsl;

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    string savePath = Directory.GetCurrentDirectory() + "\\Saved Files";
                    Directory.CreateDirectory(savePath);

                    using (WebClient client = new WebClient() { Credentials = new NetworkCredential(UserName, Password) })
                    {
                        client.DownloadFileAsync(new Uri(path), savePath + "\\" + fileName);
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        static async public Task UploadFile(string filePath, string currentAddress, string fileName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(currentAddress + "/" + fileName);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(UserName, Password);
            request.EnableSsl = EnableSsl;

            try
            {
                byte[] data = File.ReadAllBytes(filePath);
                request.ContentLength = data.Length;

                using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    await requestStream.WriteAsync(data, 0, data.Length);
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        static public List<DirectoryItem> ParseDirectories(string address, List<string> directories)
        {
            List<DirectoryItem> dirItems = new List<DirectoryItem>();
            string appDirectory = Directory.GetCurrentDirectory();

            foreach (string directory in directories)
            {
                string directoryName = directory.Substring(directory.LastIndexOf(' ') + 1);
                string directoryPath = address + directoryName;
                bool isDirectory = directory[0] == 'd' ? true : false;

                DirectoryItem newItem = new DirectoryItem
                {
                    Path = directoryPath,
                    Name = directoryName,
                    IsDirectory = isDirectory,
                    Image = isDirectory == true ?
                            new BitmapImage(new Uri(appDirectory + "\\Directory.png")) :
                            new BitmapImage(new Uri(appDirectory + "\\File.png"))
                };

                dirItems.Add(newItem);
            }

            return dirItems;
        }

        static public void SetFtpServerParameters(string userName, string password, bool enableSsl)
        {
            UserName = userName;
            Password = password;
            EnableSsl = enableSsl;
        }
    }
}