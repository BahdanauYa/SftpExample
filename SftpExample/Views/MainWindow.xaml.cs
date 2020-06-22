using System.Linq;
using SftpExample.ViewModels;

namespace SftpExample.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            //удаление подключенного словаря, он был нужен только для xaml дизайнера
            var res = (from d in Resources.MergedDictionaries
                where d.Source != null && d.Source.OriginalString.Contains("Resources/lang.")
                select d).First();
            Resources.MergedDictionaries.Remove(res);

            DataContext = new MainWindowViewModel();
        }
    }
}
