using System.Linq;
using Xamarin.Forms;

namespace TravellerApp.Utils
{
    public static class PageExtensions
    {
        public static bool IsModal(this Page page)
        {
            for (int i = 0; i < page.Navigation.ModalStack.Count(); i++)
            {
                if (page == page.Navigation.ModalStack[i])
                    return true;
            }
            return false;
        }
    }
}