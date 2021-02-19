using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Unicorn.ViewManager
{
    public class ToolTabGroupControl : TabGroupControl
    {
        private static readonly DependencyProperty SelectedTitleProperty = DependencyProperty.Register("SelectedTitle", typeof(object), typeof(ToolTabGroupControl), new PropertyMetadata(null));

        public bool IsHeaderVisible
        {
            get
            {
                return (bool)GetValue(IsHeaderVisibleProperty);
            }
            private set
            {
                SetValue(IsHeaderVisibleProperty, value);
            }
        }
        public static readonly DependencyProperty IsHeaderVisibleProperty = DependencyProperty.Register("IsHeaderVisible", typeof(bool), typeof(ToolTabGroupControl), new PropertyMetadata(true));

        static ToolTabGroupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolTabGroupControl), new FrameworkPropertyMetadata(typeof(ToolTabGroupControl)));
        }

        private ContentPresenter _titleContainer = null;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._titleContainer = this.GetTemplateChild("PART_TitleContainer") as ContentPresenter;
            if (this._titleContainer != null)
            {
                this._titleContainer.SetBinding(
                    ContentPresenter.ContentProperty,
                    new Binding
                    {
                        Source = this,
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(SelectedTitleProperty)
                    });
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            this.IsHeaderVisible = this.Items.Count >= 2;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            this.SetValue(SelectedTitleProperty, (this.SelectedItem as TabGroupTabItem)?._titleDockDragGrip);
        }

        public override TabGroupControl CreateTabGroup()
        {
            return new ToolTabGroupControl();
        }
    }
}


