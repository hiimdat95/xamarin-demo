namespace Xamarin.Forms.Controls
{
    public class NoKeyboardEntry : Entry
    {
        public NoKeyboardEntry()
        {
            Init();
        }

        private void Init()
        {
            Focused += OnEntryFocused;
        }

        private void OnEntryFocused(object sender, FocusEventArgs e)
        {
            if (e.IsFocused)
            {
                Unfocus();
            }
        }
    }
}