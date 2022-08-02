using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tradility.Common.Extensions;
using Tradility.Common.Localization;

namespace Tradility.ViewModels
{
    public class LanguageViewModel
    {
        public Language Language { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public ImageSource IconSource => new BitmapImage(new Uri(Icon));

        public LanguageViewModel(Language language)
        {
            Language = language;
            Icon = $"pack://application:,,,/Properties/Icons/{language.ToCode()}.png";
            Name = language.ToString();
        }
    }
}
