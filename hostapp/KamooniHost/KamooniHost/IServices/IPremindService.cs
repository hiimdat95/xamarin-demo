using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace KamooniHost.IServices
{
    public interface IPremindService
    {
        void InitContent(View content);

        void ShowContent();

        void HideContent();

        void InitIcon(CircleImage icon);

        void ShowIcon();

        void HideIcon();
    }
}