using WpfCustomControlLibrary.Services.Commands;
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
using System.Windows.Data;

namespace WpfCustomControlLibrary.Controls
{
    [TemplatePart(Name = PART_Dropdown_Popup, Type = typeof(Popup))]
    [TemplatePart(Name = PART_Dropdown_ListBox, Type = typeof(ExtendedListBox))]
    [TemplatePart(Name = PART_Dropdown_CheckBox, Type = typeof(CheckBox))]
    [TemplatePart(Name = PART_Dropdown_SearchTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Dropdown_CloseButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_MultiSelectComboBox_Button, Type = typeof(Button))]
    [TemplatePart(Name = PART_MultiSelectComboBox_ItemsControl, Type = typeof(ExtendedItemsControl))] 
    public class MultiselectComboBox : Control
    {
        #region Events
        //Событие отслеживающее изменения выбора элементов
        public event MultiselectComboBoxEventHandler SelectionChanged;

        public delegate void MultiselectComboBoxEventHandler(object sender, SelectionChangedEventArgs e);

        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged.Invoke(this, e);
            }
        }
        #endregion

        #region Parts Const
        private const string PART_Dropdown_Popup = "PART_Dropdown_Popup";
        private const string PART_Dropdown_ListBox = "PART_Dropdown_ListBox";
        private const string PART_Dropdown_CheckBox = "PART_Dropdown_CheckBox";
        private const string PART_Dropdown_SearchTextBox = "PART_Dropdown_SearchTextBox";
        private const string PART_Dropdown_CloseButton = "PART_Dropdown_CloseButton";
        private const string PART_MultiSelectComboBox_Button = "PART_MultiSelectComboBox_Button";
        private const string PART_MultiSelectComboBox_ItemsControl = "PART_MultiSelectComboBox_ItemsControl";
        #endregion

        #region Parts
        private Popup _popup;
        private ExtendedListBox _extendedListBox;
        private CheckBox _checkBox;
        private TextBox _searchTextBox;
        private Button _closeButton;
        private Button _mscButton;
        private ExtendedItemsControl _mscItemsControl;
        #endregion

        #region Fields
        private bool _isSelectedValuesChange;
        private bool _isInitialize;
        private CollectionView _collectionView;
        private Window _parentWindow;
        private RelayCommand<object> _deleteCommand;
        #endregion

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark",
          typeof(object), typeof(MultiselectComboBox), new UIPropertyMetadata(null));
        public object Watermark
        {
            get { return (object)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register("WatermarkTemplate",
          typeof(DataTemplate), typeof(MultiselectComboBox), new UIPropertyMetadata(null));
        public DataTemplate WatermarkTemplate
        {
            get { return (DataTemplate)GetValue(WatermarkTemplateProperty); }
            set { SetValue(WatermarkTemplateProperty, value); }
        }

        public static readonly DependencyProperty DisplayModeProperty =
           DependencyProperty.Register("DisplayMode", typeof(DisplayModeView), typeof(MultiselectComboBox),
               new FrameworkPropertyMetadata(DisplayModeView.String));
        public DisplayModeView DisplayMode
        {
            get { return (DisplayModeView)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(MultiselectComboBox),
                new FrameworkPropertyMetadata(string.Empty));
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        public static readonly DependencyProperty FilterMemberPathProperty =
           DependencyProperty.Register("FilterMemberPath", typeof(string), typeof(MultiselectComboBox),
               new FrameworkPropertyMetadata(string.Empty));
        public string FilterMemberPath
        {
            get { return (string)GetValue(FilterMemberPathProperty); }
            set { SetValue(FilterMemberPathProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
              DependencyProperty.Register("SelectedItem", typeof(object), typeof(MultiselectComboBox),
                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedValueProperty =
               DependencyProperty.Register("SelectedValue", typeof(object), typeof(MultiselectComboBox),
                       new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        public static readonly DependencyProperty SelectedIndexProperty =
               DependencyProperty.Register("SelectedIndex", typeof(int), typeof(MultiselectComboBox),
                       new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectedValuePathProperty =
               DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(MultiselectComboBox),
                       new FrameworkPropertyMetadata(String.Empty));
        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        public static readonly DependencyProperty SelectionModeProperty =
               DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(MultiselectComboBox),
                       new FrameworkPropertyMetadata(SelectionMode.Single));
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
                DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(MultiselectComboBox),
                        new FrameworkPropertyMetadata((DataTemplate)null));
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(MultiselectComboBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnItemSourcePropertyChanged));

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private static void OnItemSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //MultiselectComboBox comboBox = (MultiselectComboBox)d;

            //if (comboBox == null) { return; }

            //if (comboBox != null && comboBox._isInitialize)
            //{
            //    if (comboBox._extendedListBox != null)
            //    {
            //        comboBox._extendedListBox.ItemsSource = comboBox.ItemsSource;
            //    }

            //    comboBox.InitializeCollectionViewSource();
            //}

            MultiselectComboBox comboBox = (MultiselectComboBox)d;

            if (comboBox == null) { return; }

            if (comboBox != null)/*&& comboBox._isInitialize*/
            {
                comboBox.InitializeCollectionViewSource();

                if (comboBox._extendedListBox != null)
                {
                    comboBox._extendedListBox.ItemsSource = comboBox._collectionView;
                }
            }
        }


        public static readonly DependencyProperty SelectedValuesProperty = DependencyProperty.Register("SelectedValues",
           typeof(INotifyCollectionChanged), typeof(MultiselectComboBox), new FrameworkPropertyMetadata(new ObservableCollection<object>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedValuesChangedCallback));
        public INotifyCollectionChanged SelectedValues
        {
            get
            {
                return (INotifyCollectionChanged)GetValue(SelectedValuesProperty);
            }
            set
            {
                SetValue(SelectedValuesProperty, value);
            }
        }

        private static void SelectedValuesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiselectComboBox comboBox = (MultiselectComboBox)d;

            if (comboBox == null) { return; }

            if (comboBox != null && comboBox._isInitialize)
            {
                comboBox._isSelectedValuesChange = true;

                if (comboBox.SelectionMode == SelectionMode.Multiple)
                {
                    comboBox.InitializeSelectedValues();
                }

                comboBox._isSelectedValuesChange = false;
            }
        }

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems",
          typeof(IList), typeof(MultiselectComboBox), new FrameworkPropertyMetadata(new ObservableCollection<object>()));
        public IList SelectedItems
        {
            get
            {
                return (IList)GetValue(SelectedItemsProperty);
            }
            private set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty IsDropDownOpenProperty =
           DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(MultiselectComboBox),
               new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(int), typeof(MultiselectComboBox),
                new FrameworkPropertyMetadata(300, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public int MaxDropDownHeight
        {
            get => (int)GetValue(MaxDropDownHeightProperty);
            set => SetValue(MaxDropDownHeightProperty, value);
        }

        public static readonly DependencyProperty StaysOpenProperty = DependencyProperty.Register("StaysOpen",
                      typeof(bool), typeof(MultiselectComboBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool StaysOpen
        {
            get { return (bool)GetValue(StaysOpenProperty); }
            set { SetValue(StaysOpenProperty, value); }
        }

        #region Ctor
        public MultiselectComboBox()
        {
            Loaded += MultiselectComboBox_Loaded;
            //_displayDictionarySelectedItems.CollectionChanged += _dictionarySelectedItems_CollectionChanged;
        }

        static MultiselectComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiselectComboBox), new FrameworkPropertyMetadata(typeof(MultiselectComboBox)));
        }
        #endregion

        private void MultiselectComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            _isInitialize = true;
            //SelectedItemsPropertyChanged += MultiselectComboBox_SelectedItemsPropertyChanged;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AttachPopup();
            AttachListBox();
            AttachCheckBox();
            AttachSearchTextBox();
            AttachCloseButton();
            AttachMscButton();
            AttachMscItemsControl();
        }

        private void AttachPopup()
        {
            if (_popup != null)
            {
                _popup.Opened -= _popup_Opened;
                if (_parentWindow != null)
                {
                    _parentWindow.LocationChanged -= _parentWindow_LocationChanged;
                }
            }

            var popup = GetTemplateChild(PART_Dropdown_Popup) as Popup;

            if (popup != null)
            {
                _popup = popup;
                _popup.Opened += _popup_Opened;
                _parentWindow = Window.GetWindow(this);
                if (_parentWindow != null)
                {
                    _parentWindow.LocationChanged += _parentWindow_LocationChanged;
                }
            }
        }

        private void _parentWindow_LocationChanged(object? sender, EventArgs e)
        {
            var offset = _popup.HorizontalOffset;
            _popup.HorizontalOffset = offset + 1;
            _popup.HorizontalOffset = offset;
        }

        private void _popup_Opened(object? sender, EventArgs e)
        {
            if (_searchTextBox != null)
            {
                _searchTextBox.Focus();
            }
        }

        private void AttachListBox()
        {
            if (_extendedListBox != null)
            {
                _extendedListBox.SelectionChanged -= _extendedListBox_SelectionChanged;
            }

            var listBox = GetTemplateChild(PART_Dropdown_ListBox) as ExtendedListBox;

            if (listBox != null)
            {
                _extendedListBox = listBox;
                _extendedListBox.ItemsSource = _collectionView; 
                this.SelectedItems = _extendedListBox.SelectedItems;

                InitializeSelectedValues();
                //test
                //InitializeDisplayText(SelectedItems);
                //
                InitializeCollectionViewSource();

                _extendedListBox.SelectionChanged += _extendedListBox_SelectionChanged;
            }
        }

      

        private void _extendedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedValues(e);

            //Todo: Событие Selectionchanged MultiselectCombobox реализовать и передать те же самые аргументы
            OnSelectionChanged(e);

            //Test
            //UpdateDisplayText(e);
        }

        private void AttachCheckBox()
        {
            if (_checkBox != null)
            {
                _checkBox.Checked -= _checkBox_Checked;
                _checkBox.Unchecked -= _checkBox_Unchecked;
            }

            var checkBox = GetTemplateChild(PART_Dropdown_CheckBox) as CheckBox;

            if (checkBox != null)
            {
                _checkBox = checkBox;
                _checkBox.Checked += _checkBox_Checked;
                _checkBox.Unchecked += _checkBox_Unchecked;
            }
        }

        private void _checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (SelectionMode == SelectionMode.Multiple && _extendedListBox != null)
            {
                _extendedListBox.UnselectAll();
            }
        }

        private void _checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectionMode == SelectionMode.Multiple && _extendedListBox != null)
            {
                _extendedListBox.SelectAll();
            }
        }

        private void AttachSearchTextBox()
        {
            if (_searchTextBox != null)
            {
                _searchTextBox.TextChanged -= _searchTextBox_TextChanged;
            }

            var textBox = GetTemplateChild(PART_Dropdown_SearchTextBox) as TextBox;

            if (textBox != null)
            {
                _searchTextBox = textBox;

                _searchTextBox.TextChanged += _searchTextBox_TextChanged;
            }
        }

        private void _searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_collectionView != null && !string.IsNullOrWhiteSpace(FilterMemberPath))
            {
                if (!string.IsNullOrWhiteSpace(_searchTextBox.Text))
                {
                    _collectionView.Filter = s =>
                    {
                        if (SelectedItems.Contains(s))
                        {
                            return true;
                        }

                        var propertyPath = FilterMemberPath;

                        var value = FollowPropertyPath(s, propertyPath);
                        if (value != null)
                        {
                            return value.ToString().IndexOf(_searchTextBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
                        }
                        return s.ToString().IndexOf(_searchTextBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
                    };
                }
                else
                    _collectionView.Filter = null;
            }
        }

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

        private void AttachCloseButton()
        {
            if (_closeButton != null)
            {
                _closeButton.Click -= _closeButton_Click;
            }

            var closeButton = GetTemplateChild(PART_Dropdown_CloseButton) as Button;

            if (closeButton != null)
            {
                _closeButton = closeButton;
                _closeButton.Click += _closeButton_Click;
            }
        }

        private void _closeButton_Click(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = false;
        }

        private void AttachMscButton()
        {
            if (_mscButton != null)
            {
                _mscButton.Click -= _mscButton_Click;
            }

            var mscButton = GetTemplateChild(PART_MultiSelectComboBox_Button) as Button;

            if(mscButton != null)
            {
                _mscButton = mscButton;
                _mscButton.Click += _mscButton_Click;
            }
        }

        private void _mscButton_Click(object sender, RoutedEventArgs e)
        {
            if (StaysOpen)
            {
                IsDropDownOpen = !IsDropDownOpen;
            }
            else
                IsDropDownOpen = true;
        }

        private void AttachMscItemsControl()
        {
            if (_mscItemsControl != null)
            {
                _deleteCommand = null;
                _mscItemsControl.DeleteCommand = null;
            }

            var itemsControl = GetTemplateChild(PART_MultiSelectComboBox_ItemsControl) as ExtendedItemsControl;

            if (itemsControl != null)
            {
                _mscItemsControl = itemsControl;
                _deleteCommand = new RelayCommand<object>(Delete);
                _mscItemsControl.DeleteCommand = _deleteCommand;
            }
        }

        public void Delete(object parametr)
        {
            if (_extendedListBox != null)
            {
                _extendedListBox.SelectedItems.Remove(parametr);
            }
        }

        //Обновление коллекций
        private void InitializeSelectedValues()
        {
            //var col = SelectedItems;
            //var stat = BindingOperations.GetBindingExpressionBase(this, MultiselectComboBox.SelectedItemsProperty)?.Status;
            //var stat1 = DependencyPropertyHelper.GetValueSource(this, MultiselectComboBox.SelectedItemsProperty).BaseValueSource;

            if (SelectionMode == SelectionMode.Multiple && SelectedValuePath != null)
            {
                if (DependencyPropertyHelper.GetValueSource(this, MultiselectComboBox.SelectedValuesProperty).BaseValueSource == BaseValueSource.Default)
                {
                    if (SelectedValues is IList collection)
                    {
                        collection.Clear();
                        var selectedValuePath = SelectedValuePath;

                        foreach (var item in _extendedListBox.SelectedItems)
                        {
                            //INotifyPropertyChanged propertychanged = item as INotifyPropertyChanged;
                            //if (propertychanged != null)
                            //{
                            //    propertychanged.PropertyChanged += OnItemPropertyChanged;
                            //}

                            var type = item.GetType();
                            var property = type.GetProperty(selectedValuePath);
                            if (property != null)
                            {
                                var value = property.GetValue(item);

                                if (value != null) { collection.Add(value); }
                            }
                        }
                    }
                }
                else if (DependencyPropertyHelper.GetValueSource(this, MultiselectComboBox.SelectedValuesProperty).BaseValueSource == BaseValueSource.Local)
                {
                    _extendedListBox.SelectedItems.Clear();

                    var selectedValuePath = SelectedValuePath;

                    if (SelectedValues is IList collection)
                    {
                        foreach (var selectedValue in collection)
                        {
                            var source = ItemsSource.Cast<object>();
                            var selectedItem = source.FirstOrDefault(item =>
                            {
                                var type = item.GetType();
                                var property = type.GetProperty(selectedValuePath);
                                if (property != null)
                                {
                                    var value = property.GetValue(item);
                                    bool result = value.Equals(selectedValue);

                                    return result;
                                }
                                else
                                {
                                    return false;
                                }
                            });

                            if (selectedItem != null && !_extendedListBox.SelectedItems.Contains(selectedItem))
                            {
                                _extendedListBox.SelectedItems.Add(selectedItem);

                                //INotifyPropertyChanged propertychanged = selectedItem as INotifyPropertyChanged;
                                //if (propertychanged != null)
                                //{
                                //    propertychanged.PropertyChanged += OnItemPropertyChanged;
                                //}
                            }
                        }
                    }
                }
            }
        }

        private void UpdateSelectedValues(SelectionChangedEventArgs e)
        {
            if (SelectedValuePath != null && SelectionMode == SelectionMode.Multiple && !_isSelectedValuesChange)
            {
                this._extendedListBox.SelectionChanged -= _extendedListBox_SelectionChanged;

                var selectedItems = _extendedListBox.SelectedItems;
                var selectedValues = SelectedValues as IList;

                if (e.RemovedItems != null && e.RemovedItems.Count > 0)
                {
                    foreach (var oldItem in e.RemovedItems)
                    {
                        //INotifyPropertyChanged propertychanged = oldItem as INotifyPropertyChanged;
                        //if (propertychanged != null)
                        //{
                        //    propertychanged.PropertyChanged -= OnItemPropertyChanged;
                        //}

                        var type = oldItem.GetType();
                        var selectedValuePath = SelectedValuePath;
                        var property = type.GetProperty(selectedValuePath);
                        if (property != null)
                        {
                            var value = property.GetValue(oldItem);
                            if (selectedValues.Contains(value))
                            {
                                selectedValues.Remove(value);
                            }
                        }
                    }
                }
                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    foreach (var newItem in e.AddedItems)
                    {
                        //INotifyPropertyChanged propertychanged = newItem as INotifyPropertyChanged;
                        //if (propertychanged != null)
                        //{
                        //    propertychanged.PropertyChanged += OnItemPropertyChanged;
                        //}

                        var type = newItem.GetType();
                        var selectedValuePath = SelectedValuePath;
                        var property = type.GetProperty(selectedValuePath);
                        if (property != null)
                        {
                            var value = property.GetValue(newItem);
                            if (!selectedValues.Contains(value))
                            {
                                selectedValues.Add(value);
                            }
                        }
                    }
                }

                this._extendedListBox.SelectionChanged += _extendedListBox_SelectionChanged;
            }
        }


        //test
        //private ObservableCollection<DictItem> _displayDictionarySelectedItems = new ObservableCollection<DictItem>();

        //private void UpdateDisplayText(SelectionChangedEventArgs e)
        //{
        //    if (e.RemovedItems != null && e.RemovedItems.Count > 0)
        //    {
        //        foreach (var oldItem in e.RemovedItems)
        //        {
        //            var deleteItem = _displayDictionarySelectedItems.FirstOrDefault(i => i.Key == oldItem);
        //            if (deleteItem != null) { _displayDictionarySelectedItems.Remove(deleteItem); }
        //        }
        //    }

        //    if (e.AddedItems != null && e.AddedItems.Count > 0)
        //    {
        //        foreach (var newItem in e.AddedItems)
        //        {
        //            if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
        //            {
        //                int indexItem = e.AddedItems.IndexOf(newItem);
        //                var value = FollowPropertyPath(newItem, DisplayMemberPath);
        //                if (value != null)
        //                {
        //                    DictItem displayItem = new DictItem { Key = newItem, Value = value, Index = indexItem };
        //                    _displayDictionarySelectedItems.Add(displayItem);
        //                }
        //            }
        //        }
        //    }
        //}


        //private void InitializeDisplayText(IList items)
        //{
        //    if (items != null && items.Count > 0)
        //    {
        //        foreach (var newItem in items)
        //        {
        //            if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
        //            {
        //                int indexItem = items.IndexOf(newItem);
        //                var value = FollowPropertyPath(newItem, DisplayMemberPath);
        //                if (value != null)
        //                {
        //                    _displayDictionarySelectedItems.Add(new DictItem { Key = newItem, Value = value, Index = indexItem });
        //                }
        //            }
        //        }
        //    }
        //}

        //private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        //{
        //    if (DisplayMemberPath == propertyChangedEventArgs.PropertyName)
        //    {
        //        var findChangeItem = _displayDictionarySelectedItems.FirstOrDefault(i => i.Key == sender);
        //        if (findChangeItem != null) 
        //        {
        //            object oldValue = findChangeItem.Value;
        //            findChangeItem.Value = FollowPropertyPath(sender, DisplayMemberPath);
        //            OnSelectedItemsPropertyChanged(new SelectedItemsPropertyArgs(findChangeItem, oldValue, findChangeItem.Value, propertyChangedEventArgs.PropertyName));
        //        }
        //    }
        //}

        //событие отслеживающее изменения свойств в обьектах SelectedItems
        //public event SelectedItemsPropertyEventHandler SelectedItemsPropertyChanged;

        //public delegate void SelectedItemsPropertyEventHandler(object sender, SelectedItemsPropertyArgs e);

        //protected virtual void OnSelectedItemsPropertyChanged(SelectedItemsPropertyArgs e)
        //{
        //    if (SelectedItemsPropertyChanged != null)
        //    {
        //        SelectedItemsPropertyChanged.Invoke(this, e);
        //    }
        //}
        //

        private void InitializeCollectionViewSource()
        {
            //if (_extendedListBox != null)
            //{
            //    _collectionView = (CollectionView)CollectionViewSource.GetDefaultView(_extendedListBox.Items);
            //}

            if (ItemsSource != null)
            {
                _collectionView = (CollectionView)CollectionViewSource.GetDefaultView(ItemsSource);
            }
        }
    }
}
