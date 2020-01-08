using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Unicorn.Utilities
{
    public static class ControlAnimationHelper
    {
        public static bool BeginTransformAnimation(this FrameworkElement targetcontrol, AnimationParameter parameter)
        {
            if (targetcontrol != null
                && parameter != null)
            {
                if (targetcontrol.RenderTransform == null
                    || targetcontrol.RenderTransform == Transform.Identity)
                {
                    targetcontrol.RenderTransform = new TransformGroup
                    {
                        Children = new TransformCollection
                        {
                            new ScaleTransform()
                            {
                                CenterX = parameter.Values.ScaleCenterX,
                                CenterY = parameter.Values.ScaleCenterY,
                                ScaleX = parameter.Values.ScaleFromX,
                                ScaleY = parameter.Values.ScaleFromY
                            },
                            new TranslateTransform()
                            {
                                X = parameter.Values.TranslateFromX,
                                Y = parameter.Values.TranslateFromY
                            },
                            new RotateTransform(),
                            new SkewTransform(),
                        }
                    };
                }

                Storyboard storyboard = TransformAnimations.Instance.CreateStoryboard(
                        parameter.ControlAnimation,
                        parameter.Values,
                        parameter.StoryboardComplateCallBack
                    );

                if (storyboard != null)
                {
                    targetcontrol.BeginStoryboard(storyboard);
                    return true;
                }
            }

            return false;
        }
    }

}
