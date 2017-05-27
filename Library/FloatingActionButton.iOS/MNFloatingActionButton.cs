﻿using System;
using System.Linq;
using UIKit;
using CoreGraphics;
using Foundation;
using CoreAnimation;

namespace FAB.iOS
{
    public class MNFloatingActionButton : UIControl
    {
        /// <summary>
        /// Flags for hiding/showing shadow
        /// </summary>
        public enum ShadowState
        {
            ShadowStateShown,
            ShadowStateHidden
        }

        /// <summary>
        /// FAB Size options.
        /// </summary>
        public enum FABSize
        {
            Mini,
            Normal
        }

        private readonly nfloat animationDuration;
        private readonly nfloat animationScale;
        private readonly nfloat shadowOpacity;
        private readonly nfloat shadowRadius;

        private FABSize size = FABSize.Normal;

        /// <summary>
        /// Size to render the FAB -- Normal or <ini
        /// </summary>
        /// <value>The size.</value>
        public FABSize Size
        {
            get { return size; }
            set
            {
                if (size == value)
                    return;

                size = value;
                this.UpdateBackground();
            }
        }

        UIImageView _centerImageView;

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

        UIColor _backgroundColor;

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

                this.UpdateBackground();
            }
        }

        UIColor _shadowColor;

        public UIColor ShadowColor
        {
            get { return _shadowColor; }
            set
            {
                _shadowColor = value;
                this.UpdateBackground();
            }
        }

        bool _hasShadow;

        public bool HasShadow
        {
            get { return _hasShadow; }
            set
            {
                _hasShadow = value;
                this.UpdateBackground();
            }
        }

        public nfloat ShadowOpacity { get; private set; }

        public nfloat ShadowRadius { get; private set; }

        public nfloat AnimationScale { get; private set; }

        public nfloat AnimationDuration { get; private set; }

        public bool IsAnimating { get; private set; }

        public UIView BackgroundCircle { get; private set; }

        public bool AnimateOnSelection { get; set; }

        public MNFloatingActionButton(bool animateOnSelection)
            : base()
        {
            this.animationDuration = 0.05f;
            this.animationScale = 0.85f;
            this.shadowOpacity = 0.6f;
            this.shadowRadius = 1.5f;
            this.AnimateOnSelection = animateOnSelection;

            this.CommonInit();
        }

        public MNFloatingActionButton(CGRect frame, bool animateOnSelection)
            : base(frame)
        {
            this.animationDuration = 0.05f;
            this.animationScale = 0.85f;
            this.shadowOpacity = 0.6f;
            this.shadowRadius = 1.5f;
            this.AnimateOnSelection = animateOnSelection;

            this.CommonInit();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            this.CenterImageView.Center = this.BackgroundCircle.Center;
            if (!this.IsAnimating)
            {
                this.UpdateBackground();
            }
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            this.AnimateToSelectedState();
            this.SendActionForControlEvents(UIControlEvent.TouchDown);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            this.AnimateToDeselectedState();
            this.SendActionForControlEvents(UIControlEvent.TouchUpInside);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            this.AnimateToDeselectedState();
            this.SendActionForControlEvents(UIControlEvent.TouchCancel);
        }

        private void CommonInit()
        {
            this.BackgroundCircle = new UIView();

            this.BackgroundColor = UIColor.Red.ColorWithAlpha(0.4f);
            this.BackgroundColor = new UIColor(33.0f / 255.0f, 150.0f / 255.0f, 243.0f / 255.0f, 1.0f);
            this.BackgroundCircle.BackgroundColor = this.BackgroundColor;
            this.ShadowOpacity = shadowOpacity;
            this.ShadowRadius = shadowRadius;
            this.AnimationScale = animationScale;
            this.AnimationDuration = animationDuration;

            this.BackgroundCircle.AddSubview(this.CenterImageView);
            this.AddSubview(this.BackgroundCircle);
        }

        private void AnimateToSelectedState()
        {
            if (this.AnimateOnSelection)
            {
                this.IsAnimating = true;
                this.ToggleShadowAnimationToState(ShadowState.ShadowStateHidden);
                UIView.Animate(animationDuration, () =>
                    {
                        this.BackgroundCircle.Transform = CGAffineTransform.MakeScale(this.AnimationScale, this.AnimationScale);
                    }, () =>
                    {
                        this.IsAnimating = false;
                });
            }
        }

        private void AnimateToDeselectedState()
        {
            if (this.AnimateOnSelection)
            {
                this.IsAnimating = true;
                this.ToggleShadowAnimationToState(ShadowState.ShadowStateShown);
                UIView.Animate(animationDuration, () =>
                    {
                        this.BackgroundCircle.Transform = CGAffineTransform.MakeScale(1.0f, 1.0f);
                    }, () =>
                    {
                        this.IsAnimating = false;
                });
            }
        }

        private void ToggleShadowAnimationToState(ShadowState state)
        {
            nfloat endOpacity = 0.0f;
            if (state == ShadowState.ShadowStateShown)
            {
                endOpacity = this.ShadowOpacity;
            }

            CABasicAnimation animation = CABasicAnimation.FromKeyPath("shadowOpacity");
            animation.From = NSNumber.FromFloat((float)this.ShadowOpacity);
            animation.To = NSNumber.FromFloat((float)endOpacity);
            animation.Duration = animationDuration;
            this.BackgroundCircle.Layer.AddAnimation(animation, "shadowOpacity");
            this.BackgroundCircle.Layer.ShadowOpacity = (float)endOpacity;
        }

        private void UpdateBackground()
        {
            this.BackgroundCircle.Frame = this.Bounds;
            this.BackgroundCircle.Layer.CornerRadius = this.Bounds.Size.Height / 2;
            this.BackgroundCircle.Layer.ShadowColor = this.ShadowColor != null ? this.ShadowColor.CGColor : this.BackgroundColor.CGColor;
            this.BackgroundCircle.Layer.ShadowOpacity = (float)this.ShadowOpacity;
            this.BackgroundCircle.Layer.ShadowRadius = this.ShadowRadius;
            this.BackgroundCircle.Layer.ShadowOffset = new CGSize(1.0, 1.0);
            this.BackgroundCircle.BackgroundColor = this.BackgroundColor;

            var xPos = (this.BackgroundCircle.Bounds.Width / 2) - 12;
            var yPos = (this.BackgroundCircle.Bounds.Height / 2) - 12;

            this.CenterImageView.Frame = new CGRect(xPos, yPos, 24, 24);
        }
    }
}