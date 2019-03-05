using Android.Widget;

namespace KamooniHost.Droid.Controls.FloatingActionButton
{
    public interface IOnScrollChangedListener
    {
        void OnScrollChanged(ScrollView who, int l, int t, int oldl, int oldt);
    }
}