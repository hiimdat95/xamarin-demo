using Android.Content;
using Android.Views;
using TravellerApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ImageWithLongPressGesture), typeof(LongPressGestureRecognizerImageRenderer))]

namespace TravellerApp.Droid.Renderers
{
    public class LongPressGestureRecognizerImageRenderer : ImageRenderer
    {
        private readonly GestureDetector.SimpleOnGestureListener _listener;
        private readonly GestureDetector _detector;

        public LongPressGestureRecognizerImageRenderer(Context context) : base(context)
        {
            _listener = new GestureDetector.SimpleOnGestureListener();
            _detector = new GestureDetector(_listener);
            _detector.IsLongpressEnabled = true;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
            {
                this.GenericMotion -= HandleGenericMotion;
                this.LongClick -= HandleLongClick;
            }

            if (e.OldElement == null)
            {
                this.GenericMotion += HandleGenericMotion;
                this.LongClick += HandleLongClick;
            }
        }

        private void HandleLongClick(object sender, LongClickEventArgs e)
        {
            ((ImageWithLongPressGesture)Element).InvokeLongPressedEvent();
        }

        private void HandleGenericMotion(object sender, GenericMotionEventArgs e)
        {
            _detector.OnTouchEvent(e.Event);
        }
    }
}