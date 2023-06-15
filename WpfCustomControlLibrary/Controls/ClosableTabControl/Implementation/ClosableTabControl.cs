using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using WpfCustomControlLibrary.Services.Commands;

namespace WpfCustomControlLibrary.Controls
{
    public class ClosableTabControl : TabControl
    {
        public ICommand? DeleteCommand
        {
            get => (ICommand?)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }
        public static readonly DependencyProperty DeleteCommandProperty
            = DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(ClosableTabControl), new PropertyMetadata(default(ICommand?)));

        public object? DeleteCommandParameter
        {
            get => GetValue(DeleteCommandParameterProperty);
            set => SetValue(DeleteCommandParameterProperty, value);
        }
        public static readonly DependencyProperty DeleteCommandParameterProperty
            = DependencyProperty.Register(nameof(DeleteCommandParameter), typeof(object), typeof(ClosableTabControl), new PropertyMetadata(default(object?)));


        //Тест удаления если не реализована команда удаления из самого источника
        private RelayCommand<object> _removeItemCommand;

        private void RemoveItem(object obj)
        {
            var item = obj;

            try
            {
                dynamic items = ItemsSource;
                dynamic it = item;
                items.Remove(it);
            }
            catch (RuntimeBinderException ex)
            {
                Trace.TraceError("Ошибка выполнения... " + ex.ToString());
            }
        }
        //

        static ClosableTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClosableTabControl),
                                                     new FrameworkPropertyMetadata(
                                                         typeof(ClosableTabControl)));
        }

        public ClosableTabControl()
        {
            this.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
            this._removeItemCommand = new RelayCommand<object>(RemoveItem);
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                if (ItemsSource != null)
                {
                    foreach (var item in Items.SourceCollection)
                    {
                        ClosableTabItem containerItem = this.ItemContainerGenerator.ContainerFromItem(item) as ClosableTabItem;
                        if (containerItem == null)
                        {
                            this.UpdateLayout();
                            containerItem = this.ItemContainerGenerator.ContainerFromItem(item) as ClosableTabItem;
                        }

                        if (containerItem != null)
                        {
                            if (DeleteCommand != null)
                            {
                                containerItem.DeleteCommand = DeleteCommand;
                            }
                            else
                                containerItem.DeleteCommand = _removeItemCommand;

                            containerItem.DeleteCommandParameter = item;
                            containerItem.Focus();
                        }
                    }
                }
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ClosableTabItem);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ClosableTabItem();
        }
    }
}
