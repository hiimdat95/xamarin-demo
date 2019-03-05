using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FloatingActionButton), typeof(KamooniHost.Droid.Controls.FloatingActionButton.FloatingActionButtonRenderer))]

namespace KamooniHost.Droid.Controls.FloatingActionButton
{
    public class FloatingActionButtonRenderer : ViewRenderer<Xamarin.Forms.Controls.FloatingActionButton, FrameLayout>
    {
        /// <summary>
        /// Used for registration with dependency service
        /// </summary>
        public static void Init()
        {
            var temp = DateTime.Now;
        }

        private const int MARGIN_DIPS = 16;
        private const int FAB_HEIGHT_NORMAL = 56;
        private const int FAB_HEIGHT_MINI = 40;
        private const int FAB_FRAME_HEIGHT_WITH_PADDING = (MARGIN_DIPS * 2) + FAB_HEIGHT_NORMAL;
        private const int FAB_FRAME_WIDTH_WITH_PADDING = (MARGIN_DIPS * 2) + FAB_HEIGHT_NORMAL;
        private const int FAB_MINI_FRAME_HEIGHT_WITH_PADDING = (MARGIN_DIPS * 2) + FAB_HEIGHT_MINI;
        private const int FAB_MINI_FRAME_WIDTH_WITH_PADDING = (MARGIN_DIPS * 2) + FAB_HEIGHT_MINI;
        private readonly Context context;
        private readonly FloatingActionButton fab;

        /// <summary>
        /// Construtor
        /// </summary>
        public FloatingActionButtonRenderer(Context context) : base(context)
        {
            this.context = context;

            float d = context.Resources.DisplayMetrics.Density;
            var margin = (int)(MARGIN_DIPS * d); // margin in pixels

            fab = new FloatingActionButton(context);
            var lp = new FrameLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent)
            {
                Gravity = GravityFlags.CenterVertical | GravityFlags.CenterHorizontal,
                LeftMargin = margin,
                TopMargin = margin,
                BottomMargin = margin,
                RightMargin = margin
            };
            fab.LayoutParameters = lp;
        }

        /// <summary>
        /// Element Changed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Controls.FloatingActionButton> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            if (e.OldElement != null)
                e.OldElement.PropertyChanged -= HandlePropertyChanged;

            if (Element != null)
            {
                //UpdateContent();
                Element.PropertyChanged += HandlePropertyChanged;
            }

            Element.Show = Show;
            Element.Hide = Hide;

            SetFabImage(Element.ImageSource);
            SetFabSize(Element.Size);

            fab.ColorNormal = Element.NormalColor.ToAndroid();
            fab.ColorPressed = Element.PressedColor.ToAndroid();
            fab.ColorRipple = Element.RippleColor.ToAndroid();
            fab.HasShadow = Element.HasShadow;
            fab.Click += Fab_Click;

            var frame = new FrameLayout(context);
            frame.RemoveAllViews();
            frame.AddView(fab);

            SetNativeControl(frame);
        }

        /// <summary>
        /// Show
        /// </summary>
        /// <param name="animate"></param>
        public void Show(bool animate = true) => fab?.Show(animate);

        /// <summary>
        /// Hide!
        /// </summary>
        /// <param name="animate"></param>
        public void Hide(bool animate = true) => fab?.Hide(animate);

        private void HandlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Content")
            {
                Tracker.UpdateLayout();
            }
            else if (e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.NormalColorProperty.PropertyName)
            {
                fab.ColorNormal = Element.NormalColor.ToAndroid();
            }
            else if (e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.PressedColorProperty.PropertyName)
            {
                fab.ColorPressed = Element.PressedColor.ToAndroid();
            }
            else if (e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.RippleColorProperty.PropertyName)
            {
                fab.ColorRipple = Element.RippleColor.ToAndroid();
            }
            else if (e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.ImageSourceProperty.PropertyName)
            {
                SetFabImage(Element.ImageSource);
            }
            else if (e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.SizeProperty.PropertyName)
            {
                SetFabSize(Element.Size);
            }
            else if (e.PropertyName == Xamarin.Forms.Controls.FloatingActionButton.HasShadowProperty.PropertyName)
            {
                fab.HasShadow = Element.HasShadow;
            }
        }

        private void SetFabImage(ImageSource imageSource)
        {
            if (imageSource is FileImageSource fileImageSource)
            {
                try
                {
                    var drawableNameWithoutExtension = Path.GetFileNameWithoutExtension(fileImageSource.File.ToLower());
                    var resources = context.Resources;
                    var imageResourceName = resources.GetIdentifier(drawableNameWithoutExtension, "drawable", context.PackageName);
                    fab.SetImageBitmap(Android.Graphics.BitmapFactory.DecodeResource(context.Resources, imageResourceName));
                }
                catch (Exception ex)
                {
                    throw new FileNotFoundException("There was no Android Drawable by that name.", ex);
                }
            }
        }

        private void SetFabSize(FabSize size)
        {
            if (size == FabSize.Mini)
            {
                fab.Size = FabSize.Mini;
                Element.WidthRequest = FAB_MINI_FRAME_WIDTH_WITH_PADDING;
                Element.HeightRequest = FAB_MINI_FRAME_HEIGHT_WITH_PADDING;
            }
            else
            {
                fab.Size = FabSize.Normal;
                Element.WidthRequest = FAB_FRAME_WIDTH_WITH_PADDING;
                Element.HeightRequest = FAB_FRAME_HEIGHT_WITH_PADDING;
            }
        }

        private void Fab_Click(object sender, EventArgs e)
        {
            Element?.SendClicked();
        }
    }
}