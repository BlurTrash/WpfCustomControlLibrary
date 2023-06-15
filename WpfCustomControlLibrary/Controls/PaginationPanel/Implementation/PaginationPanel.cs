using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfCustomControlLibrary.Controls
{
    public enum CountType 
    {
        Five = 5,
        Ten = 10,
        Fifteen = 15,
        Twenty = 20,
        Twentyfive = 25,
        Fifty = 50,
        OneHundred = 100
    }

    public class PageChangedEventArgs : RoutedEventArgs
    {
        private int _pageNumber;
        /// <summary>
        /// Номер новой выбранной страницы
        /// </summary>
        public int PageNumber
        {
            get { return _pageNumber; }
        }

        private int _selectedCountDataPerPage;
        /// <summary>
        /// Выбранное количество элементов на страницы
        /// </summary>
        public int SelectedCountDataPerPage
        {
            get { return _selectedCountDataPerPage; }
        }

        private int _fromStart;
        /// <summary>
        /// "C" какого выбрано включительно
        /// </summary>
        public int FromStart
        {
            get { return _fromStart; }
        }

        private int _toEnd;
        /// <summary>
        /// "По" какой выбрано включительно
        /// </summary>
        public int ToEnd
        {
            get { return _toEnd; }
        }

        private int _totalElementCount;
        /// <summary>
        /// "По" какой выбрано включительно
        /// </summary>
        public int TotalElementCount
        {
            get { return _totalElementCount; }
        }

        public PageChangedEventArgs(RoutedEvent id, int pageNumber, int selectedCountDataPerPage, int fromStart, int toEnd, int totalElementCount)
        {
            _selectedCountDataPerPage = selectedCountDataPerPage;
            _pageNumber = pageNumber;
            _fromStart = fromStart;
            _toEnd = toEnd;
            _totalElementCount = totalElementCount;
            RoutedEvent = id;
        }
    }

    public class TypeInt32
    {
        public int Value { get; set; }

        public TypeInt32(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    [TemplatePart(Name = "PART_ComboBox", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_ListBoxPages", Type = typeof(ComboBox))]
    [TemplatePart(Name = "PART_ButtonFirstPage", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ButtonPrePage", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ButtonNextPage", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ButtonLastPage", Type = typeof(Button))]
    public class PaginationPanel : Control
    {
        #region Events
        public static readonly RoutedEvent SelectionPageChangedEvent =
          EventManager.RegisterRoutedEvent("SelectionPageChanged", RoutingStrategy.Direct,
                        typeof(PageChangedEventHandler), typeof(PaginationPanel));

        public event PageChangedEventHandler SelectionPageChanged
        {
            add { AddHandler(SelectionPageChangedEvent, value); }
            remove { RemoveHandler(SelectionPageChangedEvent, value); }
        }

        public delegate void PageChangedEventHandler(object sender, PageChangedEventArgs e);

        protected virtual void OnSelectionPageChanged(PageChangedEventArgs e)
        {
            RaiseEvent(e);
        }
        #endregion

        static PaginationPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PaginationPanel), new FrameworkPropertyMetadata(
                                                         typeof(PaginationPanel)));
        }
        public PaginationPanel()
        {
            Loaded += PaginationPanel_Loaded;
        }

        private void PaginationPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _isInitialize = true;
            if (IsLoaded && TotalDataCount.Value != 0)
            {
                OnSelectionPageChanged(new PageChangedEventArgs(SelectionPageChangedEvent, CurrentPageNumber, PageDataCount, ShowingPageDataStartNumber, ShowingPageDataEndNumber, TotalDataCount.Value));
            }
            Loaded -= PaginationPanel_Loaded;
        }

        /// <summary>
        /// Whether to display the data volume selection control on each page
        /// </summary>
        public static readonly DependencyProperty IsShowPageDataCountSelectorProperty = DependencyProperty.Register("IsShowPageDataCountSelector", typeof(bool), typeof(PaginationPanel),
            new PropertyMetadata(true, null));
        public bool IsShowPageDataCountSelector
        {
            get { return (bool)GetValue(IsShowPageDataCountSelectorProperty); }
            set { SetValue(IsShowPageDataCountSelectorProperty, value); }
        }

        /// <summary>
        /// Selectable collection of data items displayed on each page
        /// </summary>
        public static readonly DependencyProperty PageDataCountCollectionProperty = DependencyProperty.Register("PageDataCountCollection", typeof(ObservableCollection<int>), typeof(PaginationPanel),
            new PropertyMetadata(new ObservableCollection<int> { 5, 10, 15, 20, 25, 50, 100 }, null)); /*new ObservableCollection<int> { 20, 30, 50 }*/
        public ObservableCollection<int> PageDataCountCollection
        {
            get { return (ObservableCollection<int>)GetValue(PageDataCountCollectionProperty); }
            private set { SetValue(PageDataCountCollectionProperty, value); }
        }


        /// <summary>
        /// Enum, the maximum number of data displayed on each page
        /// </summary>
        public static readonly DependencyProperty PageDataCountTypeProperty = DependencyProperty.Register("PageDataCountType", typeof(CountType), typeof(PaginationPanel),
            new PropertyMetadata(CountType.Ten, OnPageDataCountTypePropertyChanged));

        public CountType PageDataCountType
        {
            get { return (CountType)GetValue(PageDataCountTypeProperty); }
            set { SetValue(PageDataCountTypeProperty, value); }
        }
        /// <summary>
        /// The maximum number of data displayed on each page
        /// </summary>
        public static readonly DependencyProperty PageDataCountProperty = DependencyProperty.Register("PageDataCount", typeof(int), typeof(PaginationPanel),
            new PropertyMetadata(10, OnPageDataCountPropertyChanged));
        public int PageDataCount
        {
            get { return (int)GetValue(PageDataCountProperty); }
            private set { SetValue(PageDataCountProperty, value); }
        }

        /// <summary>
        /// The set of page numbers currently displayed for selection
        /// </summary>
        public static readonly DependencyProperty ShowingPageNumberCollectionProperty = DependencyProperty.Register("ShowingPageNumberCollection", typeof(ObservableCollection<int>), typeof(PaginationPanel),
            new PropertyMetadata(null, null));
        public ObservableCollection<int> ShowingPageNumberCollection
        {
            get { return (ObservableCollection<int>)GetValue(ShowingPageNumberCollectionProperty); }
            set { SetValue(ShowingPageNumberCollectionProperty, value); }
        }

        /// <summary>
        /// Number of pages currently selected
        /// </summary>
        public static readonly DependencyProperty CurrentPageNumberProperty = DependencyProperty.Register("CurrentPageNumber", typeof(int), typeof(PaginationPanel),
            new PropertyMetadata(1, OnCurrentPageNumberChanged));
        public int CurrentPageNumber
        {
            get { return (int)GetValue(CurrentPageNumberProperty); }
            set { SetValue(CurrentPageNumberProperty, value); }
        }

        /// <summary>
        /// Whether to display paging information
        /// </summary>
        public static readonly DependencyProperty IsShowPageInfoProperty = DependencyProperty.Register("IsShowPageInfo", typeof(bool), typeof(PaginationPanel),
            new PropertyMetadata(true, null));
        public bool IsShowPageInfo
        {
            get { return (bool)GetValue(IsShowPageInfoProperty); }
            set { SetValue(IsShowPageInfoProperty, value); }
        }

        /// <summary>
        /// Total data volume
        /// </summary>
        public static readonly DependencyProperty TotalDataCountProperty = DependencyProperty.Register("TotalDataCount", typeof(TypeInt32), typeof(PaginationPanel),
            new PropertyMetadata(new TypeInt32(0), OnTotalDataCountChanged));

        public TypeInt32 TotalDataCount
        {
            get { return (TypeInt32)GetValue(TotalDataCountProperty); }
            set { SetValue(TotalDataCountProperty, value); }
        }

        /// <summary>
        /// The number of data items displayed on the current page
        /// </summary>
        public static readonly DependencyProperty CurrentPageDataCountProperty = DependencyProperty.Register("CurrentPageDataCount", typeof(int), typeof(PaginationPanel),
            new PropertyMetadata(0, null));
        public int CurrentPageDataCount
        {
            get { return (int)GetValue(CurrentPageDataCountProperty); }
            set { SetValue(CurrentPageDataCountProperty, value); }
        }

        /// <summary>
        /// total pages 
        /// </summary>
        public static readonly DependencyProperty TotalPageCountProperty = DependencyProperty.Register("TotalPageCount", typeof(int), typeof(PaginationPanel),
            new PropertyMetadata(1, null));
        public int TotalPageCount
        {
            get { return (int)GetValue(TotalPageCountProperty); }
            set { SetValue(TotalPageCountProperty, value); }
        }

        /// <summary>
        /// The data start number of the currently displayed page
        /// </summary>
        public static readonly DependencyProperty ShowingPageDataStartNumberProperty = DependencyProperty.Register("ShowingPageDataStartNumber", typeof(int), typeof(PaginationPanel),
            new PropertyMetadata(0, null));
        public int ShowingPageDataStartNumber
        {
            get { return (int)GetValue(ShowingPageDataStartNumberProperty); }
            set { SetValue(ShowingPageDataStartNumberProperty, value); }
        }

        /// <summary>
        /// Data end number of the currently displayed page
        /// </summary>
        public static readonly DependencyProperty ShowingPageDataEndNumberProperty = DependencyProperty.Register("ShowingPageDataEndNumber", typeof(int), typeof(PaginationPanel),
            new PropertyMetadata(0, null));
        public int ShowingPageDataEndNumber
        {
            get { return (int)GetValue(ShowingPageDataEndNumberProperty); }
            set { SetValue(ShowingPageDataEndNumberProperty, value); }
        }

        /// <summary>
        /// The maximum number of selectable pages displayed
        /// </summary>
        public static readonly DependencyProperty MaxShownPageCountProperty = DependencyProperty.Register("MaxShownPageCount", typeof(int), typeof(PaginationPanel),
            new PropertyMetadata(3, null));
        public int MaxShownPageCount
        {
            get { return (int)GetValue(MaxShownPageCountProperty); }
            set { SetValue(MaxShownPageCountProperty, value); }
        }

        /// <summary>
        /// The background color of the selected page
        /// </summary>
        //public static readonly DependencyProperty SelectedPageBackgroundProperty = DependencyProperty.Register("SelectedPageBackground", typeof(Brush), typeof(PaginationPanel),
        //    new PropertyMetadata(new SolidColorBrush(Colors.Red), null));
        //public Brush SelectedPageBackground
        //{
        //    get { return (Brush)GetValue(SelectedPageBackgroundProperty); }
        //    set { SetValue(SelectedPageBackgroundProperty, value); }
        //}

        ///// <summary>
        ///// The background color of the page number not selected
        ///// </summary>
        //public static readonly DependencyProperty PageSelectorBackgroundProperty = DependencyProperty.Register("PageSelectorBackground", typeof(Brush), typeof(PaginationPanel),
        //    new PropertyMetadata(null, null));
        //public Brush PageSelectorBackground
        //{
        //    get { return (Brush)GetValue(PageSelectorBackgroundProperty); }
        //    set { SetValue(PageSelectorBackgroundProperty, value); }
        //}

        /// <summary>
        /// The callback method when the currently selected page number changes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnCurrentPageNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PaginationPanel pagination = d as PaginationPanel;
            if (pagination == null)
            {
                return;
            }

            if (pagination._lstShowingPage != null)
            {
                pagination._lstShowingPage.SelectedItem = e.NewValue;
            }

            pagination.SetBtnEnable();
        }

        //Добавил
        private static void OnTotalDataCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PaginationPanel pagination = d as PaginationPanel;
            if (pagination == null)
            {
                return;
            }
            
            if (pagination._isInitialize) //pagination.IsLoaded
            {
                pagination.InitData();
            }
        }

        private static void OnPageDataCountTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PaginationPanel pagination = d as PaginationPanel;
            if (pagination == null)
            {
                return;
            }

            var pageDataCount = (int)e.NewValue;
            
            pagination.PageDataCount = pageDataCount;

            //Добавил
            if (pagination._cbbPageDataCount != null)
            {
                pagination._cbbPageDataCount.SelectedItem = pageDataCount;
            }
            //
        }

        /// <summary>
        /// The callback method when the maximum amount of data displayed on each page changes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnPageDataCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PaginationPanel pagination = d as PaginationPanel;
            if (pagination == null)
            {
                return;
            }
            
            if (pagination.IsLoaded)
            {
                pagination.InitData();
            }
            //pagination.InitData();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //Initialize the control
            InitControls();

            //Initialization data 
            ShowingPageNumberCollection = new ObservableCollection<int>();
            InitData();
        }

        private ComboBox _cbbPageDataCount;
        private ListBox _lstShowingPage;
        private Button _btnFirstPage;
        private Button _btnPrePage;
        private Button _btnNextPage;
        private Button _btnLastPage;

        private bool _isIgnoreListBoxSelectionChanged;
        private bool _isInitialize;
        private object _lock = new();

        /// <summary>
        /// Initialize the control
        /// </summary>
        private void InitControls()
        {
            _cbbPageDataCount = GetTemplateChild("PART_ComboBox") as ComboBox;
            if (_cbbPageDataCount != null)
            {
                _cbbPageDataCount.SelectedItem = PageDataCount;
                _cbbPageDataCount.SelectionChanged += _cbbPageDataCount_SelectionChanged;
            }

            _lstShowingPage = GetTemplateChild("PART_ListBoxPages") as ListBox;
            if (_lstShowingPage != null)
            {
                _lstShowingPage.SelectionChanged += _lstShowingPage_SelectionChanged;
            }

            _btnFirstPage = GetTemplateChild("PART_ButtonFirstPage") as Button;
            if (_btnFirstPage != null)
            {
                _btnFirstPage.Click += _btnFirstPage_Click;
            }

            _btnPrePage = GetTemplateChild("PART_ButtonPrePage") as Button;
            if (_btnPrePage != null)
            {
                _btnPrePage.Click += _btnPrePage_Click;
            }

            _btnNextPage = GetTemplateChild("PART_ButtonNextPage") as Button;
            if (_btnNextPage != null)
            {
                _btnNextPage.Click += _btnNextPage_Click;
            }

            _btnLastPage = GetTemplateChild("PART_ButtonLastPage") as Button;
            if (_btnLastPage != null)
            {
                _btnLastPage.Click += _btnLastPage_Click;
            }
        }

        /// <summary>
        /// Initialization data 
        /// </summary>
        private void InitData()
        {
            try
            {
                _isIgnoreListBoxSelectionChanged = true;
                if (PageDataCount > 0)
                {
                    //Calculate the total number of pages based on the total amount of data and the maximum amount of data displayed on each page
                    if (TotalDataCount.Value % PageDataCount > 0)
                    {
                        TotalPageCount = TotalDataCount.Value / PageDataCount + 1;
                    }
                    else
                    {
                        TotalPageCount = TotalDataCount.Value / PageDataCount;
                    }

                    //Add the optional page number to the data binding collection
                    if (ShowingPageNumberCollection != null)
                    {
                        lock (_lock)
                        {
                            ShowingPageNumberCollection.Clear();
                            int addPageCount = MaxShownPageCount;
                            if (TotalPageCount < MaxShownPageCount)
                            {
                                addPageCount = TotalPageCount;
                            }

                            for (int i = 1; i <= addPageCount; i++)
                            {
                                ShowingPageNumberCollection.Add(i);
                            }
                        }
                    }

                    //Initialize the selected page
                    if (_lstShowingPage != null)
                    {
                        _lstShowingPage.SelectedIndex = 0;
                        CurrentPageNumber = 1;
                    }

                    //Update paging data information
                    UpdateShowingPageInfo();
                }

                SetBtnEnable();
            }
            finally
            {
                _isIgnoreListBoxSelectionChanged = false;

                if (_isInitialize && TotalDataCount.Value != 0)
                {
                    OnSelectionPageChanged(new PageChangedEventArgs(SelectionPageChangedEvent, CurrentPageNumber, PageDataCount, ShowingPageDataStartNumber, ShowingPageDataEndNumber, TotalDataCount.Value));
                }
            }
        }


        private void _cbbPageDataCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbb = sender as ComboBox;
            if (cbb == null || cbb.SelectedItem == null)
            {
                return;
            }

            string selectedCountString = cbb.SelectedItem.ToString();
            if (!int.TryParse(selectedCountString, out int selectedDataCount))
            {
                return;
            }

            if (PageDataCount != selectedDataCount)
            {
                PageDataCount = selectedDataCount;

            }
            //PageDataCount = selectedDataCount;

            //InitData();
        }
        private void _lstShowingPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isIgnoreListBoxSelectionChanged)
            {
                return;
            }

            try
            {
                _isIgnoreListBoxSelectionChanged = true;

                ListBox lst = sender as ListBox;
                if (lst == null || lst.SelectedItem == null)
                {
                    return;
                }

                string selectedPageString = lst.SelectedItem.ToString();
                if (!int.TryParse(selectedPageString, out int selectedPageNumber))
                {
                    return;
                }

                //The total number of pages is less than the maximum number of displayable pages, indicating that no matter what the number of pages is selected, there is no need to change the data in the page number collection, and return directly at this time
                if (TotalPageCount <= MaxShownPageCount)
                {
                    CurrentPageNumber = selectedPageNumber;
                    UpdateShowingPageInfo();
                    return;
                }

                //Calculate the number of times you need to move to the right to keep the selected item in the center. Compare the middle position with the index of the selected item
                int moveCount = MaxShownPageCount / 2 - _lstShowingPage.SelectedIndex;
                int startPageNumber = ShowingPageNumberCollection.First();
                if (moveCount > 0) //Move to the right
                {
                    int realMoveCount = moveCount;
                    if (ShowingPageNumberCollection.First() - 1 < moveCount)
                    {
                        realMoveCount = ShowingPageNumberCollection.First() - 1;
                    }

                    startPageNumber = ShowingPageNumberCollection.First() - realMoveCount;
                }
                else if (moveCount < 0) //Move to the left
                {
                    int realMoveCount = -moveCount;
                    if (TotalPageCount - ShowingPageNumberCollection.Last() < realMoveCount)
                    {
                        realMoveCount = TotalPageCount - ShowingPageNumberCollection.Last();
                    }

                    startPageNumber = ShowingPageNumberCollection.First() + realMoveCount;
                }

                lock (_lock)
                {
                    ShowingPageNumberCollection.Clear();
                    for (int i = 0; i < MaxShownPageCount; i++)
                    {
                        ShowingPageNumberCollection.Add(startPageNumber + i);
                    }
                }

                int selectedItemIndex = ShowingPageNumberCollection.IndexOf(selectedPageNumber);
                _lstShowingPage.SelectedIndex = selectedItemIndex;

                CurrentPageNumber = selectedPageNumber;
                UpdateShowingPageInfo();
            }
            finally
            {
                _isIgnoreListBoxSelectionChanged = false;

                OnSelectionPageChanged(new PageChangedEventArgs(SelectionPageChangedEvent, CurrentPageNumber, PageDataCount, ShowingPageDataStartNumber, ShowingPageDataEndNumber, TotalDataCount.Value));
            }
        }
        /// <summary>
        /// Jump to the homepage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            if (_lstShowingPage == null
                || ShowingPageNumberCollection == null
                || ShowingPageNumberCollection.Count == 0)
            {
                return;
            }

            if (ShowingPageNumberCollection[0] != 1)
            {
                try
                {
                    _isIgnoreListBoxSelectionChanged = true;
                    lock (_lock)
                    {
                        ShowingPageNumberCollection.Clear();
                        for (int i = 1; i <= MaxShownPageCount; i++)
                        {
                            ShowingPageNumberCollection.Add(i);
                        }
                    }
                }
                finally
                {
                    _isIgnoreListBoxSelectionChanged = false;
                }
            }

            _lstShowingPage.SelectedIndex = 0;
        }
        /// <summary>
        /// Jump to the last page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnLastPage_Click(object sender, RoutedEventArgs e)
        {
            if (_lstShowingPage == null
              || ShowingPageNumberCollection == null
              || ShowingPageNumberCollection.Count == 0)
            {
                return;
            }

            if (ShowingPageNumberCollection.Last() != TotalPageCount)
            {
                try
                {
                    _isIgnoreListBoxSelectionChanged = true;
                    lock (_lock)
                    {
                        ShowingPageNumberCollection.Clear();
                        for (int i = 0; i < MaxShownPageCount; i++)
                        {
                            ShowingPageNumberCollection.Add(TotalPageCount - MaxShownPageCount + i + 1);
                        }
                    }
                }
                finally
                {
                    _isIgnoreListBoxSelectionChanged = false;
                }
            }

            _lstShowingPage.SelectedIndex = _lstShowingPage.Items.Count - 1;
        }
        /// <summary>
        /// Jump to the previous page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnPrePage_Click(object sender, RoutedEventArgs e)
        {
            if (_lstShowingPage == null
                || ShowingPageNumberCollection == null
                || ShowingPageNumberCollection.Count == 0)
            {
                return;
            }

            if (_lstShowingPage.SelectedIndex > 0)
            {
                _lstShowingPage.SelectedIndex--;
            }
        }
        /// <summary>
        /// Jump to the next
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_lstShowingPage == null
                || ShowingPageNumberCollection == null
                || ShowingPageNumberCollection.Count == 0)
            {
                return;
            }

            if (_lstShowingPage.SelectedIndex < MaxShownPageCount - 1)
            {
                _lstShowingPage.SelectedIndex++;
            }
        }


        private void UpdateShowingPageInfo()
        {
            if (TotalPageCount == 0)
            {
                ShowingPageDataStartNumber = 0;
                ShowingPageDataEndNumber = 0;
            }
            else if (CurrentPageNumber < TotalPageCount)
            {
                ShowingPageDataStartNumber = (CurrentPageNumber - 1) * PageDataCount + 1;
                ShowingPageDataEndNumber = CurrentPageNumber * PageDataCount;
            }
            else if (CurrentPageNumber == TotalPageCount)
            {
                ShowingPageDataStartNumber = (CurrentPageNumber - 1) * PageDataCount + 1;
                ShowingPageDataEndNumber = TotalDataCount.Value;
            }
        }

        /// <summary>
        /// Set button availability
        /// </summary>
        private void SetBtnEnable()
        {
            if (_btnFirstPage == null || _btnPrePage == null
                || _btnNextPage == null || _btnLastPage == null)
            {
                return;
            }

            _btnPrePage.IsEnabled = true;
            _btnNextPage.IsEnabled = true;
            _btnFirstPage.IsEnabled = true;
            _btnLastPage.IsEnabled = true;

            if (ShowingPageNumberCollection == null || ShowingPageNumberCollection.Count == 0)//The collection is empty or no data, all buttons are unavailable
            {
                _btnPrePage.IsEnabled = false;
                _btnNextPage.IsEnabled = false;
                _btnFirstPage.IsEnabled = false;
                _btnLastPage.IsEnabled = false;
            }
            else
            {
                if (CurrentPageNumber == 1)
                {
                    _btnFirstPage.IsEnabled = false;
                    _btnPrePage.IsEnabled = false;
                }

                if (CurrentPageNumber == TotalPageCount)
                {
                    _btnNextPage.IsEnabled = false;
                    _btnLastPage.IsEnabled = false;
                }
            }
        }
    }
}
