using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KamooniHost.Views.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupView : ContentView
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(PopupView), default(string), BindingMode.TwoWay, propertyChanged: (b, o, n) => ((PopupView)b).OnTitleChanged(b, o, n));

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(PopupView), default(IEnumerable), BindingMode.TwoWay, propertyChanged: (b, o, n) => ((PopupView)b).OnItemsSourceChanged(b, o, n));

        public static readonly BindableProperty CloseProperty = BindableProperty.Create(nameof(Close), typeof(string), typeof(PopupView), default(string), BindingMode.TwoWay, propertyChanged: (b, o, n) => ((PopupView)b).OnCloseChanged(b, o, n));

        public static readonly BindableProperty ItemSelectedCommandProperty = BindableProperty.Create(nameof(ItemSelectedCommand), typeof(ICommand), typeof(PopupView), null, propertyChanged: (b, o, n) => ((PopupView)b).OnItemSelectedCommandChanged());

        public static readonly BindableProperty ItemTappedCommandProperty = BindableProperty.Create(nameof(ItemTappedCommand), typeof(ICommand), typeof(PopupView), null, propertyChanged: (b, o, n) => ((PopupView)b).OnItemTappedCommandChanged());

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string Close
        {
            get { return (string)GetValue(CloseProperty); }
            set { SetValue(CloseProperty, value); }
        }

        public ICommand ItemSelectedCommand
        {
            get { return (ICommand)GetValue(ItemSelectedCommandProperty); }
            set { SetValue(ItemSelectedCommandProperty, value); }
        }

        public ICommand ItemTappedCommand
        {
            get { return (ICommand)GetValue(ItemTappedCommandProperty); }
            set { SetValue(ItemTappedCommandProperty, value); }
        }

        private BindingBase itemTextBinding;

        public BindingBase ItemTextBinding
        {
            get { return itemTextBinding; }
            set
            {
                if (itemTextBinding == value)
                    return;
                OnPropertyChanging();
                var oldValue = value;
                itemTextBinding = value;
                OnItemTextBindingChanged(oldValue, itemTextBinding);
                OnPropertyChanged();
            }
        }

        private BindingBase itemDetailBinding;

        public BindingBase ItemDetailBinding
        {
            get { return itemDetailBinding; }
            set
            {
                if (itemDetailBinding == value)
                    return;
                OnPropertyChanging();
                var oldValue = value;
                itemDetailBinding = value;
                OnItemDetailBindingChanged(oldValue, itemDetailBinding);
                OnPropertyChanged();
            }
        }

        public event EventHandler<object> ItemSelected;
        public event EventHandler<object> ItemTapped;

        public PopupView()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == "IsVisible")
            {
                if (IsVisible == true)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await this.FadeTo(1, 150);
                    });
                }
            }
        }

        private void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            LblTitle.Text = (string)newValue;
        }

        private void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ListView.ItemsSource = (IEnumerable)newValue;
        }

        private void OnItemTextBindingChanged(BindingBase oldValue, BindingBase itemTextBinding)
        {
            ResetItems();
        }

        private void OnItemDetailBindingChanged(BindingBase oldValue, BindingBase itemTextBinding)
        {
            ResetItems();
        }

        private void ResetItems()
        {
            var itemTemplate = new DataTemplate(() =>
            {
                ViewCell viewCell = new ViewCell();
                Grid grid = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnSpacing = 1,
                    RowSpacing = 1,
                    ColumnDefinitions = new ColumnDefinitionCollection()
                    {
                        new ColumnDefinition()
                        {
                            Width = GridLength.Star
                        }
                    },
                    RowDefinitions = new RowDefinitionCollection()
                    {
                        new RowDefinition()
                        {
                            Height = GridLength.Auto
                        },
                        new RowDefinition()
                        {
                            Height = GridLength.Auto
                        }
                    }
                };
                if (ItemTextBinding != null)
                {
                    Label label = new Label()
                    {
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 13,
                        TextColor = Color.Black
                    };
                    label.SetBinding(Label.TextProperty, ((Binding)ItemTextBinding).Path);
                    grid.Children.Add(label, 0, 0);
                }
                if (ItemDetailBinding != null)
                {
                    Label label = new Label()
                    {
                        FontSize = 13,
                        TextColor = Color.Black
                    };
                    label.SetBinding(Label.TextProperty, ((Binding)ItemDetailBinding).Path);
                    grid.Children.Add(label, 0, 1);
                }
                viewCell.View = grid;
                return viewCell;
            });
            ListView.ItemTemplate = itemTemplate;
        }

        private void OnCloseChanged(BindableObject bindable, object oldValue, object newValue)
        {
            LblClose.Text = (string)newValue;
        }

        private void OnItemSelectedCommandChanged()
        {
            if (ItemSelectedCommand != null)
            {
                ItemSelectedCommand.CanExecuteChanged += OnItemSelectedCommandCanExecuteChanged;
                OnItemSelectedCommandCanExecuteChanged(this, EventArgs.Empty);
            }
            else
                IsEnabled = true;
        }

        private void OnItemSelectedCommandCanExecuteChanged(object sender, EventArgs e)
        {
            ICommand cmd = ItemSelectedCommand;
            if (cmd != null)
                IsEnabled = cmd.CanExecute(null);
        }

        private void OnItemTappedCommandChanged()
        {
            if (ItemTappedCommand != null)
            {
                ItemTappedCommand.CanExecuteChanged += OnItemTappedCommandCanExecuteChanged;
                OnItemTappedCommandCanExecuteChanged(this, EventArgs.Empty);
            }
            else
                IsEnabled = true;
        }

        private void OnItemTappedCommandCanExecuteChanged(object sender, EventArgs e)
        {
            ICommand cmd = ItemTappedCommand;
            if (cmd != null)
                IsEnabled = cmd.CanExecute(null);
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListView.SelectedItem = null;
            if (ItemSelectedCommand != null && ItemSelectedCommand.CanExecute(e.SelectedItem))
            {
                ItemSelectedCommand.Execute(e.SelectedItem);
            }
            ItemSelected?.Invoke(this, e.SelectedItem);
            Close_Tapped(this, null);
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (ItemTappedCommand != null && ItemTappedCommand.CanExecute(e.Item))
            {
                ItemTappedCommand.Execute(e.Item);
            }
            ItemTapped?.Invoke(this, e.Item);
            Close_Tapped(this, null);
        }

        private async void Close_Tapped(object sender, EventArgs e)
        {
            await this.FadeTo(0, 150);
            IsVisible = false;
        }
    }
}