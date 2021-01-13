using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unicorn.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Unicorn.ViewManager
{
    class DockView : DependencyObject
    {

    }

    class ViewGroup : DockView
    {

    }

    class DockRoot : DockView
    {

    }



    public class DockHostCollection : ReadOnlyObservableCollection<IDockHost>
    {
        public DockHostCollection(ObservableCollection<IDockHost> children) :
            base(children)
        {

        }
    }

    public interface IDockHost
    {
        IDockHost DockParent { get; }
        DockHostCollection Children { get; }
        void Dock(DockDirection direction, IDockHost oldchild, IDockHost newchild);
        void Dock(DockDirection direction, IDockHost oldchild);
        void Dock(IDockHost oldchild);
        void UnDock(IDockHost child);
        bool IsDirectionAllowed(DockDirection direction);
    }

    public class DockItem : SplitterItem, IDockHost
    {
        private DockGroupControl _dockGroupControl = null;
        internal DockGroupControl DockGroupControl
        {
            get => this._dockGroupControl ?? (this._dockGroupControl = new DockGroupControl());
        }
        public IDockHost DockParent
        {
            get => this.DockGroupControl.DockParent;
        }
        public DockHostCollection Children
        {
            get => this.DockGroupControl.Children;
        }

        static DockItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockItem), new FrameworkPropertyMetadata(typeof(DockItem)));
        }

        public DockItem()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var container = this.GetTemplateChild("PART_CONTAINER") as ContentPresenter;
            if (container != null)
            {
                container.Content = this.DockGroupControl;
            }
        }

        public void Dock(DockDirection direction, IDockHost oldchild, IDockHost newchild) => this.DockGroupControl.Dock(direction, oldchild, newchild);
        public void Dock(DockDirection direction, IDockHost child) => this.DockGroupControl.Dock(direction, child);
        public void Dock(IDockHost child) => this.DockGroupControl.Dock(child);
        public void UnDock(IDockHost child) => this.DockGroupControl.UnDock(child);
        public bool IsDirectionAllowed(DockDirection direction) => this.DockGroupControl.IsDirectionAllowed(direction);
    }
}
