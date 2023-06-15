using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WpfCustomControlLibrary.Controls
{
    public enum TooglePosition
    {
        Left,
        Right
    }

    [TemplatePart(Name = PART_HeaderContent, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_BodyContent, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_ToogleButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = PART_CanvasPanel, Type = typeof(Canvas))]
    public class AccordionPanel : Control
    {
        #region PartsConst
        private const string PART_HeaderContent = "PART_HeaderContent";
        private const string PART_ToogleButton = "PART_ToogleButton";
        private const string PART_CanvasPanel = "PART_CanvasPanel";
        private const string PART_BodyContent = "PART_BodyContent";
        #endregion

        #region Parts
        private ContentPresenter _headerContent;
        private ContentPresenter _bodyContent;
        private ToggleButton _toggleButton;
        private Canvas _canvasPanel;
        #endregion

        #region Ctor
        public AccordionPanel()
        {
        }

        static AccordionPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AccordionPanel), new FrameworkPropertyMetadata(typeof(AccordionPanel)));
        }
        #endregion

        public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.Register("HeaderContent",
         typeof(object), typeof(AccordionPanel), new UIPropertyMetadata(null));
        public object HeaderContent
        {
            get { return (object)GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        //public static readonly DependencyProperty HeaderContentTemplateProperty = DependencyProperty.Register("HeaderContentTemplate",
        // typeof(DataTemplate), typeof(AccordionPanel), new UIPropertyMetadata(null));
        //public DataTemplate HeaderContentTemplate
        //{
        //    get { return (DataTemplate)GetValue(HeaderContentTemplateProperty); }
        //    set { SetValue(HeaderContentTemplateProperty, value); }
        //}

        public static readonly DependencyProperty BodyContentProperty = DependencyProperty.Register("BodyContent",
            typeof(object), typeof(AccordionPanel), new UIPropertyMetadata(null));
        public object BodyContent
        {
            get { return (object)GetValue(BodyContentProperty); }
            set { SetValue(BodyContentProperty, value);}
        }

        //public static readonly DependencyProperty BodyContentTemplateProperty = DependencyProperty.Register("BodyContentTemplate",
        //typeof(DataTemplate), typeof(AccordionPanel), new UIPropertyMetadata(null));
        //public DataTemplate BodyContentTemplate
        //{
        //    get { return (DataTemplate)GetValue(BodyContentTemplateProperty); }
        //    set { SetValue(BodyContentTemplateProperty, value); }
        //}

        public static readonly DependencyProperty BackgroundBodyProperty = DependencyProperty.Register("BackgroundBody", typeof(Brush), typeof(AccordionPanel),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray), null));
        public Brush BackgroundBody
        {
            get { return (Brush)GetValue(BackgroundBodyProperty); }
            set { SetValue(BackgroundBodyProperty, value); }
        }

        public static readonly DependencyProperty BorderBrushBodyProperty = DependencyProperty.Register("BorderBrushBody", typeof(Brush), typeof(AccordionPanel),
           new PropertyMetadata(new SolidColorBrush(Colors.Gray), null));
        public Brush BorderBrushBody
        {
            get { return (Brush)GetValue(BorderBrushBodyProperty); }
            set { SetValue(BorderBrushBodyProperty, value); }
        }

        public static readonly DependencyProperty BorderThicknessBodyProperty = DependencyProperty.Register("BorderThicknessBody", typeof(Thickness), typeof(AccordionPanel),
          new PropertyMetadata(new Thickness(0, 0, 0, 0), null));
        public Thickness BorderThicknessBody
        {
            get { return (Thickness)GetValue(BorderThicknessBodyProperty); }
            set { SetValue(BorderThicknessBodyProperty, value); }
        }

        public static readonly DependencyProperty ToogleButtonPositionProperty = DependencyProperty.Register("ToogleButtonPosition", typeof(TooglePosition), typeof(AccordionPanel),
         new PropertyMetadata(TooglePosition.Right, null));
        public TooglePosition ToogleButtonPosition
        {
            get { return (TooglePosition)GetValue(ToogleButtonPositionProperty); }
            set { SetValue(ToogleButtonPositionProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AttachHeadeContent();
            AttachBodyContent();
            AttachToggleButton();
            AttachCanvasPanel();
        }

        private void AttachHeadeContent()
        {
            var headerContent = GetTemplateChild(PART_HeaderContent) as ContentPresenter;

            if (headerContent != null)
            {
                _headerContent = headerContent;
                //var child = _headerContent.GetChildrens().FirstOrDefault();
                //child.DataContext = this.DataContext;
            }
        }

        private void AttachBodyContent()
        {
            var bodyContent = GetTemplateChild(PART_BodyContent) as ContentPresenter;

            if (bodyContent != null)
            {
                _bodyContent = bodyContent;
                _bodyContent.DataContext = this.DataContext;
            }
        }

        private void AttachToggleButton()
        {
            var toggleButton = GetTemplateChild(PART_ToogleButton) as ToggleButton;

            if (toggleButton != null)
            {
                _toggleButton = toggleButton;
            }
        }

        private void AttachCanvasPanel()
        {
            var canvasPanel = GetTemplateChild(PART_CanvasPanel) as Canvas;

            if (canvasPanel != null)
            {
                _canvasPanel = canvasPanel;
            }
        }
    }
}
