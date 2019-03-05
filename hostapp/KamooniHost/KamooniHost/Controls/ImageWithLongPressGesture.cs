namespace Xamarin.Forms.Controls
{
    public class ImageWithLongPressGesture : Image
    {
        public delegate void LongPress(object sender);
        public event LongPress LongPressed;

        public void InvokeLongPressedEvent()
        {
            LongPressed(this);
        }
    }
}
