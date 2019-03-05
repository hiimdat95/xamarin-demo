using System;

namespace Xamarin.Forms.Controls
{
    public class RatingView : StackLayout
    {
        public static readonly BindableProperty StarSizeProperty = BindableProperty.Create(nameof(StarSize), typeof(double), typeof(RatingView), 50d, BindingMode.OneWay, propertyChanged: (b, o, n) => ((RatingView)b).OnStarSizeChanged(b, o, n));

        public static readonly BindableProperty MaxStarsProperty = BindableProperty.Create(nameof(MaxStars), typeof(int), typeof(RatingView), 5, BindingMode.OneWay, propertyChanged: (b, o, n) => ((RatingView)b).OnMaxStarsChanged(b, o, n));

        public static readonly BindableProperty RatingProperty = BindableProperty.Create(nameof(Rating), typeof(int), typeof(RatingView), 1, BindingMode.TwoWay, propertyChanged: (b, o, n) => ((RatingView)b).OnRatingChanged(b, o, n));

        public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(nameof(IsReadOnly), typeof(bool), typeof(RatingView), default, BindingMode.OneWay, propertyChanged: (b, o, n) => ((RatingView)b).OnIsReadOnlyChanged(b, o, n));

        public double StarSize
        {
            get => (double)GetValue(StarSizeProperty);
            set => SetValue(StarSizeProperty, value);
        }

        public int MaxStars
        {
            get => (int)GetValue(MaxStarsProperty);
            set => SetValue(MaxStarsProperty, value);
        }

        public int Rating
        {
            get => (int)GetValue(RatingProperty);
            set => SetValue(RatingProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public RatingView()
        {
            Orientation = StackOrientation.Horizontal;

            Init();
        }

        private void Init()
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;

            for (int i = 1; i <= MaxStars; i++)
            {
                Image star = new Image
                {
                    ClassId = i.ToString(),
                    HeightRequest = StarSize,
                    WidthRequest = StarSize,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                if (!IsReadOnly)
                    star.GestureRecognizers.Add(tapGestureRecognizer);
                Children.Add(star);
            }

            OnRatingChanged();
        }

        private void OnMaxStarsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue.Equals(newValue))
                return;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;

            Children.Clear();
            for (int i = 1; i <= MaxStars; i++)
            {
                Image star = new Image
                {
                    ClassId = i.ToString(),
                    HeightRequest = StarSize,
                    WidthRequest = StarSize,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                if (!IsReadOnly)
                    star.GestureRecognizers.Add(tapGestureRecognizer);
                Children.Add(star);
            }

            OnRatingChanged();
        }

        private void OnStarSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue.Equals(newValue))
                return;

            for (int i = 1; i <= Children.Count; i++)
            {
                if (Children[i - 1] is Image star)
                {
                    star.HeightRequest = StarSize;
                    star.WidthRequest = StarSize;
                }
            }
        }

        private void OnRatingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue.Equals(newValue))
                return;

            OnRatingChanged();
        }

        private void OnIsReadOnlyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue.Equals(newValue))
                return;

            InputTransparent = IsReadOnly;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (sender is Image tappedStar)
            {
                Rating = Convert.ToInt32(tappedStar.ClassId);
                OnRatingChanged();
            }
        }

        private void OnRatingChanged()
        {
            for (int i = 1; i <= Children.Count; i++)
            {
                if (Children[i - 1] is Image star)
                {
                    star.Source = i <= Rating ? "ic_star_rated_yellow_400" : "ic_star_unrated_yellow_400";
                }
            }
        }
    }
}
