using Organilog.iOS.Effects;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("Organilog")]
[assembly: ExportEffect(typeof(BorderlessEffect), "BorderlessEffect")]

namespace Organilog.iOS.Effects
{
    public class BorderlessEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                Control.Layer.BorderWidth = 0;
                if (Control is UITextField entry)
                {
                    entry.BorderStyle = UITextBorderStyle.None;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
        }
    }
}