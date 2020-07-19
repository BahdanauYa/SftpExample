using System;
using System.Collections.ObjectModel;
using Renci.SshNet;

namespace SftpExample.Models
{
    public class ConnectionManager:IDisposable
    {
        private SftpClient Sftp { get; set; }
        private string _host = "test.rebex.net";
        private string _username = "demo";
        private string _password = "password";

        public ConnectionManager()
        {
            Sftp = new SftpClient(_host, _username, _password);
        }

        internal bool Connect()
        {
            try
            {
                Sftp.Connect();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal void FillFileList(ObservableCollection<FileDescription> fList)
        {
            var temp = Sftp.ListDirectory(Sftp.WorkingDirectory);
            foreach (var f in temp)
            {
                if (f.Name.StartsWith(".") && f.Name != "..") continue;

                fList.Add(new FileDescription(f.FullName, f.Name, f.Length, f.IsDirectory));
            }
        }

        public void Dispose()
        {
            if (Sftp.IsConnected)
                Sftp.Disconnect();
        }
    }
}