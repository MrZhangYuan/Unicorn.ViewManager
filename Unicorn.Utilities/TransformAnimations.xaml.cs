using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Unicorn.Utilities
{
    [Flags]
    public enum ControlAnimation : uint
    {
        None = 0x0,
        ScaleTransformXToOne = 0x1,
        ScaleTransformXToZero = 0x2,
        ScaleTransformXToValue = 0x4,

        ScaleTransformYToOne = 0x8,
        ScaleTransformYToZero = 0x10,
        ScaleTransformYToValue = 0x20,

        ScaleTransformToZero = ScaleTransformXToZero | ScaleTransformYToZero,
        ScaleTransformToOne = ScaleTransformXToOne | ScaleTransformYToOne,
        ScaleTransformToValue = ScaleTransformXToValue | ScaleTransformYToValue,

        TranslateTransformXToZero = 0x400,
        TranslateTransformXToValue = 0x800,

        TranslateTransformYToZero = 0x1000,
        TranslateTransformYToValue = 0x2000,

        TranslateTransformToZero = TranslateTransformXToZero | TranslateTransformYToZero,
        TranslateTransformToValue = TranslateTransformXToValue | TranslateTransformYToValue,

        ControlOpacityToOne = 0x10000,
        ControlOpacityToZero = 0x20000,

        ControlShaking = 0x40000,
        ControlFlicking = 0x80000
    }
    public class TransformValues
    {
        public double ScaleFromX
        {
            get; set;
        }
        public double ScaleFromY
        {
            get; set;
        }

        public double ScaleToX
        {
            get; set;
        }
        public double ScaleToY
        {
            get; set;
        }
        public double ScaleCenterX
        {
            get; set;
        }
        public double ScaleCenterY
        {
            get; set;
        }


        public double TranslateFromX
        {
            get; set;
        }
        public double TranslateFromY
        {
            get; set;
        }
        public double TranslateToX
        {
            get; set;
        }
        public double TranslateToY
        {
            get; set;
        }

        public TransformValues()
        {
            this.ScaleFromX = 1;
            this.ScaleFromY = 1;
        }
    }

    public class AnimationParameter
    {
        public ControlAnimation ControlAnimation
        {
            get; set;
        }

        public TransformValues Values
        {
            get; set;
        }

        public Action<object> StoryboardComplateCallBack
        {
            get; set;
        }
    }
    /// <summary>
    /// TransformAnimations.xaml 的交互逻辑
    /// </summary>
    public partial class TransformAnimations : ResourceDictionary
    {
        private static TransformAnimations _instance = null;
        public static TransformAnimations Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TransformAnimations();
                }
                return _instance;
            }
        }

        private TransformAnimations()
        {
            InitializeComponent();
        }

        private static bool IsFlagSet(ControlAnimation flag, ControlAnimation flags)
        {
            return (uint)(flags & flag) > 0U;
        }

        private DoubleAnimationUsingKeyFrames CreateKeyFrame(string propertypath, double value)
        {
            DoubleAnimationUsingKeyFrames keyframes = new DoubleAnimationUsingKeyFrames
            {
                BeginTime = TimeSpan.Zero
            };
            keyframes.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(propertypath));

            SplineDoubleKeyFrame spkeyframe = new SplineDoubleKeyFrame()
            {
                KeySpline = new KeySpline(0.25, 1, 0.05, 1),
                KeyTime = TimeSpan.FromMilliseconds(750),
                Value = value
            };

            keyframes.KeyFrames.Add(spkeyframe);

            return keyframes;
        }

        private DoubleAnimationUsingKeyFrames CreateOpacityKeyFrame(string propertypath, double value)
        {
            DoubleAnimationUsingKeyFrames keyframes = new DoubleAnimationUsingKeyFrames
            {
                BeginTime = TimeSpan.Zero
            };
            keyframes.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(propertypath));

            SplineDoubleKeyFrame spkeyframe = new SplineDoubleKeyFrame()
            {
                KeySpline = new KeySpline(0.25, 1, 0.05, 1),
                KeyTime = TimeSpan.FromMilliseconds(750),
                Value = value
            };

            keyframes.KeyFrames.Add(spkeyframe);

            return keyframes;
        }

        public Storyboard CreateStoryboard(ControlAnimation animation, TransformValues values, Action<object> callback)
        {
            Storyboard storyboard = new Storyboard();

            if (animation != ControlAnimation.None)
            {
                if (IsFlagSet(ControlAnimation.ControlShaking, animation))
                {
                    storyboard = (this["ControlShaking"] as Storyboard).Clone();
                }
                else if (IsFlagSet(ControlAnimation.ControlFlicking, animation))
                {
                    storyboard = (this["ControlFlicking"] as Storyboard).Clone();
                }
                else
                {
                    if (IsFlagSet(ControlAnimation.ScaleTransformXToOne, animation))
                    {
                        storyboard.Children.Add(
                            this.CreateKeyFrame(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)",
                                1
                            ));
                    }
                    if (IsFlagSet(ControlAnimation.ScaleTransformXToZero, animation))
                    {
                        storyboard.Children.Add(
                            this.CreateKeyFrame(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)",
                                0
                            ));
                    }
                    if (IsFlagSet(ControlAnimation.ScaleTransformXToValue, animation))
                    {
                        storyboard.Children.Add(
                            this.CreateKeyFrame(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)",
                                values.ScaleToX
                            ));
                    }

                    if (IsFlagSet(ControlAnimation.ScaleTransformYToOne, animation))
                    {
                        storyboard.Children.Add(
                            this.CreateKeyFrame(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)",
                                1
                            ));
                    }
                    if (IsFlagSet(ControlAnimation.ScaleTransformYToZero, animation))
                    {
                        storyboard.Children.Add(
                            this.CreateKeyFrame(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)",
                                0
                            ));
                    }
                    if (IsFlagSet(ControlAnimation.ScaleTransformYToValue, animation))
                    {
                        storyboard.Children.Add(
                            this.CreateKeyFrame(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)",
                                values.ScaleToY
                            ));
                    }


                    if (IsFlagSet(ControlAnimation.TranslateTransformXToZero, animation))
                    {
                        storyboard.Children.Add(
                            this.CreateKeyFrame(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)",
                                0
                            ));
                    }
                    if (IsFlagSet(ControlAnimation.TranslateTransformXToValue, animation))
                    {
                        storyboard.Children.Add(
                            this.CreateKeyFrame(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)",
                                values.TranslateToX
                            ));
                    }

                    if (IsFlagSet(ControlAnimation.TranslateTransformYToZero, animation))
                    {
                        storyboard.Children.Add(
                            this.CreateKeyFrame(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)",
                                0
                            ));
                    }
                    if (IsFlagSet(ControlAnimation.TranslateTransformYToValue, animation))
                    {
                        storyboard.Children.Add(
                            this.CreateKeyFrame(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)",
                                values.TranslateToY
                            ));
                    }


                    if (IsFlagSet(ControlAnimation.ControlOpacityToZero, animation))
                    {
                        storyboard.Children.Add(
                            this.CreateOpacityKeyFrame(
                                "(UIElement.Opacity)",
                                0
                            ));
                    }
                    if (IsFlagSet(ControlAnimation.ControlOpacityToOne, animation))
                    {
                        storyboard.Children.Add(
                            this.CreateOpacityKeyFrame(
                                "(UIElement.Opacity)",
                                1
                            ));
                    }
                }
            }
            storyboard.Completed += (sender, e) =>
            {
                callback?.Invoke(storyboard);
            };
            storyboard.Freeze();
            return storyboard;
        }
    }
}
