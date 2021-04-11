using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Prism.Commands;
using Prism.Mvvm;
using SftpExample.Models;
using static System.Windows.Application;

namespace SftpExample.ViewModels
{
    public class MainWindowViewModel : BindableBase, IDisposable
    {
        public MainWindowViewModel()
        {
            Lang = new AppLanguage();

            RefreshTitle();

            Lang.LanguageChanged += Lang_LanguageChanged;

            CnManager = new ConnectionManager();

            ConnectCmdExecute();
        }

        private DelegateCommand<CultureInfo> _changeLangCmd;
        private string _title;
        private DelegateCommand _selectDestinationCmd;
        private string _destinationPath = " ";
        private bool _messageWindowIsOpen;
        private DelegateCommand _selectAllFilesCmd;
        private DelegateCommand<FileDescription> _selectFileCmd;
        private DelegateCommand<FileDescription> _toFolderCmd;
        private bool _isBusy;
        private DelegateCommand _downloadCmd;
        private ObservableCollection<FileDescription> _files = new ObservableCollection<FileDescription>();
        private bool _settingsIsOpen;
        private DelegateCommand _showSettingsCmd;
        private DelegateCommand _connectCmd;

        public DelegateCommand ConnectCmd
        {
            get { return _connectCmd ??= new DelegateCommand(ConnectCmdExecute); }
        }

        public DelegateCommand<CultureInfo> ChangeLangCmd
        {
            get { return _changeLangCmd ??= new DelegateCommand<CultureInfo>(ChangeLangCmdExecute); }
        }

        public DelegateCommand SelectDestinationCmd
        {
            get { return _selectDestinationCmd ??= new DelegateCommand(SelectDestinationCmdExecute); }
        }

        public DelegateCommand SelectAllFilesCmd
        {
            get { return _selectAllFilesCmd ??= new DelegateCommand(SelectAllFilesCmdExecute); }
        }

        public DelegateCommand<FileDescription> SelectFileCmd
        {
            get { return _selectFileCmd ??= new DelegateCommand<FileDescription>(SelectFileCmdExecute); }
        }

        public DelegateCommand<FileDescription> ToFolderCmd
        {
            get { return _toFolderCmd ??= new DelegateCommand<FileDescription>(ToFolderCmdExecute, ToFolderCmdCanExecute); }
        }

        public DelegateCommand DownloadCmd
        {
            get { return _downloadCmd ??= new DelegateCommand(DownloadCmdExecuteAsync, DownloadCmdCanExecute); }
        }

        public DelegateCommand ShowSettingsCmd
        {
            get { return _showSettingsCmd ??= new DelegateCommand(ShowSettingsCmdExecute); }
        }

        public AppLanguage Lang { get; set; }

        public ObservableCollection<FileDescription> Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }

        public ConnectionManager CnManager { get; }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string DestinationPath
        {
            get => _destinationPath;
            set
            {
                SetProperty(ref _destinationPath, value);
                DownloadCmd.RaiseCanExecuteChanged();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public bool SettingsIsOpen
        {
            get => _settingsIsOpen;
            set => SetProperty(ref _settingsIsOpen, value);
        }

        public bool MessageWindowIsOpen
        {
            get => _messageWindowIsOpen;
            set => SetProperty(ref _messageWindowIsOpen, value);
        }

        private async void ConnectCmdExecute()
        {
            IsBusy = true;

            await Task.Run(() =>
            {
                bool connected = CnManager.Connect();
                MessageWindowIsOpen = !connected;
                Files = connected ? CnManager.GetFiles() : new ObservableCollection<FileDescription>();
            });
            
            IsBusy = false;
        }

        private void ChangeLangCmdExecute(CultureInfo ci)
        {
            if (ci != null)
                if (!Equals(ci, Lang.Language))
                    Lang.Language = ci;
        }

        private void SelectDestinationCmdExecute()
        {
            using var fbd = new FolderBrowserDialog();
            var result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                DestinationPath = fbd.SelectedPath;
            }
        }

        private void SelectAllFilesCmdExecute()
        {
            var firstFile = Files.FirstOrDefault();
            bool selection = firstFile != null && firstFile.Selected;
            foreach (var f in Files)
                f.Selected = !selection;

            DownloadCmd.RaiseCanExecuteChanged();
        }

        private void SelectFileCmdExecute(FileDescription file)
        {
            file.Selected = !file.Selected;
            DownloadCmd.RaiseCanExecuteChanged();
        }

        private async void ToFolderCmdExecute(FileDescription file)
        {
            IsBusy = true;

            await Task.Run(() =>
            {
                Files = CnManager.GetFiles(file.FullFileName);
            });

            IsBusy = false;
        }

        private async void DownloadCmdExecuteAsync()
        {
            IsBusy = true;
            
            await Task.Run(async () =>
            {
                await CnManager.DownloadAsync(Files, DestinationPath);
            });

            IsBusy = false;
        }

        private void ShowSettingsCmdExecute()
        {
            SettingsIsOpen = !SettingsIsOpen;
        }

        private bool ToFolderCmdCanExecute(FileDescription file)
        {
            return file.IsDirectory;
        }

        private bool DownloadCmdCanExecute()
        {
            return (Files.Any(f => f.Selected) && Directory.Exists(DestinationPath));
        }

        private void RefreshTitle()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Title = Current.TryFindResource("SeSftpDownloader") as string + " " + version;
        }

        private void Lang_LanguageChanged(CultureInfo cultureInfo)
        {
            RefreshTitle();
        }

        public void Dispose()
        {
            Lang.LanguageChanged -= Lang_LanguageChanged;
        }
    }
}
