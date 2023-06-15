using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfCustomControlLibrary.Controls
{
    public class ExtendedListBox : ListBox
    {
        public ExtendedListBox()
        {    
        }

        static ExtendedListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedListBox),
                                                     new FrameworkPropertyMetadata(
                                                         typeof(ExtendedListBox)));
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ExtendedListBoxItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ExtendedListBoxItem();
        }
    }
}
