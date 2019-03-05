using System;

namespace Xamarin.Forms.Controls
{
    public class RoundedEditor : Editor
    {
        public static readonly BindableProperty NextFocusProperty = BindableProperty.Create(nameof(NextFocus), typeof(View), typeof(RoundedEditor), null);

        public View NextFocus
        {
            get { return (View)GetValue(NextFocusProperty); }
            set { SetValue(NextFocusProperty, value); }
        }

        public RoundedEditor()
        {
            Init();
        }

        private void Init()
        {
            Completed += OnEditorCompleted;
        }

        private void OnEditorCompleted(object sender, EventArgs e)
        {
            if (NextFocus is View view)
            {
                view.Focus();
            }
        }
    }
}