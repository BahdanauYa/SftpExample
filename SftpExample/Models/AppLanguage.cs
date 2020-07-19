using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using Prism.Mvvm;

namespace SftpExample.Models
{
    public class AppLanguage:BindableBase
    {
        public AppLanguage()
        {
            Languages = new ObservableCollection<CultureInfo> {new CultureInfo("en-US"), new CultureInfo("ru-RU")};

            Language = Properties.Settings.Default.DefaultLanguage;
        }
        
        public ObservableCollection<CultureInfo> Languages { get; }
        
        public delegate void LanguageHandler(CultureInfo cultureInfo);
        public event LanguageHandler LanguageChanged;

        public CultureInfo Language
        {
            get => System.Threading.Thread.CurrentThread.CurrentUICulture;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                
                //1. Меняем язык приложения:
                System.Threading.Thread.CurrentThread.CurrentUICulture = value;

                //2. Создаём ResourceDictionary для новой культуры
                ResourceDictionary dict = new ResourceDictionary();
                switch (value.Name)
                {
                    case "ru-RU":
                        dict.Source = new Uri($"Resources/lang.{value.Name}.xaml", UriKind.Relative);
                        break;
                    default:
                        dict.Source = new Uri("Resources/lang.xaml", UriKind.Relative);
                        break;
                }

                //3. Находим старую ResourceDictionary и удаляем его и добавляем новую ResourceDictionary
                ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                              where d.Source != null && d.Source.OriginalString.StartsWith("Resources/lang.")
                                              select d).FirstOrDefault();
                if (oldDict != null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }

                //4. Вызываем евент для оповещения всех окон.
                LanguageChanged?.Invoke(value);

                //5. Сохраняем язык
                Properties.Settings.Default.DefaultLanguage = value;
                Properties.Settings.Default.Save();

                RaisePropertyChanged();
            }
        }
    }
}
