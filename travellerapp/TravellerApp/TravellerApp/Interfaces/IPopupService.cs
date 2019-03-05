using Xamarin.Forms;

namespace TravellerApp.Interfaces
{
    public interface IPopupService
    {
        void ShowContent(View content);

        void HideContent();
    }
}