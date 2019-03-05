using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TravellerApp.Controls
{
  public class EmptyEntry : Entry
  {
    public static readonly BindableProperty PaddingProperty = BindableProperty.Create(nameof(Padding), typeof(string), typeof(EmptyEntry), "0,0,0,0");

    public string Padding
    {
      get => (string)GetValue(PaddingProperty);
      set => SetValue(PaddingProperty, value);
    }

  }
}
