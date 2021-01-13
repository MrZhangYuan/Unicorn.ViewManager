using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Unicorn.ViewManager
{
    internal static class ViewTreeHelper
    {
        public static void HitTestVisibleElements(
             Visual visual,
             HitTestResultCallback resultCallback,
             HitTestParameters parameters)
        {
            VisualTreeHelper.HitTest(visual, new HitTestFilterCallback(ExcludeNonVisualElements), resultCallback, parameters);
        }

        private static HitTestFilterBehavior ExcludeNonVisualElements(DependencyObject potentialHitTestTarget)
        {
            return !(potentialHitTestTarget is Visual)
                || potentialHitTestTarget is UIElement uiElement
                && (!uiElement.IsVisible || !uiElement.IsEnabled)
                ?
                HitTestFilterBehavior.ContinueSkipSelfAndChildren : HitTestFilterBehavior.Continue;
        }

        public static TAncestorType FindAncestorOrSelf<TAncestorType>(this Visual obj) where TAncestorType : DependencyObject
        {
            return obj.FindAncestorOrSelf<TAncestorType, DependencyObject>(GetVisualOrLogicalParent);
        }

        public static TAncestorType FindAncestorOrSelf<TAncestorType, TElementType>(this TElementType obj, Func<TElementType, TElementType> parentEvaluator) where TAncestorType : DependencyObject
        {
            return (obj as TAncestorType) ?? obj.FindAncestor<TAncestorType, TElementType>(parentEvaluator);
        }

        public static TAncestorType FindAncestor<TAncestorType>(this Visual obj) where TAncestorType : DependencyObject
        {
            return obj.FindAncestor<TAncestorType, DependencyObject>(GetVisualOrLogicalParent);
        }



        public static TAncestorType FindAncestor<TAncestorType, TElementType>(this TElementType obj, Func<TElementType, TElementType> parentEvaluator) where TAncestorType : class
        {
            return obj.FindAncestor(parentEvaluator, (TElementType ancestor) => ((object)ancestor) is TAncestorType) as TAncestorType;
        }

        public static object FindAncestor<TElementType>(this TElementType obj, Func<TElementType, TElementType> parentEvaluator, Func<TElementType, bool> ancestorSelector)
        {
            for (TElementType val = parentEvaluator(obj); val != null; val = parentEvaluator(val))
            {
                if (ancestorSelector(val))
                {
                    return val;
                }
            }
            return null;
        }


        public static TAncestorType FindAncestor<TAncestorType>(this Visual obj, Func<TAncestorType, bool> ancestorSelector) where TAncestorType : DependencyObject
        {
            for (DependencyObject val = GetVisualOrLogicalParent(obj); val != null; val = GetVisualOrLogicalParent(val))
            {
                if (val is TAncestorType ancestor
                    && ancestorSelector(ancestor))
                {
                    return ancestor;
                }
            }
            return null;
        }

        public static IEnumerable<TAncestorType> FindAncestorAll<TAncestorType>(this Visual obj, Func<TAncestorType, bool> ancestorSelector) where TAncestorType : DependencyObject
        {
            for (DependencyObject val = GetVisualOrLogicalParent(obj); val != null; val = GetVisualOrLogicalParent(val))
            {
                if (val is TAncestorType ancestor
                    && ancestorSelector(ancestor))
                {
                    yield return ancestor;
                }
            }
        }


        public static DependencyObject GetVisualOrLogicalParent(this DependencyObject sourceElement)
        {
            if (sourceElement == null)
            {
                return null;
            }
            if (sourceElement is Visual)
            {
                return VisualTreeHelper.GetParent(sourceElement) ?? LogicalTreeHelper.GetParent(sourceElement);
            }
            return LogicalTreeHelper.GetParent(sourceElement);
        }

        public static bool IsConnectedToPresentationSource(this DependencyObject obj)
        {
            return PresentationSource.FromDependencyObject(obj) != null;
        }


        private static T FindParent<T>(this FrameworkElement element) where T : class
        {
            if (element == null)
            {
                return null;
            }

            FrameworkElement parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
            if (parent == null)
            {
                parent = LogicalTreeHelper.GetParent(element) as FrameworkElement;
            }

            while (parent != null
                && !(parent is T)
                && !(parent is Window))
            {
                var temp = VisualTreeHelper.GetParent(parent) as FrameworkElement;

                if (temp == null)
                {
                    temp = LogicalTreeHelper.GetParent(parent) as FrameworkElement;
                }

                parent = temp;
            }

            return parent as T;
        }
    }
}
