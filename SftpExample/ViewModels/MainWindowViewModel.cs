using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
            MessageWindowOpen = !CnManager.Connect();

            Files = new ObservableCollection<FileDescription>();
            CnManager.FillFileList(Files);
        }

        private DelegateCommand<CultureInfo> _changeLangCmd;
        private string _title;
        private DelegateCommand _selectDestinationCmd;
        private string _destinationPath = " ";
        private bool _messageWindowOpen;
        private DelegateCommand _selectAllFilesCmd;
        private DelegateCommand<FileDescription> _selectFileCmd;

        public DelegateCommand<CultureInfo> ChangeLangCmd
        {
            get { return _changeLangCmd ??= new DelegateCommand<CultureInfo>(ChangeLangCmd_EventHandler); }
        }

        public DelegateCommand SelectDestinationCmd
        {
            get { return _selectDestinationCmd ??= new DelegateCommand(SelectDestinationCmd_EventHandler); }
        }

        public DelegateCommand SelectAllFilesCmd
        {
            get { return _selectAllFilesCmd ??= new DelegateCommand(SelectAllFilesCmd_EventHandler); }
        }

        public DelegateCommand<FileDescription> SelectFileCmd
        {
            get { return _selectFileCmd ??= new DelegateCommand<FileDescription>(SelectFileCmd_EventHandler); }
        }

        public AppLanguage Lang { get; set; }

        public ObservableCollection<FileDescription> Files { get; set; }

        private ConnectionManager CnManager { get; set; }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }

        public string DestinationPath
        {
            get => _destinationPath;
            set
            {
                _destinationPath = value;
                RaisePropertyChanged();
            }
        }

        public bool MessageWindowOpen
        {
            get => _messageWindowOpen;
            set
            {
                _messageWindowOpen = value;
                RaisePropertyChanged();
            }
        }


        private void ChangeLangCmd_EventHandler(CultureInfo ci)
        {
            if (ci != null)
                if (!Equals(ci, Lang.Language))
                    Lang.Language = ci;
        }

        private void SelectDestinationCmd_EventHandler()
        {
            using var fbd = new FolderBrowserDialog();
            var result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                DestinationPath = fbd.SelectedPath;
            }
        }

        private void SelectAllFilesCmd_EventHandler()
        {
            var firstFile = Files.FirstOrDefault();
            bool selection = firstFile != null && firstFile.Selected;
            foreach (var f in Files)
            {
                f.Selected = !selection;
            }
        }

        private void SelectFileCmd_EventHandler(FileDescription file)
        {
            file.Selected = !file.Selected;
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
