using System;

namespace Xamarin.Forms.Controls
{
    public class RoundedEntry : Entry
    {
        public static readonly BindableProperty NextFocusProperty = BindableProperty.Create(nameof(NextFocus), typeof(View), typeof(RoundedEntry), null);

        public View NextFocus
        {
            get { return (View)GetValue(NextFocusProperty); }
            set { SetValue(NextFocusProperty, value); }
        }

        public RoundedEntry()
        {
            Init();
        }

        private void Init()
        {
            Completed += OnEntryCompleted;
        }

        private void OnEntryCompleted(object sender, EventArgs e)
        {
            if (NextFocus is View view)
            {
                view.Focus();
            }
        }
    }
}