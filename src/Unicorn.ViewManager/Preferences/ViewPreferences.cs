using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Unicorn.ViewManager.Preferences
{
    public sealed class ViewPreferences : DependencyObject
    {
        private static ViewPreferences _instance = null;
        public static ViewPreferences Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewPreferences();
                }
                return _instance;
            }
        }

        private ViewPreferences()
        {

        }

        public bool UsePopupViewAnimations
        {
            get
            {
                return (bool)GetValue(UsePopupViewAnimationsProperty);
            }
            set
            {
                SetValue(UsePopupViewAnimationsProperty, value);
            }
        }
        public static readonly DependencyProperty UsePopupViewAnimationsProperty = DependencyProperty.Register("UsePopupViewAnimations", typeof(bool), typeof(ViewPreferences), new PropertyMetadata(true));


    }
}
