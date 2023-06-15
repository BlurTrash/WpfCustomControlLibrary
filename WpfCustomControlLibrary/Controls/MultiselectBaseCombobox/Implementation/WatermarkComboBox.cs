using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WpfCustomControlLibrary.Services.Commands;

namespace WpfCustomControlLibrary.Controls
{
    [TemplatePart(Name = "PART_ChipsPanel", Type = typeof(ChipsPanel))]
    [TemplatePart(Name = "ListView", Type = typeof(ListView))]
    [TemplatePart(Name = "PART_EditableTextBox", Type = typeof(WatermarkTextBox))]
    public abstract class WatermarkComboBox : ComboBox
    {

        #region Ctor
        public WatermarkComboBox()
        {
        }

        #endregion

        #region Events
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            e.Handled = !IsDropDownOpen;

            if (!e.Handled)
            {
                base.OnPreviewMouseWheel(e);
            }
            else
            {
                if (CanSelectMultiple)
                {
                    var scrollViwer = _chipsPanel.GetScroll();

                    if (scrollViwer != null)
                    {
                        if (e.Delta < 0)
                        {
                            scrollViwer.ScrollToVerticalOffset(scrollViwer.VerticalOffset + 60);
                        }
                        else if (e.Delta > 0)
                        {
                            scrollViwer.ScrollToVerticalOffset(scrollViwer.VerticalOffset - 60);
                        }
                    }
                }
            }
        }
        #endregion

        #region Watermark
        public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register("WatermarkText",
           typeof(string), typeof(WatermarkComboBox), new PropertyMetadata(null));

        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
        }
        #endregion

        #region ViewBase
        public static readonly DependencyProperty ViewProperty = DependencyProperty.Register("View",
            typeof(ViewBase), typeof(WatermarkComboBox));

        public ViewBase View
        {
            get { return (ViewBase)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }
        #endregion

        #region SelectionMode
        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register("SelectionMode",
                       typeof(SelectionMode), typeof(WatermarkComboBox), new FrameworkPropertyMetadata(SelectionMode.Single, new PropertyChangedCallback(OnSelectionModeChanged)), new ValidateValueCallback(IsValidSelectionMode));

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WatermarkComboBox comboBox = (WatermarkComboBox)d;
            var selectionMode = (SelectionMode)e.NewValue;
            if (selectionMode == SelectionMode.Multiple || selectionMode == SelectionMode.Extended)
            {
                comboBox.CanSelectMultiple = true;
                if (comboBox._listView != null)
                {
                    comboBox._listView.SelectionChanged -= comboBox._listView_CloseDropDown;

                }
            }
            else
            {
                comboBox.CanSelectMultiple = false;
                if (comboBox._listView != null)
                {
                    comboBox._listView.SelectionChanged += comboBox._listView_CloseDropDown;
                }
            }
        }

        private static bool IsValidSelectionMode(object o)
        {
            SelectionMode value = (SelectionMode)o;
            return value == SelectionMode.Single || value == SelectionMode.Multiple || value == SelectionMode.Extended;
        }

        public static readonly DependencyProperty CanSelectMultipleProperty = DependencyProperty.Register("CanSelectMultiple",
                      typeof(bool), typeof(WatermarkComboBox), new FrameworkPropertyMetadata(false));
        internal bool CanSelectMultiple
        {
            get { return (bool)GetValue(CanSelectMultipleProperty); }
            set { SetValue(CanSelectMultipleProperty, value); }
        }
        #endregion

        #region SelectedItems
        //public static readonly DependencyProperty ListViewSelectedItemsProperty = DependencyProperty.Register("ListViewSelectedItems",
        //   typeof(ObservableCollection<object>), typeof(WatermarkComboBox), new FrameworkPropertyMetadata(new ObservableCollection<object>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)); //ListViewSelectedItemsChanged

        //public ObservableCollection<object> ListViewSelectedItems
        //{
        //    get { return (ObservableCollection<object>)GetValue(ListViewSelectedItemsProperty); }
        //    set { SetValue(ListViewSelectedItemsProperty, value); }
        //}

        //public static readonly DependencyProperty ListViewSelectedItemsProperty = DependencyProperty.Register("ListViewSelectedItems",
        //  typeof(IList), typeof(WatermarkComboBox), new FrameworkPropertyMetadata(default(IList)));

        //public IList ListViewSelectedItems
        //{
        //    get { return (IList)GetValue(ListViewSelectedItemsProperty); }
        //    private set { SetValue(ListViewSelectedItemsProperty, value); }
        //}

        public static readonly DependencyProperty ListViewSelectedItemsProperty = DependencyProperty.Register("ListViewSelectedItems",
           typeof(INotifyCollectionChanged), typeof(WatermarkComboBox), new FrameworkPropertyMetadata(new ObservableCollection<object>())); //ListViewSelectedItemsChanged

        public IList ListViewSelectedItems
        {
            get { return (IList)GetValue(ListViewSelectedItemsProperty); }
            private set { SetValue(ListViewSelectedItemsProperty, value); }
        }
        #endregion

        #region Fields
        private ChipsPanel _chipsPanel;
        private ListView _listView;
        private WatermarkTextBox _textBox;
        private RelayCommand<object> _deleteCommand;

        private bool isSelectedValuesChange;
        #endregion

        #region IsLoaded
        public static readonly DependencyProperty IsSearchProperty = DependencyProperty.Register("IsSearch",
          typeof(bool), typeof(WatermarkComboBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)); //ListViewSelectedItemsChanged

        public bool IsSearch
        {
            get { return (bool)GetValue(IsSearchProperty); }
            set { SetValue(IsSearchProperty, value); }
        }

        //public bool IsSearch { get; protected set; } = false;
        #endregion

        #region ApplyTemplate
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AttachChipsPanel();
            AttachListView();
            AttachTextBox();
        }

        private void AttachChipsPanel()
        {
            var chipsPanel = GetTemplateChild("PART_ChipsPanel") as ChipsPanel;
            if (chipsPanel != null)
            {
                _chipsPanel = chipsPanel;
                _deleteCommand = new RelayCommand<object>(Delete);
                _chipsPanel.DeleteCommand = _deleteCommand;
                _chipsPanel.TextChanged += _chipsPanel_TextChanged;
            }
        }

        private void _chipsPanel_TextChanged(object sender, RoutedEventArgs e)
        {
            if (CanSelectMultiple)
            {
                var chipsPanel = sender as ChipsPanel;

                if (chipsPanel.Text.Length > 0)
                {
                    IsDropDownOpen = true;
                }
                else
                {
                    IsDropDownOpen = false;
                    //Добавил
                    chipsPanel.Focus();
                }

                CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(Items);
                cv.Filter = s =>
                {
                    if (ListViewSelectedItems.Contains(s))
                    {
                        return true;
                    }

                    //var type = s.GetType();
                    //var displayMember = DisplayMemberPath;
                    //var property = type.GetProperty(displayMember);

                    //if (property != null)
                    //{
                    //    var value = property.GetValue(s);
                    //    if (value != null)
                    //    {
                    //        return value.ToString().IndexOf(chipsPanel.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
                    //    }
                    //}
                    //return s.ToString().IndexOf(chipsPanel.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;

                    var propertyPath = DisplayMemberPath;

                    if (!string.IsNullOrWhiteSpace(propertyPath))
                    {
                        var value = FollowPropertyPath(s, propertyPath);
                        if (value != null)
                        {
                            return value.ToString().IndexOf(chipsPanel.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
                        }
                    }
                    return s.ToString().IndexOf(chipsPanel.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
                };
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

        public void Delete(object parametr)
        {
            _listView.SelectedItems.Remove(parametr);
        }

        private void AttachListView()
        {
            var element = GetTemplateChild("ListView") as ListView;

            if (element != null)
            {
                _listView = element;

                if (!CanSelectMultiple)
                {
                    _listView.SelectionChanged += _listView_CloseDropDown;
                }

                _listView.SelectionChanged += _listView_SelectionChanged;
                //Добавил
                UpdateSelectedItems();

                //Добавил
                this.ListViewSelectedItems = _listView.SelectedItems;

                _listView.SizeChanged += _listView_SizeChanged;
            }
        }

        private void _listView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            if (gView != null)
            {
                var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
                int widthIsNotNuNCount = 0;

                foreach (var item in gView.Columns)
                {
                    if (item.Width is not Double.NaN)
                    {
                        workingWidth -= item.Width;
                        widthIsNotNuNCount++;
                    }
                }

                var actualElementWidth = workingWidth / (gView.Columns.Count - widthIsNotNuNCount);

                foreach (var item in gView.Columns)
                {
                    if (item.Width is Double.NaN)
                    {
                        item.Width = actualElementWidth;
                    }
                    item.Width -= 1;
                }
            }
        }

        private void _listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView.SelectedItem != null)
            {
                e.Handled = true;
            }
            
            if (SelectedValuePath != null && CanSelectMultiple && !isSelectedValuesChange)
            {
                this._listView.SelectionChanged -= _listView_SelectionChanged;

                var selectedItems = _listView.SelectedItems;

                if (e.RemovedItems != null && e.RemovedItems.Count > 0)
                {
                    foreach (var oldItem in e.RemovedItems)
                    {
                        //if (ListViewSelectedItems.Contains(oldItem))
                        //{
                        //    ListViewSelectedItems.Remove(oldItem);
                        //}

                        var type = oldItem.GetType();
                        var selectedValuePath = SelectedValuePath;
                        var property = type.GetProperty(selectedValuePath);
                        if (property != null)
                        {
                            var value = property.GetValue(oldItem);
                            if (SelectedValues.Contains(value))
                            {
                                SelectedValues.Remove(value);
                            }
                        }
                    }
                }
                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    foreach (var newItem in e.AddedItems)
                    {
                        //if (!ListViewSelectedItems.Contains(newItem))
                        //{
                        //    ListViewSelectedItems.Add(newItem);
                        //}

                        var type = newItem.GetType();
                        var selectedValuePath = SelectedValuePath;
                        var property = type.GetProperty(selectedValuePath);
                        if (property != null)
                        {
                            var value = property.GetValue(newItem);
                            if (!SelectedValues.Contains(value))
                            {
                                SelectedValues.Add(value);
                            }
                        }
                    }
                }

                this._listView.SelectionChanged += _listView_SelectionChanged;
            }




            //1 Вариант
            //if (SelectedValuePath != null && CanSelectMultiple)
            //{
            //    this._listView.SelectionChanged -= _listView_SelectionChanged;

            //    var selectedItems = _listView.SelectedItems;

            //    if (e.RemovedItems != null && e.RemovedItems.Count > 0)
            //    {
            //        foreach (var oldItem in e.RemovedItems)
            //        {
            //            var type = oldItem.GetType();
            //            var selectedValuePath = SelectedValuePath;
            //            var property = type.GetProperty(selectedValuePath);
            //            if (property != null)
            //            {
            //                var value = property.GetValue(oldItem);
            //                if (SelectedValues.Contains(value))
            //                {
            //                    SelectedValues.Remove(value);
            //                }
            //            }
            //        }
            //    }
            //    if (e.AddedItems != null && e.AddedItems.Count > 0)
            //    {
            //        foreach (var newItem in e.AddedItems)
            //        {
            //            var type = newItem.GetType();
            //            var selectedValuePath = SelectedValuePath;
            //            var property = type.GetProperty(selectedValuePath);
            //            if (property != null)
            //            {
            //                var value = property.GetValue(newItem);
            //                if (!SelectedValues.Contains(value))
            //                {
            //                    SelectedValues.Add(value);
            //                }
            //            }
            //        }
            //    }

            //    // 2Вариант
            //    //SelectedValues.Clear();

            //    //if (selectedItems.Count > 0)
            //    //{
            //    //    foreach (var item in selectedItems)
            //    //    {
            //    //        var type = item.GetType();
            //    //        var selectedValuePath = SelectedValuePath;
            //    //        var property = type.GetProperty(selectedValuePath);
            //    //        if (property != null)
            //    //        {
            //    //            var value = property.GetValue(item);
            //    //            if (!SelectedValues.Contains(value))
            //    //            {
            //    //                SelectedValues.Add(value);
            //    //            }
            //    //        }
            //    //    }
            //    //}

            //    this._listView.SelectionChanged += _listView_SelectionChanged;
            //}
        }


        private void _listView_CloseDropDown(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsDropDownOpen)
            {
                this.IsDropDownOpen = false;
            }
        }

        private void AttachTextBox()
        {
            var element = GetTemplateChild("PART_EditableTextBox") as WatermarkTextBox;

            if (element != null)
            {
                _textBox = element;

                _textBox.TextChanged += _textBox_TextChanged;
            }
        }

        private void _textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsEditable && SelectionMode == SelectionMode.Single)
            {
                if (string.IsNullOrWhiteSpace(_textBox.Text) && SelectedItem != null)
                {
                    SelectedItem = null;
                }
            }
        }
        #endregion

        #region SelectedValues
        public static readonly DependencyProperty SelectedValuesProperty = DependencyProperty.Register("SelectedValues",
            typeof(IList), typeof(WatermarkComboBox), new FrameworkPropertyMetadata(new ObservableCollection<object>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(SelectedValuesChangedCallback)));

        public IList SelectedValues
        {
            get
            {
                return (IList)GetValue(SelectedValuesProperty);
            }
            set
            {
                SetValue(SelectedValuesProperty, value);
            }
        }

        private static void SelectedValuesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WatermarkComboBox comboBox = (WatermarkComboBox)d;

            comboBox.isSelectedValuesChange = true;

            if (comboBox.CanSelectMultiple)
            {
                comboBox.UpdateSelectedItems();
            }

            comboBox.isSelectedValuesChange = false;
        }

        #endregion

        #region OnItemTemplateChanged
        //Добавил
        protected override void OnItemTemplateChanged(DataTemplate oldItemTemplate, DataTemplate newItemTemplate)
        {
            base.OnItemTemplateChanged(oldItemTemplate, newItemTemplate);

            if (newItemTemplate != null && IsEditable && CanSelectMultiple)
            {
                IsEditable = false;
            }
        }
        #endregion
        #region OnItemsSourceChanged

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (CanSelectMultiple)
            {
                if (_listView != null)
                {
                    _listView.ItemsSource = this.ItemsSource;
                }
                
                if (SelectedValues != null)
                {
                    if (SelectedValues.Count > 0)
                    {
                        UpdateSelectedItems();
                    }
                }
            }

            base.OnItemsSourceChanged(oldValue, newValue);
        }

        //protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        //{
        //    if (CanSelectMultiple)
        //    {
        //        var selectedValues = SelectedValues; //test
        //        var itemSource = this.ItemsSource;
        //        var listViewItemSource = _listView.ItemsSource;

        //        _listView.ItemsSource = this.ItemsSource;


        //        if (ListViewSelectedItems != null)
        //        {
        //            if (ListViewSelectedItems.Count > 0)
        //            {
        //                ListViewSelectedItems.Clear();
        //            }
        //        }

        //        UpdateSelectedItems();
        //    }

        //    base.OnItemsSourceChanged(oldValue, newValue);
        //}


        private void UpdateSelectedItems() //Новая
        {
            if (SelectedValuePath != null && ItemsSource != null && _listView != null && SelectionMode != SelectionMode.Single)
            {
                //Если меняется коллекция SelectedValues
                _listView.SelectedItems.Clear();

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

                        if (selectedItem != null && !_listView.SelectedItems.Contains(selectedItem))
                        {
                            _listView.SelectedItems.Add(selectedItem);
                        }
                    }
                }
            }
        }
        //private void UpdateSelectedItems() //обновление коллекции если множественный выбор установлен
        //{
        //    if (SelectedValuePath != null && ItemsSource != null)
        //    {
        //        var selectedValuePath = SelectedValuePath;

        //        if (SelectedValues is IList collection)
        //        {
        //            foreach (var selectedValue in collection)
        //            {
        //                var source = ItemsSource.Cast<object>();
        //                var selectedItem = source.FirstOrDefault(item =>
        //                {
        //                    var type = item.GetType();
        //                    var property = type.GetProperty(selectedValuePath);
        //                    if (property != null)
        //                    {
        //                        var value = property.GetValue(item);
        //                        bool result = value.Equals(selectedValue);

        //                        return result;
        //                    }
        //                    else
        //                    {
        //                        return false;
        //                    }
        //                });

        //                if (selectedItem != null && !ListViewSelectedItems.Contains(selectedItem))
        //                {
        //                    ListViewSelectedItems.Add(selectedItem);
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
