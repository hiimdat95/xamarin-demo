using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Xamarin.Forms.Controls
{
    public class ExtendedListView : ListView
    {
        public static readonly BindableProperty ItemSelectedCommandProperty = BindableProperty.Create(nameof(ItemSelectedCommand), typeof(ICommand), typeof(RepeaterView), default(ICommand));

        public static readonly BindableProperty ItemSelectedCommandParameterProperty = BindableProperty.Create(nameof(ItemSelectedCommandParameter), typeof(object), typeof(RepeaterView), default);

        public static readonly BindableProperty ItemTappedCommandProperty = BindableProperty.Create(nameof(ItemTappedCommand), typeof(ICommand), typeof(RepeaterView), default(ICommand));

        public static readonly BindableProperty ItemTappedCommandParameterProperty = BindableProperty.Create(nameof(ItemTappedCommandParameter), typeof(object), typeof(RepeaterView), default);

        public ICommand ItemSelectedCommand
        {
            get { return (ICommand)GetValue(ItemTappedCommandProperty); }
            set { SetValue(ItemTappedCommandProperty, value); }
        }

        public object ItemSelectedCommandParameter
        {
            get { return GetValue(ItemTappedCommandParameterProperty); }
            set { SetValue(ItemTappedCommandParameterProperty, value); }
        }

        public ICommand ItemTappedCommand
        {
            get { return (ICommand)GetValue(ItemTappedCommandProperty); }
            set { SetValue(ItemTappedCommandProperty, value); }
        }

        public object ItemTappedCommandParameter
        {
            get { return GetValue(ItemTappedCommandParameterProperty); }
            set { SetValue(ItemTappedCommandParameterProperty, value); }
        }

        public ExtendedListView() : base(ListViewCachingStrategy.RecycleElement)
        {
            Init();
        }

        private void Init()
        {
            ItemSelected += ExtendedListView_ItemSelected;
            ItemTapped += ExtendedListView_ItemTapped;
        }

        private void ExtendedListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                var param = ItemSelectedCommandParameter ?? e.SelectedItem;

                if (ItemSelectedCommand != null && ItemSelectedCommand.CanExecute(param))
                {
                    ItemSelectedCommand.Execute(param);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void ExtendedListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                var param = ItemTappedCommandParameter ?? e.Item;

                if (ItemTappedCommand != null && ItemTappedCommand.CanExecute(param))
                {
                    ItemTappedCommand.Execute(param);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}