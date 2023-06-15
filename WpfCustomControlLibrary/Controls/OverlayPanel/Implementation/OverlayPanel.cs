using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WpfCustomControlLibrary.Services.Commands;

namespace WpfCustomControlLibrary.Controls
{

    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    public class OverlayPanel : ContentControl
    {
        public static readonly DependencyProperty ShowCloseIconProperty = DependencyProperty.Register("ShowCloseIcon",
            typeof(bool), typeof(OverlayPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool ShowCloseIcon
        {
            get { return (bool)GetValue(ShowCloseIconProperty); }
            set { SetValue(ShowCloseIconProperty, value); }
        }

        public static readonly DependencyProperty AllowsTransparencyProperty = DependencyProperty.Register("AllowsTransparency",
            typeof(bool), typeof(OverlayPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool AllowsTransparency
        {
            get { return (bool)GetValue(AllowsTransparencyProperty); }
            set { SetValue(AllowsTransparencyProperty, value); }
        }

        public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.Register("PlacementTarget",
                       typeof(UIElement), typeof(OverlayPanel), new FrameworkPropertyMetadata((object)null));

        public UIElement PlacementTarget
        {
            get { return (UIElement)GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
        }

        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register("Placement",
                        typeof(PlacementMode), typeof(OverlayPanel), new FrameworkPropertyMetadata(PlacementMode.Bottom, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        public static readonly DependencyProperty StaysOpenProperty = DependencyProperty.Register("StaysOpen",
                       typeof(bool), typeof(OverlayPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool StaysOpen
        {
            get { return (bool)GetValue(StaysOpenProperty); }
            set { SetValue(StaysOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen",
           typeof(bool), typeof(OverlayPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        private Popup _popup;
        private Window _parentWindow;

        public double HorizontalOffset
        {
            get 
            {
                if (_popup != null)
                {
                    return _popup.HorizontalOffset;
                }
                return 0;   
            }
            set 
            {
                if (_popup != null)
                {
                    _popup.HorizontalOffset = value;
                }
            }
        }

        private RelayCommand _toggleCommand;
        public RelayCommand ToggleCommand =>
            _toggleCommand ??= new RelayCommand(() =>
            {
                if (_popup != null)
                {
                    _popup.IsOpen = !_popup.IsOpen;
                }
            });

        static OverlayPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OverlayPanel),
                                                   new FrameworkPropertyMetadata(
                                                       typeof(OverlayPanel)));
        }

        public OverlayPanel()
        {
            Loaded += OverlayPanel_Loaded;
        }

        private void OverlayPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _parentWindow = Window.GetWindow(this);
            if (_parentWindow != null)
            {
                _parentWindow.LocationChanged += _parentWindow_LocationChanged;
            }

            Loaded -= OverlayPanel_Loaded;
        }

        private void _parentWindow_LocationChanged(object sender, EventArgs e)
        {
            var offset = HorizontalOffset;
            HorizontalOffset = offset + 1;
            HorizontalOffset = offset;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var element = GetTemplateChild("PART_Popup") as Popup;

            if (element != null)
            {
                _popup = (Popup)element;
            }
        }

        
    }
}
