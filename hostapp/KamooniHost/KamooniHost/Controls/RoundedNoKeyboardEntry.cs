namespace Xamarin.Forms.Controls
{
    public class RoundedNoKeyboardEntry : Entry
    {
        public RoundedNoKeyboardEntry()
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