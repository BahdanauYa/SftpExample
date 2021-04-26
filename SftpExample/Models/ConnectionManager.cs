using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Prism.Mvvm;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace SftpExample.Models
{
    public class ConnectionManager : BindableBase, IDisposable
    {
        private string _host = "test.rebex.net";
        private string _username = "demo";
        private string _password = "password";
        private SftpClient Sftp { get; set; }

        public string Host
        {
            get => _host;
            set => SetProperty(ref _host, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool Connect()
        {
            try
            {
                Sftp = new SftpClient(Host, Username, Password);

                Sftp.Connect();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ObservableCollection<FileDescription> GetFiles(string directory = null)
        {
            if (directory == null) directory = Sftp.WorkingDirectory;

            var fList = new ObservableCollection<FileDescription>();

            var temp = Sftp.ListDirectory(directory);
            foreach (var f in temp)
            {
                if (f.Name.StartsWith(".") && f.Name != "..") continue;

                fList.Add(new FileDescription(f.FullName, f.Name, f.Length, f.IsDirectory));
            }

            return fList;
        }

        public async Task DownloadAsync(ObservableCollection<FileDescription> fList, string destinationPath)
        {
            foreach (var f in fList)
            {
                if (f.Name.StartsWith(".")) continue;
                if (!f.Selected) continue;

                if (f.IsDirectory)
                {
                    Directory.CreateDirectory(Path.Combine(destinationPath, f.Name));

                    IEnumerable<SftpFile> subFiles = null;
                    await Task.Run(() => { subFiles = Sftp.ListDirectory(f.FullFileName); });

                    foreach (var item in subFiles)
                    {
                        DownloadFolder(item, Path.Combine(destinationPath, f.Name));
                    }

                    f.Downloaded = true;
                    continue;
                }

                using Stream fileStream = File.Create(Path.Combine(destinationPath, f.Name));
                Sftp.DownloadFile(f.FullFileName, fileStream);
                f.Downloaded = true;
            }
        }

        private void DownloadFolder(SftpFile f, string destinationPath)
        {
            if (f.Name.StartsWith(".")) return;

            if (f.IsDirectory)
            {
                Directory.CreateDirectory(Path.Combine(destinationPath, f.Name));
                var subFiles = Sftp.ListDirectory(f.FullName);

                foreach (var item in subFiles)
                {
                    DownloadFolder(item, Path.Combine(destinationPath, f.Name));
                }
            }
            else
                using (Stream fileStream = File.Create(Path.Combine(destinationPath, f.Name)))
                {
                    Sftp.DownloadFile(f.FullName, fileStream);
                }
        }

        public void Dispose()
        {
            if (Sftp.IsConnected)
                Sftp.Disconnect();
        }
    }
}