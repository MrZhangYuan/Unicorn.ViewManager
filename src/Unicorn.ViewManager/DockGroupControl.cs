using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unicorn.ViewManager.Internal;
using System.Collections.ObjectModel;

namespace Unicorn.ViewManager
{
    public class DockGroupControl : SplitterItemsControl, IDockHost
    {
        public virtual IDockHost DockParent
        {
            get
            {
                return (IDockHost)GetValue(DockParentProperty);
            }
            private set
            {
                SetValue(DockParentProperty, value);
            }
        }
        public static readonly DependencyProperty DockParentProperty = DependencyProperty.Register("DockParent", typeof(IDockHost), typeof(DockGroupControl), new PropertyMetadata(null));


        public virtual DockHostCollection Children
        {
            get
            {
                return (DockHostCollection)GetValue(ChildrenProperty);
            }
            private set
            {
                SetValue(ChildrenProperty, value);
            }
        }
        public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register("Children", typeof(DockHostCollection), typeof(DockGroupControl), new PropertyMetadata(null));



        static DockGroupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockGroupControl), new FrameworkPropertyMetadata(typeof(DockGroupControl)));
        }

        private readonly ObservableCollection<IDockHost> _innerChildren = new ObservableCollection<IDockHost>();
        public DockGroupControl()
        {
            this.Children = new DockHostCollection(this._innerChildren);

            this.SetBinding(ItemsSourceProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(DockGroupControl.ChildrenProperty),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
        }

        public void Dock(DockDirection direction, IDockHost child)
        {
            switch (direction)
            {
                case DockDirection.Fill:
                    break;

                case DockDirection.Left:
                case DockDirection.Right:
                    this.SetValue(DockTarget.OrientationProperty, Orientation.Horizontal);
                    break;

                case DockDirection.Top:
                case DockDirection.Bottom:
                    this.SetValue(DockTarget.OrientationProperty, Orientation.Vertical);
                    break;
            }

            this._innerChildren.Add(child);
        }

        public void Dock(IDockHost child)
        {
            this._innerChildren.Add(child);
        }

        public virtual void Dock(DockDirection direction, IDockHost oldchild, IDockHost newchild)
        {
            if (!IsDirectionAllowed(direction))
            {
                throw new InvalidOperationException($"当前不可停靠在 {direction} 位置");
            }

            int index = this._innerChildren.IndexOf(oldchild);

            if (this._innerChildren.Count <= 1)
            {
                if (index >= 0)
                {
                    if (newchild.DockParent != null)
                    {
                        newchild.DockParent.UnDock(newchild);
                    }

                    index = direction == DockDirection.Right || direction == DockDirection.Bottom ? index + 1 : index;

                    switch (direction)
                    {
                        case DockDirection.Fill:
                            break;

                        case DockDirection.Left:
                        case DockDirection.Right:
                            this.SetValue(DockTarget.OrientationProperty, Orientation.Horizontal);
                            break;

                        case DockDirection.Top:
                        case DockDirection.Bottom:
                            this.SetValue(DockTarget.OrientationProperty, Orientation.Vertical);
                            break;
                    }

                    this._innerChildren.Insert(index, newchild);
                }
            }
        }

        public virtual void UnDock(IDockHost child)
        {
            if (this._innerChildren.Contains(child))
            {
                this._innerChildren.Remove(child);
            }
        }

        public virtual bool IsDirectionAllowed(DockDirection direction)
        {
            Orientation? orientation = (Orientation?)this.GetValue(DockTarget.OrientationProperty);
            switch (orientation)
            {
                case Orientation.Vertical:
                    return direction == DockDirection.Top
                        || direction == DockDirection.Bottom;

                case Orientation.Horizontal:
                    return direction == DockDirection.Left
                        || direction == DockDirection.Right;
            }

            return true;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return false;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DockItem();
        }
    }
}
