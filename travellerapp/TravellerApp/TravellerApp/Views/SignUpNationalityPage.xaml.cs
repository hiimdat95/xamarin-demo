using Realms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TravellerApp.Constants;
using TravellerApp.Models;
using TravellerApp.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpNationalityPage : ContentPage
    {
        private Page modalParentPage;

        private List<Country> countryList;
        private ObservableCollection<Country> countryCollection;

        public SignUpNationalityPage(Page modalParentPage)
        {
            this.modalParentPage = modalParentPage;

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, true);

            countryList = Country.All;
            countryCollection = new ObservableCollection<Country>(countryList);

            countryListView.ItemsSource = countryCollection;
            countryListView.ItemTapped += (s, e) => OnCountrySelect(s, e);

            countrySearchBar.TextChanged += (s, e) => OnSearchTextChanged(s, e);
        }

        private async void OnCountrySelect(object sender, ItemTappedEventArgs e)
        {
            Country selectedCountry = (Country)e.Item;
            countryListView.SelectedItem = null;

            if (PageExtensions.IsModal(this))
            {
                MyProfilePage parentPage = (MyProfilePage)modalParentPage;

                parentPage.OnNationalitySelected(selectedCountry.IsoCode);
                await Navigation.PopModalAsync();
            }
            else
            {
                var realm = Realm.GetInstance();
                User user = realm.Find<User>(DBLocalID.USER_TEMP_SIGN_UP);

                realm.Write(() =>
                {
                    user.country = selectedCountry.IsoCode;
                    realm.Add(user, update: true);
                });

                await Navigation.PushAsync(new SignUpProfilePicturePage());
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            Search(countrySearchBar.Text.ToString());
        }

        private void Search(string text)
        {
            countryCollection.Clear();

            foreach (var item in countryList)
            {
                string itemName = item.Name.Trim().ToLower();
                string searchText = text.Trim().ToLower();

                if (itemName.Contains(searchText))
                {
                    countryCollection.Add(item);
                }
            }
        }
    }
}