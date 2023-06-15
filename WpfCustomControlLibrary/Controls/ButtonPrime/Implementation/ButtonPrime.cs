using System.Windows;
using System.Windows.Controls;

namespace WpfCustomControlLibrary.Controls
{
    public enum Severity
    {
        Primary,
        Secondary,
        //Success,
        //Info,
        //Warning,
        //Help,
        //Error
    }

    public class ButtonPrime : Button
    {
        static ButtonPrime()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonPrime), new FrameworkPropertyMetadata(typeof(ButtonPrime)));
        }

        public static readonly DependencyProperty IsOutlinedProperty = DependencyProperty.Register("IsOutlined", typeof(bool), typeof(ButtonPrime), new PropertyMetadata(false));
        public bool IsOutlined
        {
            get { return (bool)GetValue(IsOutlinedProperty); }
            set { SetValue(IsOutlinedProperty, value); }
        }

        public static readonly DependencyProperty IsRoundedProperty = DependencyProperty.Register("IsRounded", typeof(bool), typeof(ButtonPrime), new PropertyMetadata(false));
        public bool IsRounded
        {
            get { return (bool)GetValue(IsRoundedProperty); }
            set { SetValue(IsRoundedProperty, value); }
        }

        public static readonly DependencyProperty IsTextProperty = DependencyProperty.Register("IsText", typeof(bool), typeof(ButtonPrime), new PropertyMetadata(false));
        public bool IsText
        {
            get { return (bool)GetValue(IsTextProperty); }
            set { SetValue(IsTextProperty, value); }
        }

        public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register("Severity", typeof(Severity), typeof(ButtonPrime), new PropertyMetadata(Severity.Primary));
        public Severity Severity
        {
            get { return (Severity)GetValue(SeverityProperty); }
            set { SetValue(SeverityProperty, value); }
        }

        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(double), typeof(ButtonPrime), new PropertyMetadata(11d));
        public double IconSize
        {
            get
            {
                return (double)GetValue(IconSizeProperty);
            }
            set
            {
                SetValue(IconSizeProperty, value);
            }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(IconType), typeof(ButtonPrime), new PropertyMetadata(IconType.None));
        public IconType Icon
        {
            get
            {
                return (IconType)GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ButtonPrime));
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }
    }
}
