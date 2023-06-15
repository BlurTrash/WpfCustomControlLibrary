using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfCustomControlLibrary.Controls.Converters
{

    public class DisplayNameConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType().IsEnum)
                {
                    var fieldInfo = value.GetType().GetField(value.ToString());
                    var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
                    if (displayAttribute != null)
                    {
                        return displayAttribute.Name;
                    }
                }

                return Binding.DoNothing;
            }

            return Binding.DoNothing;
        }

        public virtual object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
