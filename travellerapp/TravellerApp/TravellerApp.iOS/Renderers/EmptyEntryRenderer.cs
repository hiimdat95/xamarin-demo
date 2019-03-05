using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using TravellerApp.Controls;
using TravellerApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(EmptyEntry), typeof(EmptyEntryRenderer))]
namespace TravellerApp.iOS.Renderers
{
    public class EmptyEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;
            Control.LeftView = new UIView(new CGRect(0, 0, 15, 0));
            Control.LeftViewMode = UITextFieldViewMode.Always;
            Control.RightView = new UIView(new CGRect(0, 0, 15, 0));
            Control.RightViewMode = UITextFieldViewMode.Always;
            Control.BorderStyle = UITextBorderStyle.None;
        }
    }

}