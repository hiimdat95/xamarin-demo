using KamooniHost.Utils;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KamooniHost.Views.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CountrySelector : ContentView
    {
        public static readonly BindableProperty CountrySelectedCommandProperty = BindableProperty.Create(nameof(CountrySelectedCommand), typeof(ICommand), typeof(CountrySelector), default(ICommand));

        public ICommand CountrySelectedCommand
        {
            get { return (ICommand)GetValue(CountrySelectedCommandProperty); }
            set { SetValue(CountrySelectedCommandProperty, value); }
        }

        public event EventHandler<object> CountrySelected;

        private CancellationTokenSource cts;

        public CountrySelector()
        {
            InitializeComponent();

            PropertyChanged += CountrySelector_PropertyChanged;
            SBNationality.TextChanged += SBNationality_TextChanged;
            LVNationality.ItemTapped += LVNationality_ItemTapped;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => IsVisible = false;
            LblClose.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void CountrySelector_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == IsVisibleProperty.PropertyName)
            {
                if (IsVisible)
                {
                    SBNationality.Text = "";
                    //LVNationality.ItemsSource = CountryUtil.AllCountries;
                }
            }
        }

        private void SBNationality_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cts != null)
                cts.Cancel();

            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            Task.Run(() =>
            {
                Task.Delay(1500, token).Wait();

                if (string.IsNullOrWhiteSpace(SBNationality.Text))
                    return CountryUtil.AllCountries.OrderBy(c => c.Name).ToList();
                else
                    return CountryUtil.AllCountries.FindAll(country => country.Name.ToUpper().Contains(SBNationality.Text.ToUpper())).OrderBy(c => c.Name).ToList();
            }, token).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    LVNationality.ItemsSource = task.Result;
                }
            }));
        }

        private void LVNationality_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (CountrySelectedCommand != null && CountrySelectedCommand.CanExecute(e.Item))
            {
                CountrySelectedCommand.Execute(e.Item);
            }

            CountrySelected?.Invoke(this, e.Item);

            IsVisible = false;
        }
    }
}