using KamooniHost.iOS.Renderers;
using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NoKeyboardEntry), typeof(NoKeyboardEntryRenderer))]

namespace KamooniHost.iOS.Renderers
{
    public class NoKeyboardEntryRenderer : EntryRenderer
    {
        public NoKeyboardEntryRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null && Element != null)
            {
            }

            if (Control != null)
            {
                // Disabling the keyboard
                Control.InputView = new UIView();

                Control.TintColor = UIColor.Clear;

                //Control.SpellCheckingType = UITextSpellCheckingType.No;             // No Spellchecking
                //Control.AutocorrectionType = UITextAutocorrectionType.No;           // No Autocorrection
                //Control.AutocapitalizationType = UITextAutocapitalizationType.None; // No Autocapitalization
            }
        }
    }
}