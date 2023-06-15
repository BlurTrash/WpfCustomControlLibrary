using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace WpfCustomControlLibrary.Controls
{
    [TemplatePart(Name = PART_Button, Type = typeof(Button))]
    public class ClosableTabItem : TabItem
    {
        #region Consts
        private const string PART_Button = "PART_Button";
        #endregion

        #region DependencyProperty : DeleteCommandProperty
        public ICommand? DeleteCommand
        {
            get => (ICommand?)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }
        public static readonly DependencyProperty DeleteCommandProperty
            = DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(ClosableTabItem), new PropertyMetadata(default(ICommand?)));
        #endregion

        #region DependencyProperty : DeleteCommandParameterProperty
        public object? DeleteCommandParameter
        {
            get => GetValue(DeleteCommandParameterProperty);
            set => SetValue(DeleteCommandParameterProperty, value);
        }
        public static readonly DependencyProperty DeleteCommandParameterProperty
            = DependencyProperty.Register(nameof(DeleteCommandParameter), typeof(object), typeof(ClosableTabItem), new PropertyMetadata(default(object?)));
        #endregion

        #region Events
        public event RoutedEventHandler DeleteClick
        {
            add => AddHandler(DeleteClickEvent, value);
            remove => RemoveHandler(DeleteClickEvent, value);
        }

        public static readonly RoutedEvent DeleteClickEvent
            = EventManager.RegisterRoutedEvent(nameof(DeleteClick), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ClosableTabItem));
        #endregion

        #region Properties
        public TabControl ParentContainer
        {
            get
            {
                //TabControl.ItemsControlFromItemContainer(this) as ClosableTabControl;
                var control = ItemsControl.ItemsControlFromItemContainer(this);
                var control2 = control as TabControl;
                return control2;
            }
        }
        #endregion

        #region Fields
        protected Button _button;
        #endregion

        #region Constructors
        static ClosableTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClosableTabItem),
                                                     new FrameworkPropertyMetadata(
                                                         typeof(ClosableTabItem)));
        }

        public ClosableTabItem()
        {
        }
        #endregion

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
            TabControl parent = ParentContainer;
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
