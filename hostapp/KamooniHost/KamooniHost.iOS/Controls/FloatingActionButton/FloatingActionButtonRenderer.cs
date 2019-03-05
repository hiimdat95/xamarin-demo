using CoreGraphics;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FloatingActionButton), typeof(KamooniHost.iOS.Controls.FloatingActionButton.FloatingActionButtonRenderer))]

namespace KamooniHost.iOS.Controls.FloatingActionButton
{
    public partial class FloatingActionButtonRenderer : ViewRenderer<Xamarin.Forms.Controls.FloatingActionButton, MNFloatingActionButton>
    {
        private const int margin = 16;
        private const int fabHeightNormal = 56;
        private const int fabHeightMini = 40;

        private int fabSize;
        private double imageSize;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Controls.FloatingActionButton> e)
        {
            base.OnElementChanged(e);

            if (Control == null && Element != null)
            {
                Element.Margin = new Thickness(margin, margin, margin, margin);

                fabSize = Element.Size == FabSize.Normal ? fabHeightNormal : fabHeightMini;
                imageSize = fabSize / 1.75;

                SetNativeControl(new MNFloatingActionButton(Element.AnimateOnSelection)
                {
                    Frame = new CGRect(0, 0, fabSize, fabSize)
                });

                UpdateStyles();
            }

            if (e.NewElement != null)
            {
                Control.TouchUpInside += Fab_TouchUpInside;
            }

            if (e.OldElement != null)
            {
                Control.TouchUpInside -= Fab_TouchUpInside;
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.SizeProperty.PropertyName)
            {
                SetSize();
            }
            else if (e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.NormalColorProperty.PropertyName ||
                     e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.RippleColorProperty.PropertyName ||
                     e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.DisabledColorProperty.PropertyName)
            {
                SetBackgroundColors();
            }
            else if (e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.HasShadowProperty.PropertyName)
            {
                SetHasShadow();
            }
            else if (e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.ImageSourceProperty.PropertyName ||
                     e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.WidthProperty.PropertyName ||
                     e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.HeightProperty.PropertyName)
            {
                SetImage();
            }
            else if (e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.IsEnabledProperty.PropertyName)
            {
                UpdateEnabled();
            }
            else if (e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.AnimateOnSelectionProperty.PropertyName)
            {
                UpdateAnimateOnSelection();
            }
            else
            {
                base.OnElementPropertyChanged(sender, e);
            }
        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            return new SizeRequest(new Size(fabSize, fabSize));
        }

        private void UpdateStyles()
        {
            SetSize();

            SetBackgroundColors();

            SetHasShadow();

            SetImage();

            UpdateEnabled();
        }

        private void SetSize()
        {
            switch (Element.Size)
            {
                case FabSize.Mini:
                    Control.Size = FabSize.Mini;
                    break;

                case FabSize.Normal:
                    Control.Size = FabSize.Normal;
                    break;
            }
        }

        private void SetBackgroundColors()
        {
            Control.BackgroundColor = Element.NormalColor.ToUIColor();
            //Control.PressedBackgroundColor = Element.Ripplecolor.ToUIColor();
        }

        private void SetHasShadow()
        {
            Control.HasShadow = Element.HasShadow;
        }

        private void SetImage()
        {
            SetImageAsync(Element.ImageSource, Control);
        }

        private void UpdateEnabled()
        {
            Control.Enabled = Element.IsEnabled;

            if (Control.Enabled == false)
            {
                Control.BackgroundColor = Element.DisabledColor.ToUIColor();
            }
            else
            {
                SetBackgroundColors();
            }
        }

        private void UpdateAnimateOnSelection()
        {
            Control.AnimateOnSelection = Element.AnimateOnSelection;
        }

        private void Fab_TouchUpInside(object sender, EventArgs e)
        {
            Element.SendClicked();
        }

        private async void SetImageAsync(ImageSource source, MNFloatingActionButton targetButton)
        {
            if (source != null)
            {
                var handler = GetHandler(source);
                using (UIImage image = await handler.LoadImageAsync(source))
                {
                    if (image != null)
                    {
                        UIGraphics.BeginImageContextWithOptions(new CGSize(imageSize, imageSize), false, UIScreen.MainScreen.Scale);
                        image.Draw(new CGRect(0, 0, imageSize, imageSize));
                        using (var resultImage = UIGraphics.GetImageFromCurrentImageContext())
                        {
                            if (resultImage != null)
                            {
                                UIGraphics.EndImageContext();
                                using (var resizableImage = resultImage.CreateResizableImage(new UIEdgeInsets(0f, 0f, (nfloat)imageSize, (nfloat)imageSize)))
                                {
                                    targetButton.CenterImageView.Image = resizableImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                                }
                            }
                        }
                    }
                    else
                    {
                        targetButton.CenterImageView.Image = null;
                    }
                }
            }
            else
            {
                targetButton.CenterImageView.Image = null;
            }
        }

        private IImageSourceHandler GetHandler(ImageSource source)
        {
            IImageSourceHandler returnValue = null;
            if (source is UriImageSource)
            {
                returnValue = new ImageLoaderSourceHandler();
            }
            else if (source is FileImageSource)
            {
                returnValue = new FileImageSourceHandler();
            }
            else if (source is StreamImageSource)
            {
                returnValue = new StreamImagesourceHandler();
            }
            return returnValue;
        }
    }
}