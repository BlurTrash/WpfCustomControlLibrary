using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfCustomControlLibrary.Controls
{
    public class MultiselectBaseComboBox : WatermarkComboBox
    {
        public MultiselectBaseComboBox()
        {
            var myResourceDictionary = new ResourceDictionary();
            myResourceDictionary.Source = new Uri("/WpfCustomControlLibrary;component/Themes/MultiselectBaseComboBox/MultiselectBaseComboboxPrimeStyle.xaml", UriKind.RelativeOrAbsolute);

            var style = myResourceDictionary["MultiselectBaseComboBoxPrimeStyle"] as Style;

            if (style != null)
            {
                this.Style = style;
            }
        }

        //static MultiselectComboBox()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiselectComboBox), new FrameworkPropertyMetadata(typeof(MultiselectComboBox)));
        //}
    }
}
