using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Unicorn.ViewManager
{
    public class TabGroupTabItem : TabItem
    {
        [Category("Content")]
        public object Title
        {
            get
            {
                return (object)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(TabGroupTabItem), new PropertyMetadata(null));

        public DataTemplate TitleTemplate
        {
            get
            {
                return (DataTemplate)GetValue(TitleTemplateProperty);
            }
            set
            {
                SetValue(TitleTemplateProperty, value);
            }
        }
        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(TabGroupTabItem), new PropertyMetadata(null));


        public bool IsActive
        {
            get
            {
                return (bool)GetValue(IsActiveProperty);
            }
            set
            {
                SetValue(IsActiveProperty, value);
            }
        }
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(TabGroupTabItem), new PropertyMetadata(false, IsActivePropertyChangedCallback));

        private static void IsActivePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        static TabGroupTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabGroupTabItem), new FrameworkPropertyMetadata(typeof(TabGroupTabItem)));
        }

        public TabGroupControl ParentHost
        {
            get;
            internal set;
        }

        internal readonly DockDragGrip _titleDockDragGrip = new DockDragGrip();
        public TabGroupTabItem()
        {
            this._titleDockDragGrip.Element = this;
            this._titleDockDragGrip.SetBinding(
                ContentControl.ContentProperty,
                new Binding
                {
                    Source = this,
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Path = new PropertyPath(TabGroupTabItem.TitleProperty)
                });

            this._titleDockDragGrip.SetBinding(
                ContentControl.ContentTemplateProperty,
                new Binding
                {
                    Source = this,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Path = new PropertyPath(TabGroupTabItem.TitleTemplateProperty)
                });

            ViewManager.Instance.RegisterTabView(this);
        }

        public void UnDock()
        {
            if (this.ParentHost != null)
            {
                this.ParentHost.UnDock(this);
            }
        }

        public void Active()
        {

        }

        public void ShowFloating()
        {

        }
    }
}


