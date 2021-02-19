using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Unicorn.ViewManager
{
    [DefaultProperty("DockGroup")]
    [ContentProperty("DockGroup")]
    public class DockRootControl : Control
    {
        static DockRootControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockRootControl), new FrameworkPropertyMetadata(typeof(DockRootControl)));
        }

        public DockGroupControl DockGroup
        {
            get
            {
                return (DockGroupControl)GetValue(DockGroupProperty);
            }
            set
            {
                SetValue(DockGroupProperty, value);
            }
        }
        public static readonly DependencyProperty DockGroupProperty = DependencyProperty.Register("DockGroup", typeof(DockGroupControl), typeof(DockRootControl), new PropertyMetadata(null));


        private void EnsureInitDockGroup()
        {
            if (this.DockGroup == null)
            {
                this.DockGroup = new DockGroupControl();
            }
        }


        public void Dock(DockDirection direction, TabGroupTabItem draggedtab)
        {
            this.EnsureInitDockGroup();

            switch (direction)
            {
                case DockDirection.Fill:
                    if (this.DockGroup.Items.Count == 0)
                    {
                        TabGroupControl tabgroup = new ViewTabGroupControl();
                        tabgroup.Dock(draggedtab);
                        this.DockGroup.Dock(DockDirection.Fill, tabgroup);
                    }
                    else if (this.DockGroup.Items.Count == 1
                        && this.DockGroup.Items[0] is TabGroupControl tabgroup)
                    {
                        tabgroup.Dock(draggedtab);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                    break;

                case DockDirection.Left:
                case DockDirection.Right:
                case DockDirection.Top:
                case DockDirection.Bottom:
                    {
                        var orientation = this.DockGroup.Items.Count <= 1 ? null : (Orientation?)this.DockGroup.Orientation;

                        switch (direction)
                        {
                            case DockDirection.Left:
                                draggedtab.SetValue(AutoHideChannelControl.ChannelDockProperty, System.Windows.Controls.Dock.Left);
                                break;
                            case DockDirection.Top:
                                draggedtab.SetValue(AutoHideChannelControl.ChannelDockProperty, System.Windows.Controls.Dock.Top);
                                break;
                            case DockDirection.Right:
                                draggedtab.SetValue(AutoHideChannelControl.ChannelDockProperty, System.Windows.Controls.Dock.Right);
                                break;
                            case DockDirection.Bottom:
                                draggedtab.SetValue(AutoHideChannelControl.ChannelDockProperty, System.Windows.Controls.Dock.Bottom);
                                break;
                        }

                        switch (orientation)
                        {
                            case Orientation.Horizontal:
                                H:
                                {
                                    switch (direction)
                                    {
                                        //如果停靠方向和 DockGroupControl 方向一致，不产生新的 DockGroupControl
                                        case DockDirection.Left:
                                        case DockDirection.Right:
                                            {
                                                ToolTabGroupControl newtab = new ToolTabGroupControl();
                                                newtab.Dock(draggedtab);
                                                if (direction == DockDirection.Left)
                                                {
                                                    this.DockGroup.Items.Insert(0, newtab);
                                                }
                                                else
                                                {
                                                    this.DockGroup.Items.Add(newtab);
                                                }
                                            }
                                            break;

                                        //如果停靠方向和 DockGroupControl 方向不一致，产生新的 DockGroupControl
                                        case DockDirection.Top:
                                        case DockDirection.Bottom:
                                            {
                                                var oldgroup = this.DockGroup;
                                                ToolTabGroupControl newtab = new ToolTabGroupControl();
                                                newtab.Dock(draggedtab);
                                                DockGroupControl newgroup = new DockGroupControl()
                                                {
                                                    Orientation = Orientation.Vertical
                                                };

                                                if (direction == DockDirection.Top)
                                                {
                                                    newgroup.Items.Add(newtab);
                                                    newgroup.Items.Add(oldgroup);
                                                }
                                                else
                                                {
                                                    newgroup.Items.Add(oldgroup);
                                                    newgroup.Items.Add(newtab);
                                                }

                                                this.DockGroup = newgroup;
                                            }
                                            break;
                                    }
                                }
                                break;

                            case Orientation.Vertical:
                                V:
                                {
                                    switch (direction)
                                    {
                                        //如果停靠方向和 DockGroupControl 方向不一致，产生新的 DockGroupControl
                                        case DockDirection.Left:
                                        case DockDirection.Right:
                                            {
                                                var oldgroup = this.DockGroup;
                                                ToolTabGroupControl newtab = new ToolTabGroupControl();
                                                newtab.Dock(draggedtab);
                                                DockGroupControl newgroup = new DockGroupControl()
                                                {
                                                    Orientation = Orientation.Horizontal
                                                };

                                                if (direction == DockDirection.Left)
                                                {
                                                    newgroup.Items.Add(newtab);
                                                    newgroup.Items.Add(oldgroup);
                                                }
                                                else
                                                {
                                                    newgroup.Items.Add(oldgroup);
                                                    newgroup.Items.Add(newtab);
                                                }

                                                this.DockGroup = newgroup;
                                            }
                                            break;

                                        //如果停靠方向和 DockGroupControl 方向一致，不产生新的 DockGroupControl
                                        case DockDirection.Top:
                                        case DockDirection.Bottom:
                                            {
                                                ToolTabGroupControl newtab = new ToolTabGroupControl();
                                                newtab.Dock(draggedtab);

                                                if (direction == DockDirection.Top)
                                                {
                                                    this.DockGroup.Items.Insert(0, newtab);
                                                }
                                                else
                                                {
                                                    this.DockGroup.Items.Add(newtab);
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;

                            default:
                                {
                                    //若 DockGroupControl 未定义方向，则定义方向
                                    switch (direction)
                                    {
                                        case DockDirection.Left:
                                        case DockDirection.Right:
                                            this.DockGroup.Orientation = Orientation.Horizontal;
                                            goto H;

                                        case DockDirection.Top:
                                        case DockDirection.Bottom:
                                            this.DockGroup.Orientation = Orientation.Vertical;
                                            goto V;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
