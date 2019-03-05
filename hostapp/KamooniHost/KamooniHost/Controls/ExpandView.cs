namespace Xamarin.Forms.Controls
{
    public class ExpandView : AbsoluteLayout
    {
        public static readonly BindableProperty HeaderTemplateProperty = BindableProperty.Create(nameof(HeaderTemplate), typeof(DataTemplate), typeof(ExpandView), default(DataTemplate));

        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public static readonly BindableProperty ContentTemplateProperty = BindableProperty.Create(nameof(ContentTemplate), typeof(DataTemplate), typeof(ExpandView), default(DataTemplate));

        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        public static readonly BindableProperty ExpandedProperty = BindableProperty.Create(nameof(Expanded), typeof(bool), typeof(ExpandView), default(bool), BindingMode.TwoWay, propertyChanged: ExpandedChanged);

        public bool Expanded
        {
            get { return (bool)GetValue(ExpandedProperty); }
            set { SetValue(ExpandedProperty, value); }
        }

        public ExpandView()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
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
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            View header = CreateHeader();
            if (header != null)
            {
                header.HorizontalOptions = LayoutOptions.FillAndExpand;

                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    Expanded = !Expanded;
                };
                header.GestureRecognizers.Add(tap);

                _container.Children.Add(header);
            }

            if (Expanded)
            {
                View content = CreateContent();
                if (content != null)
                {
                    content.HorizontalOptions = LayoutOptions.FillAndExpand;
                    content.VerticalOptions = LayoutOptions.Start;

                    _container.Children.Add(content);
                }
            }

            Children.Add(_container, new Rectangle());
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
                if (!(bindable is ExpandView control))
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
                    }
                    else
                    {
                        if (control._container.Children.Count > 1)
                        {
                            control._container.Children[1].IsVisible = expanded;
                        }
                    }
                }
            }
            catch { }
        }
    }
}