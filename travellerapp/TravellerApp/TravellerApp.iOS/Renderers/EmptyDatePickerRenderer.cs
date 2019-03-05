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

[assembly: Xamarin.Forms.ExportRenderer(typeof(EmptyDatePicker), typeof(EmptyDatePickerRenderer))]
namespace TravellerApp.iOS.Renderers
{
    public class EmptyDatePickerRenderer : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;
            Control.LeftView = new UIView(new CGRect(0, 0, 0, 0));
            Control.LeftViewMode = UITextFieldViewMode.Always;
            Control.RightView = new UIView(new CGRect(0, 0, 0, 0));
            Control.RightViewMode = UITextFieldViewMode.Always;
            Control.BorderStyle = UITextBorderStyle.None;
        }
    }

}