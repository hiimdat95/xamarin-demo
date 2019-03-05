namespace Xamarin.Forms.Controls
{
    public class ExpandableView : Frame
    {
        public static readonly BindableProperty HeaderTemplateProperty = BindableProperty.Create(nameof(HeaderTemplate), typeof(DataTemplate), typeof(ExpandableView), default(DataTemplate));

        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public static readonly BindableProperty HeaderHeightProperty = BindableProperty.Create(nameof(HeaderHeight), typeof(double), typeof(ExpandableView), default(double));

        public double HeaderHeight
        {
            get => (double)GetValue(HeaderHeightProperty);
            set => SetValue(HeaderHeightProperty, value);
        }

        public static readonly BindableProperty ContentTemplateProperty = BindableProperty.Create(nameof(ContentTemplate), typeof(DataTemplate), typeof(ExpandableView), default(DataTemplate));

        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        public static readonly BindableProperty ContentHeightProperty = BindableProperty.Create(nameof(ContentHeight), typeof(double), typeof(ExpandableView), default(double));

        public double ContentHeight
        {
            get => (double)GetValue(ContentHeightProperty);
            set => SetValue(ContentHeightProperty, value);
        }

        public static readonly BindableProperty SpacingProperty = BindableProperty.Create(nameof(Spacing), typeof(double), typeof(ExpandableView), default(double));

        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(ExpandableView), default(bool), BindingMode.TwoWay, propertyChanged: ExpandedChanged);

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public ExpandableView()
        {
            IsClippedToBounds = true;
            CornerRadius = 5;
            Padding = 0;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            Init();
        }

        private void Init()
        {
            CreateView();
        }

        private StackLayout _container;

        private void CreateView()
        {
            _container = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = Spacing
            };

            View header = CreateHeader();
            if (header != null)
            {
                header.HorizontalOptions = LayoutOptions.FillAndExpand;

                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    IsExpanded = !IsExpanded;
                };
                header.GestureRecognizers.Add(tap);

                _container.Children.Add(header);
            }

            if (IsExpanded)
            {
                View content = CreateContent();
                if (content != null)
                {
                    content.HorizontalOptions = LayoutOptions.FillAndExpand;
                    content.VerticalOptions = LayoutOptions.Start;

                    _container.Children.Add(content);
                }
            }

            Content = _container;
        }

        protected virtual View CreateHeader()
        {
            View view = null;

            if (HeaderTemplate != null)
            {
                var content = HeaderTemplate.CreateContent();
                view = content is View ? content as View : ((ViewCell)content).View;
            }

            return view;
        }

        protected virtual View CreateContent()
        {
            View view = null;

            if (ContentTemplate != null)
            {
                var content = ContentTemplate.CreateContent();
                view = content is View ? content as View : ((ViewCell)content).View;
            }
            return view;
        }

        private static void ExpandedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                if (!(bindable is ExpandableView control))
                    return;

                if (newValue is bool expanded)
                {
                    if (expanded)
                    {
                        if (control._container.Children.Count > 1)
                        {
                            control._container.Children[1].IsVisible = expanded;
                        }
                        else
                        {
                            View content = control.CreateContent();
                            if (content != null)
                            {
                                content.HorizontalOptions = LayoutOptions.FillAndExpand;
                                content.VerticalOptions = LayoutOptions.Start;

                                control._container.Children.Add(content);
                            }
                        }

                        if (control.HeaderHeight > 0 && control.ContentHeight > 0)
                        {
                            control.HeightRequest = control.HeaderHeight + control.ContentHeight;
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (control._container.Children.Count > 1)
                        {
                            control._container.Children[1].IsVisible = expanded;
                        }

                        if (control.HeaderHeight > 0)
                        {
                            control.HeightRequest = control.HeaderHeight;
                        }
                        else
                        {

                        }
                    }
                }
            }
            catch { }
        }
    }
}