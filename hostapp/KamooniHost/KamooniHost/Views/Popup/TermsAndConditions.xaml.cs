using KamooniHost.Models;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KamooniHost.Views.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermsAndConditions : ContentView
    {
        public static readonly BindableProperty TermsAndConditionsAgreedCommandProperty = BindableProperty.Create(nameof(TermsAndConditionsAgreedCommand), typeof(ICommand), typeof(TermsAndConditions), default(ICommand));

        public ICommand TermsAndConditionsAgreedCommand
        {
            get { return (ICommand)GetValue(TermsAndConditionsAgreedCommandProperty); }
            set { SetValue(TermsAndConditionsAgreedCommandProperty, value); }
        }

        public event EventHandler TermsAndConditionsAgreed;

        public Host Host => Helpers.Settings.CurrentHost;

        public TermsAndConditions()
        {
            InitializeComponent();

            LblTerms.Source = new HtmlWebViewSource
            {
                Html = Host.Terms,
            };
            // Host.Terms;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(Host?.UrlTerms))
                    Device.OpenUri(new Uri(Host.UrlTerms));
            };
            LblClickHere.GestureRecognizers.Add(tapGestureRecognizer);

            BtnAgree.Clicked += BtnAgree_Clicked;
        }

        private void BtnAgree_Clicked(object sender, EventArgs e)
        {
            if (TermsAndConditionsAgreedCommand != null && TermsAndConditionsAgreedCommand.CanExecute(null))
            {
                TermsAndConditionsAgreedCommand.Execute(null);
            }

            TermsAndConditionsAgreed?.Invoke(this, EventArgs.Empty);

            IsVisible = false;
        }
    }
}