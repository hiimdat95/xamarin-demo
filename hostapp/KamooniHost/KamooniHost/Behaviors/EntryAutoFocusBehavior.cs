using System;

namespace Xamarin.Forms.Behaviors
{
    public class EntryAutoFocusBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty TargetProperty = BindableProperty.Create(nameof(Target), typeof(View), typeof(EntryAutoFocusBehavior), null);

        public View Target
        {
            get { return (View)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.Completed += OnEntryCompleted;

            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.Completed -= OnEntryCompleted;

            base.OnDetachingFrom(bindable);
        }

        private void OnEntryCompleted(object sender, EventArgs e)
        {
            if (sender is Entry entry)
            {
                if (Target is View view)
                {
                    view.Focus();
                }
            }
        }
    }
}