using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Xamarin.Forms.Platform.Android;
using TravellerApp.Controls;
using TravellerApp.Droid.Renderers;
using Xamarin.Forms;
using Android.Views;
using System.Collections.Generic;

[assembly: ExportRenderer(typeof(CustomImageButton), typeof(CustomImageButtonRenderer))]
namespace TravellerApp.Droid.Renderers
{
    /// <summary>
    /// Draws a button on the Android platform with the image shown in the right 
    /// position with the right size.
    /// </summary>
    public partial class CustomImageButtonRenderer : ButtonRenderer
    {
        private static float _density = float.MinValue;

        public CustomImageButtonRenderer(Context context) : base(context)
        {
        }

        /// <summary>
        /// Gets the underlying control typed as an <see cref="ImageButton"/>.
        /// </summary>
        private CustomImageButton ImageButton
        {
            get { return (CustomImageButton)Element; }
        }

        /// <summary>
        /// Sets up the button including the image. 
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected async override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            _density = Resources.DisplayMetrics.Density;

            var targetButton = Control;
            if (targetButton != null) targetButton.SetOnTouchListener(TouchListener.Instance.Value);

            if (Element != null && Element.Font != Font.Default && targetButton != null) targetButton.Typeface = ToExtendedTypeface(Element.Font,Context);

            if (Element != null && ImageButton.Source != null) await SetImageSourceAsync(targetButton, ImageButton).ConfigureAwait(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && Control != null)
            {
                Control.Dispose();
            }
        }


        /// <summary>
        /// Sets the image source.
        /// </summary>
        /// <param name="targetButton">The target button.</param>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="Task"/> for the awaited operation.</returns>
        private async Task SetImageSourceAsync(global::Android.Widget.Button targetButton, CustomImageButton model)
        {
            if (targetButton == null || targetButton.Handle == IntPtr.Zero || model == null) return;

            // const int Padding = 10;
            var source = model.IsEnabled ? model.Source : model.DisabledSource ?? model.Source;

            using (var bitmap = await GetBitmapAsync(source).ConfigureAwait(false))
            {
                if (bitmap == null)
                    targetButton.SetCompoundDrawables(null, null, null, null);
                else
                {
                    var drawable = new BitmapDrawable(bitmap);
                    var tintColor = model.IsEnabled ? model.ImageTintColor : model.DisabledImageTintColor;
                    if (tintColor != Xamarin.Forms.Color.Transparent)
                    {
                        drawable.SetTint(tintColor.ToAndroid());
                        drawable.SetTintMode(PorterDuff.Mode.SrcIn);
                    }

                    using (var scaledDrawable = GetScaleDrawable(drawable, GetWidth(model.ImageWidthRequest), GetHeight(model.ImageHeightRequest)))
                    {
                        Drawable left = null;
                        Drawable right = null;
                        Drawable top = null;
                        Drawable bottom = null;
                        //System.Diagnostics.Debug.WriteLine($"SetImageSourceAsync intptr{targetButton.Handle}");
                        int padding = 10; // model.Padding
                        targetButton.CompoundDrawablePadding = RequestToPixels(padding);
                        switch (model.Orientation)
                        {
                            case ImageOrientation.ImageToLeft:
                                targetButton.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
                                left = scaledDrawable;
                                break;
                            case ImageOrientation.ImageToRight:
                                targetButton.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
                                right = scaledDrawable;
                                break;
                            case ImageOrientation.ImageOnTop:
                                targetButton.Gravity = GravityFlags.Top | GravityFlags.CenterHorizontal;
                                top = scaledDrawable;
                                break;
                            case ImageOrientation.ImageOnBottom:
                                targetButton.Gravity = GravityFlags.Bottom | GravityFlags.CenterHorizontal;
                                bottom = scaledDrawable;
                                break;
                            case ImageOrientation.ImageCentered:
                                targetButton.Gravity = GravityFlags.Center; // | GravityFlags.Fill;
                                top = scaledDrawable;
                                break;
                        }

                        targetButton.SetCompoundDrawables(left, top, right, bottom);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="Bitmap"/> for the supplied <see cref="ImageSource"/>.
        /// </summary>
        /// <param name="source">The <see cref="ImageSource"/> to get the image for.</param>
        /// <returns>A loaded <see cref="Bitmap"/>.</returns>
        private async Task<Bitmap> GetBitmapAsync(ImageSource source)
        {
            var handler = GetHandler(source);
            var returnValue = (Bitmap)null;

            if (handler != null)
                returnValue = await handler.LoadImageAsync(source, Context).ConfigureAwait(false);

            return returnValue;
        }

        /// <summary>
        /// Called when the underlying model's properties are changed.
        /// </summary>
        /// <param name="sender">The Model used.</param>
        /// <param name="e">The event arguments.</param>
        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == CustomImageButton.SourceProperty.PropertyName ||
                e.PropertyName == CustomImageButton.DisabledSourceProperty.PropertyName ||
                e.PropertyName == VisualElement.IsEnabledProperty.PropertyName ||
                e.PropertyName == CustomImageButton.ImageTintColorProperty.PropertyName ||
                e.PropertyName == CustomImageButton.DisabledImageTintColorProperty.PropertyName)
            {
                await SetImageSourceAsync(Control, ImageButton).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Returns a <see cref="Drawable"/> with the correct dimensions from an 
        /// Android resource id.
        /// </summary>
        /// <param name="drawable">An android <see cref="Drawable"/>.</param>
        /// <param name="width">The width to scale to.</param>
        /// <param name="height">The height to scale to.</param>
        /// <returns>A scaled <see cref="Drawable"/>.</returns>
        private Drawable GetScaleDrawable(Drawable drawable, int width, int height)
        {
            var returnValue = new ScaleDrawable(drawable, 0, 100, 100).Drawable;

            returnValue.SetBounds(0, 0, RequestToPixels(width), RequestToPixels(height));

            return returnValue;
        }

        /// <summary>
        /// Returns a drawable dimension modified according to the current display DPI.
        /// </summary>
        /// <param name="sizeRequest">The requested size in relative units.</param>
        /// <returns>Size in pixels.</returns>
        public int RequestToPixels(int sizeRequest)
        {
            if (_density == float.MinValue)
            {
                if (Resources.Handle == IntPtr.Zero || Resources.DisplayMetrics.Handle == IntPtr.Zero)
                    _density = 1.0f;
                else
                    _density = Resources.DisplayMetrics.Density;
            }

            return (int)(sizeRequest * _density);
        }

        public Typeface ToExtendedTypeface(Font font, Context context)
        {
            Typeface typeface = null;

            //1. Lookup in the cache
            var hashKey = ToHasmapKey(font);
            typeface = TypefaceCache.SharedCache.RetrieveTypeface(hashKey);
#if DEBUG
            if (typeface != null)
                Console.WriteLine("Typeface for font {0} found in cache", font);
#endif

            //2. If not found, try custom asset folder
            if (typeface == null && !string.IsNullOrEmpty(font.FontFamily))
            {
                string filename = font.FontFamily;
                //if no extension given then assume and add .ttf
                if (filename.LastIndexOf(".", System.StringComparison.Ordinal) != filename.Length - 4)
                {
                    filename = string.Format("{0}.ttf", filename);
                }
                try
                {
                    var path = "fonts/" + filename;
#if DEBUG
                    Console.WriteLine("Lookking for font file: {0}", path);
#endif
                    typeface = Typeface.CreateFromAsset(context.Assets, path);
#if DEBUG
                    Console.WriteLine("Found in assets and cached.");
#endif
#pragma warning disable CS0168 // Variable is declared but never used
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine("not found in assets. Exception: {0}", ex);
                    Console.WriteLine("Trying creation from file");
#endif
                    try
                    {
                        typeface = Typeface.CreateFromFile("fonts/" + filename);


#if DEBUG
                        Console.WriteLine("Found in file and cached.");
#endif
                    }
                    catch (Exception ex1)
#pragma warning restore CS0168 // Variable is declared but never used
                    {
#if DEBUG
                        Console.WriteLine("not found by file. Exception: {0}", ex1);
                        Console.WriteLine("Trying creation using Xamarin.Forms implementation");
#endif

                    }
                }

            }
            //3. If not found, fall back to default Xamarin.Forms implementation to load system font
            if (typeface == null)
            {
                typeface = font.ToTypeface();
            }

            if (typeface == null)
            {
#if DEBUG
                Console.WriteLine("Falling back to default typeface");
#endif
                typeface = Typeface.Default;
            }
            //Store in cache
            TypefaceCache.SharedCache.StoreTypeface(hashKey, typeface);

            return typeface;

        }

        /// <summary>
        /// Provides unique identifier for the given font.
        /// </summary>
        /// <returns>Unique string identifier for the given font</returns>
        /// <param name="font">Font.</param>
        private static string ToHasmapKey(Font font)
        {
            return string.Format("{0}.{1}.{2}.{3}", font.FontFamily, font.FontSize, font.NamedSize, (int)font.FontAttributes);
        }
    }

    //Hot fix for the layout positioning issue on Android as described in http://forums.xamarin.com/discussion/20608/fix-for-button-layout-bug-on-android
    class TouchListener : Java.Lang.Object, global::Android.Views.View.IOnTouchListener
    {
        public static readonly Lazy<TouchListener> Instance = new Lazy<TouchListener>(() => new TouchListener());

        /// <summary>
        /// Make TouchListener a singleton.
        /// </summary>
        private TouchListener()
        { }

        public bool OnTouch(global::Android.Views.View v, MotionEvent e)
        {
            var buttonRenderer = v.Tag as ButtonRenderer;
            if (buttonRenderer != null && e.Action == MotionEventActions.Down) buttonRenderer.Control.Text = buttonRenderer.Element.Text;

            return false;
        }
    }

    public partial class CustomImageButtonRenderer
    {
        /// <summary>
        /// Returns the proper <see cref="IImageSourceHandler"/> based on the type of <see cref="ImageSource"/> provided.
        /// </summary>
        /// <param name="source">The <see cref="ImageSource"/> to get the handler for.</param>
        /// <returns>The needed handler.</returns>
        private static IImageSourceHandler GetHandler(ImageSource source)
        {
            IImageSourceHandler returnValue = null;
            if (source is UriImageSource)
            {
                returnValue = new ImageLoaderSourceHandler();
            }
            else if (source is FileImageSource)
            {
                returnValue = new FileImageSourceHandler();
            }
            else if (source is StreamImageSource)
            {
                returnValue = new StreamImagesourceHandler();
            }
            return returnValue;
        }

        /// <summary>
        /// Gets the width based on the requested width, if request less than 0, returns 50.
        /// </summary>
        /// <param name="requestedWidth">The requested width.</param>
        /// <returns>The width to use.</returns>
        private int GetWidth(int requestedWidth)
        {
            const int DefaultWidth = 50;
            return requestedWidth <= 0 ? DefaultWidth : requestedWidth;
        }

        /// <summary>
        /// Gets the height based on the requested height, if request less than 0, returns 50.
        /// </summary>
        /// <param name="requestedHeight">The requested height.</param>
        /// <returns>The height to use.</returns>
        private int GetHeight(int requestedHeight)
        {
            const int DefaultHeight = 50;
            return requestedHeight <= 0 ? DefaultHeight : requestedHeight;
        }
    }

    public interface ITypefaceCache
    {
        /// <summary>
        /// Removes typeface from cache
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="typeface">Typeface.</param>
        void StoreTypeface(string key, Typeface typeface);

        /// <summary>
        /// Removes the typeface.
        /// </summary>
        /// <param name="key">The key.</param>
        void RemoveTypeface(string key);

        /// <summary>
        /// Retrieves the typeface.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Typeface.</returns>
        Typeface RetrieveTypeface(string key);
    }

    /// <summary>
    /// TypefaceCache caches used typefaces for performance and memory reasons. 
    /// Typeface cache is singleton shared through execution of the application.
    /// You can replace default implementation of the cache by implementing ITypefaceCache 
    /// interface and setting instance of your cache to static property SharedCache of this class
    /// </summary>
    public static class TypefaceCache
    {
        private static ITypefaceCache sharedCache;

        /// <summary>
        /// Returns the shared typeface cache.
        /// </summary>
        /// <value>The shared cache.</value>
        public static ITypefaceCache SharedCache
        {
            get
            {
                if (sharedCache == null)
                {
                    sharedCache = new DefaultTypefaceCache();
                }
                return sharedCache;
            }
            set
            {
                if (sharedCache != null && sharedCache.GetType() == typeof(DefaultTypefaceCache))
                {
                    ((DefaultTypefaceCache)sharedCache).PurgeCache();
                }
                sharedCache = value;
            }
        }




    }

    /// <summary>
    /// Default implementation of the typeface cache.
    /// </summary>
    internal class DefaultTypefaceCache : ITypefaceCache
    {
        private Dictionary<string, Typeface> _cacheDict;

        public DefaultTypefaceCache()
        {
            _cacheDict = new Dictionary<string, Typeface>();
        }


        public Typeface RetrieveTypeface(string key)
        {
            if (_cacheDict.ContainsKey(key))
            {
                return _cacheDict[key];
            }
            else
            {
                return null;
            }
        }

        public void StoreTypeface(string key, Typeface typeface)
        {
            _cacheDict[key] = typeface;
        }

        public void RemoveTypeface(string key)
        {
            _cacheDict.Remove(key);
        }

        public void PurgeCache()
        {
            _cacheDict = new Dictionary<string, Typeface>();
        }
    }

}
