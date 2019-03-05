using System;
using TravellerApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ImageWithLongPressGesture), typeof(LongPressGestureRecognizerImageRenderer))]
namespace TravellerApp.iOS.Renderers
{
    public class LongPressGestureRecognizerImageRenderer : ImageRenderer
    {
        UILongPressGestureRecognizer longPressGestureRecognizer;
        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            longPressGestureRecognizer = new UILongPressGestureRecognizer(() => ((ImageWithLongPressGesture)Element).InvokeLongPressedEvent());

            if (e.NewElement != null)
                RemoveGestureRecognizer(longPressGestureRecognizer);

            if (e.OldElement != null)
                AddGestureRecognizer(longPressGestureRecognizer);
        }
    }
}
