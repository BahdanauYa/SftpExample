using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using SftpExample.Models;

namespace SftpExample.ViewModels
{
    public class MainWindowViewModel:BindableBase
    {
        public MainWindowViewModel()
        {
            Lang = new AppLanguage();
        }

        private DelegateCommand<CultureInfo> _changeLangCmd;

        public DelegateCommand<CultureInfo> ChangeLangCmd
        {
            get { return _changeLangCmd ??= new DelegateCommand<CultureInfo>(ChangeLangCmd_EventHandler); }
        }

        public AppLanguage Lang { get; set; }

        
        private void ChangeLangCmd_EventHandler(CultureInfo ci)
        {
            if (ci != null)
                if (!Equals(ci, Lang.Language))
                    Lang.Language = ci;
        }
        
    }
}
