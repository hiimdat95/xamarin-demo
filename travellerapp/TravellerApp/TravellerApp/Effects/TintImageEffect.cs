using System;
using Xamarin.Forms;

namespace TravellerApp.Effects
{

        public class TintImageEffect : RoutingEffect
        {
            public const string GroupName = "TravellerApp";
            public const string Name = "TintImageEffect";

            public Color TintColor { get; set; }

            public TintImageEffect() : base($"{GroupName}.{Name}") { }

    }
}
