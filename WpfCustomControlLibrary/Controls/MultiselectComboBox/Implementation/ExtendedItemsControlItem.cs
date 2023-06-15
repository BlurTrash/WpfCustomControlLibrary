using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace WpfCustomControlLibrary.Controls
{
    [TemplatePart(Name = PART_Button, Type = typeof(Button))]
    public class ExtendedItemsControlItem : ContentControl
    {
        #region Consts
        private const string PART_Button = "PART_Button";
        #endregion

        #region DependencyProperty : DeleteCommandProperty
        public static readonly DependencyProperty DeleteCommandProperty
            = DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(ExtendedItemsControlItem), new PropertyMetadata(default(ICommand?)));
        public ICommand? DeleteCommand
        {
            get => (ICommand?)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }
        #endregion

        #region DependencyProperty : DeleteCommandParameterProperty
        public static readonly DependencyProperty DeleteCommandParameterProperty
            = DependencyProperty.Register(nameof(DeleteCommandParameter), typeof(object), typeof(ExtendedItemsControlItem), new PropertyMetadata(default(object?)));
        public object? DeleteCommandParameter
        {
            get => GetValue(DeleteCommandParameterProperty);
            set => SetValue(DeleteCommandParameterProperty, value);
        }
        #endregion

        #region Events
        public static readonly RoutedEvent DeleteClickEvent
            = EventManager.RegisterRoutedEvent(nameof(DeleteClick), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedItemsControlItem));
        public event RoutedEventHandler DeleteClick
        {
            add => AddHandler(DeleteClickEvent, value);
            remove => RemoveHandler(DeleteClickEvent, value);
        }
        #endregion

        #region Properties
        public ExtendedItemsControl ParentContainer
        {
            get { return ItemsControl.ItemsControlFromItemContainer(this) as ExtendedItemsControl; }
        }
        #endregion

        #region Fields
        protected Button _button;
        #endregion

        public ExtendedItemsControlItem()
        {
        }

        static ExtendedItemsControlItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedItemsControlItem),
                                                    new FrameworkPropertyMetadata(
                                                        typeof(ExtendedItemsControlItem)));
        }


        #region OnApplyTemplate
        public override void OnApplyTemplate()
        {
            AttachButton();
            base.OnApplyTemplate();
        }

        private void AttachButton()
        {
            if (_button != null)
            {
                _button.Click -= DeleteButtonOnClick;

            }

            _button = GetTemplateChild("PART_Button") as Button;

            if (_button != null)
            {
                _button.Click += DeleteButtonOnClick;
            }
        }

        protected virtual void OnDeleteClick()
        {
            RaiseEvent(new RoutedEventArgs(DeleteClickEvent, this));

            if (DeleteCommand?.CanExecute(DeleteCommandParameter) ?? false)
            {
                DeleteCommand.Execute(DeleteCommandParameter);
            }
        }

        private void DeleteButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            ExtendedItemsControl parent = ParentContainer;
            if (parent.ItemsSource == null)
            {
                parent.Items.Remove(this);
            }
            else
            {
                OnDeleteClick();
                routedEventArgs.Handled = true;
            }
        }
        #endregion
    }
}
