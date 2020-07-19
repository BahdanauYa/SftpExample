using Prism.Mvvm;

namespace SftpExample.Models
{
    public class FileDescription:BindableBase
    {
        private bool _selected;

        public FileDescription(string fullFileName, string name, long size, bool isDirectory)
        {
            FullFileName = fullFileName;
            Name = name;
            Size = size;
            IsDirectory = isDirectory;
        }

        public string FullFileName { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public bool IsDirectory { get; set; }

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                RaisePropertyChanged();
            }
        }
    }
}
