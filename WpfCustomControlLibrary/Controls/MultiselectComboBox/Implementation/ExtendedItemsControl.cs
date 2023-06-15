using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WpfCustomControlLibrary.Controls
{
    public enum DisplayModeView
    {
        String,
        Template,
        ChipWithBtnDelete,
        AdvancedWithTemplating
    }

    public class ItemPropertyArgs : RoutedEventArgs
    {
        private object _source;
        /// <summary>
        /// Элемент источник вызвавший событие
        /// </summary>
        public object Source
        {
            get { return _source; }
        }

        private object _oldValue;
        /// <summary>
        /// Старое значение
        /// </summary>
        public object OldValue
        {
            get { return _oldValue; }
        }

        private object _newValue;
        /// <summary>
        /// Новое значение
        /// </summary>
        public object NewValue
        {
            get { return _newValue; }
        }

        private string _propertyName;
        /// <summary>
        /// Измененное свойство
        /// </summary>
        public string PropertyName
        {
            get { return _propertyName; }
        }

        public ItemPropertyArgs(object source, object oldValue, object newValue, string propertyName)
        {
            _propertyName = propertyName;
            _source = source;
            _oldValue = oldValue;
            _newValue = newValue;
        }
    }

    public class DictItem : INotifyPropertyChanged
    {
        public int Index { get; set; } = -1;
        public object Key { get; set; }
        public object Value { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class ExtendedItemsControl : ItemsControl
    {
        #region Events
        //Событие отслеживающее изменения свойств в обьектах Items
        public event ItemPropertyEventHandler ItemPropertyChanged;

        public delegate void ItemPropertyEventHandler(object sender, ItemPropertyArgs e);

        protected virtual void OnItemPropertyChanged(ItemPropertyArgs e)
        {
            if (ItemPropertyChanged != null)
            {
                ItemPropertyChanged.Invoke(this, e);
            }
        }
        #endregion

        public static readonly DependencyProperty DisplayModeProperty =
          DependencyProperty.Register("DisplayMode", typeof(DisplayModeView), typeof(ExtendedItemsControl),
              new FrameworkPropertyMetadata(DisplayModeView.String, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public DisplayModeView DisplayMode
        {
            get { return (DisplayModeView)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        public static readonly DependencyProperty DisplayTextProperty =
            DependencyProperty.Register("DisplayText", typeof(string), typeof(ExtendedItemsControl),
                    new FrameworkPropertyMetadata(""));
        public string DisplayText
        {
            get { return (string)GetValue(DisplayTextProperty); }
            private set { SetValue(DisplayTextProperty, value); }
        }
       
        public static readonly DependencyProperty DeleteCommandProperty
            = DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(ExtendedItemsControl), new PropertyMetadata(default(ICommand?)));
        public ICommand? DeleteCommand
        {
            get => (ICommand?)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }
       
        public static readonly DependencyProperty DeleteCommandParameterProperty
            = DependencyProperty.Register(nameof(DeleteCommandParameter), typeof(object), typeof(ExtendedItemsControl), new PropertyMetadata(default(object?)));
        public object? DeleteCommandParameter
        {
            get => GetValue(DeleteCommandParameterProperty);
            set => SetValue(DeleteCommandParameterProperty, value);
        }

        #region Fields
        private ObservableCollection<DictItem> _displayDictionaryItems = new ObservableCollection<DictItem>();
        #endregion

        public ExtendedItemsControl()
        {
            _displayDictionaryItems.CollectionChanged += _displayDictionaryItems_CollectionChanged;
            ItemPropertyChanged += ExtendedItemsControl_OnItemPropertyChanged;
            ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        static ExtendedItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedItemsControl),
                                                    new FrameworkPropertyMetadata(
                                                        typeof(ExtendedItemsControl)));
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ExtendedItemsControlItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ExtendedItemsControlItem();
        }

        protected override void OnDisplayMemberPathChanged(string oldDisplayMemberPath, string newDisplayMemberPath)
        {
            base.OnDisplayMemberPathChanged(oldDisplayMemberPath, newDisplayMemberPath);

            _displayDictionaryItems.Clear();
            UpdateDisplayText(ItemsSource as IList);
        }

        #region Methods
        private object FollowPropertyPath(object value, string path)
        {
            Type currentType = value.GetType();

            foreach (string propertyName in path.Split('.'))
            {
                PropertyInfo property = currentType.GetProperty(propertyName);
                if (property != null)
                {
                    value = property.GetValue(value, null);
                    currentType = property.PropertyType;
                }
                else
                {
                    return null;
                }
            }
            return value;
        }

        private void InitializeDisplayText(IList items)
        {
            if (items != null && items.Count > 0)
            {
                foreach (var newItem in items)
                {
                    INotifyPropertyChanged propertychanged = newItem as INotifyPropertyChanged;
                    if (propertychanged != null)
                    {
                        propertychanged.PropertyChanged += OnItemPropertyChanged;
                    }

                    if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
                    {
                        int indexItem = items.IndexOf(newItem);
                        var value = FollowPropertyPath(newItem, DisplayMemberPath);
                        if (value != null)
                        {
                            _displayDictionaryItems.Add(new DictItem { Key = newItem, Value = value, Index = indexItem });
                        }
                        else
                        {
                            throw new InvalidOperationException("DisplayMemberPath not found!");
                        }
                    }
                    else
                    {
                        int indexItem = items.IndexOf(newItem);
                        _displayDictionaryItems.Add(new DictItem { Key = newItem, Value = newItem.ToString(), Index = indexItem });
                    }
                }
            }
        }

        private void UpdateDisplayText(IList items)
        {
            if (items != null && items.Count > 0)
            {
                foreach (var newItem in items)
                {
                    if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
                    {
                        int indexItem = items.IndexOf(newItem);
                        var value = FollowPropertyPath(newItem, DisplayMemberPath);
                        if (value != null)
                        {
                            _displayDictionaryItems.Add(new DictItem { Key = newItem, Value = value, Index = indexItem });
                        }
                        else
                        {
                            throw new InvalidOperationException("DisplayMemberPath not found!");
                        }
                    }
                    else
                    {
                        int indexItem = items.IndexOf(newItem);
                        _displayDictionaryItems.Add(new DictItem { Key = newItem, Value = newItem.ToString(), Index = indexItem });
                    }
                }
            }
        }

        private void UnSubscribeItemProperty(IList items)
        {
            if (items != null && items.Count > 0)
            {
                foreach (var oldItem in items)
                {
                    INotifyPropertyChanged propertychanged = oldItem as INotifyPropertyChanged;
                    if (propertychanged != null)
                    {
                        propertychanged.PropertyChanged -= OnItemPropertyChanged;
                    }
                }
            }
        }
        #endregion

        //При генерация контейнеров
        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                if (ItemsSource != null)
                {
                    foreach (var item in Items.SourceCollection)
                    {
                        ExtendedItemsControlItem containerItem = this.ItemContainerGenerator.ContainerFromItem(item) as ExtendedItemsControlItem;
                        if (containerItem == null)
                        {
                            this.UpdateLayout();
                            containerItem = this.ItemContainerGenerator.ContainerFromItem(item) as ExtendedItemsControlItem;
                        }

                        if (containerItem != null)
                        {
                            containerItem.DeleteCommand = DeleteCommand;
                            containerItem.DeleteCommandParameter = item;
                        }
                    }
                }
            }
        }

        //Изменение коллекции для изменения текстового поля отображаемых элементов
        private void _displayDictionaryItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var dict = item as DictItem;
                        if (_displayDictionaryItems.Count > 1)
                        {
                            var addedText = $", {dict.Value}";
                            DisplayText += addedText;
                        }
                        else
                            DisplayText += dict.Value.ToString();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var dict = item as DictItem;
                        string deletedText = dict.Value.ToString();
                        int indexOfSubstring = DisplayText.IndexOf(deletedText);

                        if (indexOfSubstring >= 0)
                        {
                            if (_displayDictionaryItems.Count > 0 && indexOfSubstring > 0)
                            {
                                deletedText = ", " + deletedText;
                                indexOfSubstring = DisplayText.IndexOf(deletedText);
                                DisplayText = DisplayText.Remove(indexOfSubstring, deletedText.Length);
                            }
                            else if (_displayDictionaryItems.Count > 0 && indexOfSubstring == 0)
                            {
                                deletedText = deletedText + ", ";
                                DisplayText = DisplayText.Remove(indexOfSubstring, deletedText.Length);
                            }
                            else
                            {
                                DisplayText = DisplayText.Remove(indexOfSubstring, deletedText.Length);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    DisplayText = "";
                    break;

                default: break;
            }
        }

        private void ExtendedItemsControl_OnItemPropertyChanged(object sender, ItemPropertyArgs e)
        {
            string oldValueText = e.OldValue.ToString();
            string newValueText = e.NewValue.ToString();

            int indexOfSubstring = DisplayText.IndexOf(oldValueText);

            if (indexOfSubstring >= 0)
            {
                DisplayText = DisplayText.Remove(indexOfSubstring, oldValueText.Length);
                DisplayText = DisplayText.Insert(indexOfSubstring, newValueText);
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (DisplayMemberPath == propertyChangedEventArgs.PropertyName)
            {
                var findChangeItem = _displayDictionaryItems.FirstOrDefault(i => i.Key == sender);
                if (findChangeItem != null)
                {
                    object oldValue = findChangeItem.Value;
                    findChangeItem.Value = FollowPropertyPath(sender, DisplayMemberPath);
                    OnItemPropertyChanged(new ItemPropertyArgs(findChangeItem, oldValue, findChangeItem.Value, propertyChangedEventArgs.PropertyName));
                }
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if(oldValue != null)
            {
                _displayDictionaryItems.Clear();

                UnSubscribeItemProperty(newValue as IList);
            }
            if (newValue != null)
            {
                InitializeDisplayText(newValue as IList);
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null && e.NewItems.Count > 0)
                    {
                        foreach (var newItem in e.NewItems)
                        {
                            INotifyPropertyChanged propertychanged = newItem as INotifyPropertyChanged;
                            if (propertychanged != null)
                            {
                                propertychanged.PropertyChanged += OnItemPropertyChanged;
                            }

                            if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
                            {
                                int indexItem = e.NewItems.IndexOf(newItem);
                                var value = FollowPropertyPath(newItem, DisplayMemberPath);
                                if (value != null)
                                {
                                    DictItem displayItem = new DictItem { Key = newItem, Value = value, Index = indexItem };
                                    _displayDictionaryItems.Add(displayItem);
                                }
                                else
                                {
                                    throw new InvalidOperationException("DisplayMemberPath not found!");
                                }
                            }
                            else
                            {
                                int indexItem = e.NewItems.IndexOf(newItem);
                                _displayDictionaryItems.Add(new DictItem { Key = newItem, Value = newItem.ToString(), Index = indexItem });
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null && e.OldItems.Count > 0)
                    {
                        foreach (var oldItem in e.OldItems)
                        {
                            INotifyPropertyChanged propertychanged = oldItem as INotifyPropertyChanged;
                            if (propertychanged != null)
                            {
                                propertychanged.PropertyChanged -= OnItemPropertyChanged;
                            }

                            var deleteItem = _displayDictionaryItems.FirstOrDefault(i => i.Key == oldItem);
                            if (deleteItem != null) { _displayDictionaryItems.Remove(deleteItem); }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _displayDictionaryItems.Clear();
                    break;
                default:break;
            }
        }

        //protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        //{
        //    base.PrepareContainerForItemOverride(element, item);

        //    if (this.Items.Count > 0)
        //    {
        //        if (object.ReferenceEquals(this.Items[Items.Count - 1], item))
        //        {
        //            var container = element as ExtendedItemsControlItem;
        //            container.IsLastItem = true;
        //        }
        //    }
        //}


        //protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    base.OnItemsChanged(e);

        //    if (Items.Count > 0)
        //    {
        //        var container = (ItemContainerGenerator.ContainerFromIndex(Items.Count - 1) as ExtendedItemsControlItem);
        //        if (container != null) container.IsLastItem = true;
        //    }
        //    if (Items.Count - 1 > 0)
        //    {
        //        var container = (ItemContainerGenerator.ContainerFromIndex(Items.Count - 2) as ExtendedItemsControlItem);
        //        if (container != null) container.IsLastItem = false;
        //    }
        //}
    }
}
