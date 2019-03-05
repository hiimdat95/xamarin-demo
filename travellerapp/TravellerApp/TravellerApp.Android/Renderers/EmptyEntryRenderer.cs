using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using System;
using System.Linq;
using TravellerApp.Controls;
using TravellerApp.Droid.Renderers;

[assembly: Xamarin.Forms.ExportRenderer(typeof(EmptyEntry), typeof(EmptyEntryRenderer))]
namespace TravellerApp.Droid.Renderers
{
    public class EmptyEntryRenderer : EntryRenderer
    {
        public EmptyEntryRenderer(Context context) : base(context)
        {
        }

        /// <summary>
        /// Gets the underlying control typed as an <see cref="EmptyEntry"/>.
        /// </summary>
        private EmptyEntry Entry
        {
            get { return (EmptyEntry)Element; }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Entry> e)
        {
            base.OnElementChanged(e);
            if (Control == null || e.NewElement == null) return;
            Control.Background = null;
            this.Control.Gravity = GravityFlags.CenterVertical;

            var padding = (Entry.Padding ?? "0,0,0,0").Split(',').Select(Int32.Parse).ToList();
            Control.SetPadding(padding[0], padding[1], padding[2], padding[3]);
        }
    }

}