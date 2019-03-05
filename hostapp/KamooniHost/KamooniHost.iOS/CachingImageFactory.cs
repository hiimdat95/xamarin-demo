using System.Collections.Concurrent;
using UIKit;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.iOS.Factories;

namespace KamooniHost.iOS
{
    public class CachingImageFactory : IImageFactory
    {
        private readonly ConcurrentDictionary<string, UIImage> _cache = new ConcurrentDictionary<string, UIImage>();

        public UIImage ToUIImage(BitmapDescriptor descriptor)
        {
            var defaultFactory = DefaultImageFactory.Instance;

            if (!string.IsNullOrEmpty(descriptor.Id))
            {
                return _cache.GetOrAdd(descriptor.Id, _ => defaultFactory.ToUIImage(descriptor));
            }

            return defaultFactory.ToUIImage(descriptor);
        }
    }
}