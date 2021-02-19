using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unicorn.ViewManager;

namespace ViewManagerDemo.Views
{
    /// <summary>
    /// DockViewDemo.xaml 的交互逻辑
    /// </summary>
    public partial class DockViewDemo
    {

        private static DockViewDemo _instance = null;
        public static DockViewDemo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DockViewDemo();
                }
                return _instance;
            }
        }

        public DockViewDemo()
        {
            InitializeComponent();

            this.Shown += DockViewDemo_Shown;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.A && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                this.AddTabView();
            }
        }

        private void AddTabView()
        {

        }

        private void DockViewDemo_Shown(object sender, EventArgs e)
        {
            //for (int i = 0; i < 7; i++)
            //{
            //    this._DockGroup.Dock(new DockItem()
            //    {
            //        Content = new Button
            //        {
            //            Width = 50,
            //            Height = 50,
            //            Content = i
            //        },
            //        Tag = $"{i}"
            //    });
            //}
        }
    }
}
