using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCustomControlLibrary.Controls;

namespace ControlsDemo
{
    public class TreeUser
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<TreeUser> TreeUsers { get; set; } = new ObservableCollection<TreeUser>()
        {
            new TreeUser() { Id = 1, Name = "Павел1", Surname = "Кувшинчиков1" },
            new TreeUser() { Id = 2, Name = "Павел2", Surname = "Кувшинчиков2", ParentId = 1 },
            new TreeUser() { Id = 3, Name = "Павел3", Surname = "Кувшинчиков3", ParentId = 1 },
            new TreeUser() { Id = 4, Name = "Павел4", Surname = "Кувшинчиков4"},
            new TreeUser() { Id = 5, Name = "Павел5", Surname = "Кувшинчиков5", ParentId = 4 },
            new TreeUser() { Id = 6, Name = "Павел6", Surname = "Кувшинчиков6", ParentId = 5 },
            new TreeUser() { Id = 7, Name = "Павел7", Surname = "Кувшинчиков7", ParentId = 5 },
            new TreeUser() { Id = 8, Name = "Павел8", Surname = "Кувшинчиков8", ParentId = 1 },
        };

        public ObservableCollection<int> SelectedValueUsers { get; set; } = new ObservableCollection<int>() { 3, 6, 7 };

        public ObservableCollection<TreeUser> SelectedItemUsers { get; set; } = new();

        public TypeInt32 PaginationCount { get; set; } = new TypeInt32(150);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void PaginationPanel_SelectionPageChanged(object sender, PageChangedEventArgs e)
        {
            var page = e.PageNumber;
            var start = e.FromStart;
            var end = e.ToEnd;
            var countData = e.SelectedCountDataPerPage;
            var allDatacount = e.TotalElementCount;
        }
    }
}
