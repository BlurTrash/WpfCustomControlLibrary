using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace WpfCustomControlLibrary.Controls
{
    [TemplatePart(Name = PART_CheckBox, Type = typeof(CheckBox))]
    public class ExtendedListBoxItem : ListBoxItem
    {
        private const string PART_CheckBox = "PART_CheckBox";

        private CheckBox _checkBox;

        public ExtendedListBoxItem()
        {
        }

        static ExtendedListBoxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedListBoxItem),
                                                     new FrameworkPropertyMetadata(
                                                         typeof(ExtendedListBoxItem)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AttacCheckBox();
        }

        private void AttacCheckBox()
        {
            var element = GetTemplateChild(PART_CheckBox) as CheckBox;

            if (element != null)
            {
                _checkBox = (CheckBox)element;
            }
        }
    }
}
