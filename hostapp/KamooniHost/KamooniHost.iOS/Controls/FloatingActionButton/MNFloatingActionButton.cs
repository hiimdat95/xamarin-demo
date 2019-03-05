using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using UIKit;
using Xamarin.Forms.Controls;

namespace KamooniHost.iOS.Controls.FloatingActionButton
{
    public class MNFloatingActionButton : UIControl
    {
        private const int fabHeightNormal = 56;
        private const int fabHeightMini = 40;

        /// <summary>
        /// Flags for hiding/showing shadow
        /// </summary>
        public enum ShadowState
        {
            Shown,
            Hidden
        }

        private readonly nfloat animationDuration;
        private readonly nfloat animationScale;
        private readonly nfloat shadowOpacity;
        private readonly nfloat shadowRadius;

        private FabSize size = FabSize.Normal;

        /// <summary>
        /// Size to render the FAB -- Normal or Mini
        /// </summary>
        /// <value>The size.</value>
        public FabSize Size
        {
            get { return size; }
            set
            {
                if (size == value)
                    return;

                size = value;
                UpdateBackground();
            }
        }

        private UIImageView _centerImageView;

        /// <summary>
        /// The image to display int the center of the button
        /// </summary>
        /// <value>The center image view.</value>
        public UIImageView CenterImageView
        {
            get
            {
                if (_centerImageView == null)
                {
                    _centerImageView = new UIImageView();
                }

                return _centerImageView;
            }
            private set
            {
                _centerImageView = value;
            }
        }

        private UIColor _backgroundColor;

        /// <summary>
        /// Background Color of the FAB
        /// </summary>
        /// <value>The color of the background.</value>
        public new UIColor BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;

                UpdateBackground();
            }
        }

        private UIColor _pressedColor;

        /// <summary>
        /// Background Color of the FAB
        /// </summary>
        /// <value>The color of the background.</value>
        public UIColor PressedColor
        {
            get
            {
                return _pressedColor;
            }
            set
            {
                _pressedColor = value;

                UpdateBackground();
            }
        }

        private UIColor _shadowColor;

        public UIColor ShadowColor
        {
            get { return _shadowColor; }
            set
            {
                _shadowColor = value;
                UpdateBackground();
            }
        }

        private bool _hasShadow;

        public bool HasShadow
        {
            get { return _hasShadow; }
            set
            {
                _hasShadow = value;
                UpdateBackground();
            }
        }

        public nfloat AnimationDuration { get; private set; }

        public nfloat AnimationScale { get; private set; }

        public nfloat ShadowOpacity { get; private set; }

        public nfloat ShadowRadius { get; private set; }

        public bool IsAnimating { get; private set; }

        public UIView BackgroundCircle { get; private set; }

        public bool AnimateOnSelection { get; set; }

        public double FabHeight { get; set; }

        public double CenterImageHeight { get; set; }

        public double CenterImageMargin { get; set; }

        public MNFloatingActionButton(bool animateOnSelection)
            : base()
        {
            animationDuration = 0.05f;
            animationScale = 0.85f;
            shadowOpacity = 0.6f;
            shadowRadius = 1.5f;
            AnimateOnSelection = animateOnSelection;

            CommonInit();
        }

        public MNFloatingActionButton(CGRect frame, bool animateOnSelection)
            : base(frame)
        {
            animationDuration = 0.05f;
            animationScale = 0.85f;
            shadowOpacity = 0.6f;
            shadowRadius = 1.5f;
            AnimateOnSelection = animateOnSelection;

            CommonInit();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CenterImageView.Center = BackgroundCircle.Center;
            if (!IsAnimating)
            {
                UpdateBackground();
            }
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            AnimateToSelectedState();
            SendActionForControlEvents(UIControlEvent.TouchDown);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            AnimateToDeselectedState();
            SendActionForControlEvents(UIControlEvent.TouchUpInside);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            AnimateToDeselectedState();
            SendActionForControlEvents(UIControlEvent.TouchCancel);
        }

        private void CommonInit()
        {
            FabHeight = Size == FabSize.Normal ? fabHeightNormal : fabHeightMini;
            CenterImageHeight = FabHeight / 1.75;
            CenterImageMargin = (FabHeight - CenterImageHeight) / 2;

            BackgroundCircle = new UIView();
            BackgroundColor = UIColor.Red.ColorWithAlpha(0.4f);
            BackgroundColor = new UIColor(33.0f / 255.0f, 150.0f / 255.0f, 243.0f / 255.0f, 1.0f);
            BackgroundCircle.BackgroundColor = BackgroundColor;
            BackgroundCircle.AddSubview(CenterImageView);

            ShadowOpacity = shadowOpacity;
            ShadowRadius = shadowRadius;
            AnimationScale = animationScale;
            AnimationDuration = animationDuration;

            AddSubview(BackgroundCircle);
        }

        private void AnimateToSelectedState()
        {
            if (AnimateOnSelection)
            {
                IsAnimating = true;
                ToggleShadowAnimationToState(ShadowState.Hidden);
                Animate(animationDuration, () =>
                {
                    BackgroundCircle.Transform = CGAffineTransform.MakeScale(AnimationScale, AnimationScale);
                }, () =>
                {
                    IsAnimating = false;
                    CenterImageView.Frame = new CGRect(CenterImageMargin, CenterImageMargin, CenterImageHeight, CenterImageHeight);
                });
            }
        }

        private void AnimateToDeselectedState()
        {
            if (AnimateOnSelection)
            {
                IsAnimating = true;
                ToggleShadowAnimationToState(ShadowState.Shown);
                Animate(animationDuration, () =>
                {
                    BackgroundCircle.Transform = CGAffineTransform.MakeScale(1.0f, 1.0f);
                }, () =>
                {
                    IsAnimating = false;
                    UpdateBackground();
                });
            }
        }

        private void ToggleShadowAnimationToState(ShadowState state)
        {
            nfloat endOpacity = 0.0f;
            if (state == ShadowState.Shown)
            {
                endOpacity = ShadowOpacity;
            }

            CABasicAnimation animation = CABasicAnimation.FromKeyPath("shadowOpacity");
            animation.From = NSNumber.FromFloat((float)ShadowOpacity);
            animation.To = NSNumber.FromFloat((float)endOpacity);
            animation.Duration = animationDuration;
            BackgroundCircle.Layer.AddAnimation(animation, "shadowOpacity");
            BackgroundCircle.Layer.ShadowOpacity = (float)endOpacity;
        }

        private void UpdateBackground()
        {
            BackgroundCircle.Frame = new CGRect(0, 0, FabHeight, FabHeight);
            BackgroundCircle.Layer.CornerRadius = BackgroundCircle.Frame.Height / 2;
            BackgroundCircle.Layer.ShadowColor = ShadowColor != null ? ShadowColor.CGColor : BackgroundColor.CGColor;
            BackgroundCircle.Layer.ShadowOpacity = (float)ShadowOpacity;
            BackgroundCircle.Layer.ShadowRadius = ShadowRadius;
            BackgroundCircle.Layer.ShadowOffset = new CGSize(1.0, 1.0);
            BackgroundCircle.BackgroundColor = BackgroundColor;

            CenterImageView.Frame = new CGRect(CenterImageMargin, CenterImageMargin, CenterImageHeight, CenterImageHeight);
        }
    }
}