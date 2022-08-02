using System.Windows;

namespace Tradility.UI.Utils
{
    public class StringHolder : DependencyObject
    {
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(StringHolder), new PropertyMetadata(""));

        public override string ToString() => Value;
    }
}
