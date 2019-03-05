using CarouselView.FormsPlugin.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Converters;

namespace Xamarin.Forms.Controls
{
    public delegate void PositionChangingEventHandler(object sender, PositionChangingEventArgs e);

    public delegate void PositionChangedEventHandler(object sender, PositionChangedEventArgs e);

    public class PositionChangingEventArgs : EventArgs
    {
        public bool Canceled { get; set; }
        public int NewPosition { get; set; }
        public int OldPosition { get; set; }
    }

    public class PositionChangedEventArgs : EventArgs
    {
        public int NewPosition { get; set; }
        public int OldPosition { get; set; }
    }

    internal static class TabDefaults
    {
        public static readonly Color DefaultColor = Color.White;
        public const double DefaultThickness = 5;
        public const double DefaultTextSize = 14;
    }

    public class TabbedView : ContentView
    {
        #region ContentHeight

        public static readonly BindableProperty ContentHeightProperty = BindableProperty.Create(nameof(ContentHeight), typeof(double), typeof(TabbedView), (double)200, BindingMode.Default, null, ContentHeightChanged);

        public double ContentHeight
        {
            get { return (double)GetValue(ContentHeightProperty); }
            set { SetValue(ContentHeightProperty, value); }
        }

        private static void ContentHeightChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabbedView tabViewControl && tabViewControl._carouselView != null)
            {
                tabViewControl._carouselView.HeightRequest = (double)newValue;
            }
        }

        #endregion ContentHeight

        #region HeaderBackgroundColor

        public static readonly BindableProperty HeaderBackgroundColorProperty = BindableProperty.Create(nameof(HeaderBackgroundColor), typeof(Color), typeof(TabbedView), Color.SkyBlue, BindingMode.Default, null, HeaderBackgroundColorChanged);

        public Color HeaderBackgroundColor
        {
            get { return (Color)GetValue(HeaderBackgroundColorProperty); }
            set { SetValue(HeaderBackgroundColorProperty, value); }
        }

        private static void HeaderBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabbedView tabViewControl)
            {
                tabViewControl._headerContainer.BackgroundColor = (Color)newValue;
            }
        }

        #endregion HeaderBackgroundColor

        #region HeaderTabTextColor

        public static readonly BindableProperty HeaderTabTextColorProperty = BindableProperty.Create(nameof(HeaderTabTextColor), typeof(Color), typeof(TabbedView), TabDefaults.DefaultColor, BindingMode.OneWay, null, HeaderTabTextColorChanged);

        public Color HeaderTabTextColor
        {
            get { return (Color)GetValue(HeaderTabTextColorProperty); }
            set { SetValue(HeaderTabTextColorProperty, value); }
        }

        private static void HeaderTabTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabbedView tabViewControl && tabViewControl.ItemsSource != null)
            {
                foreach (var tab in tabViewControl.ItemsSource)
                {
                    tab.HeaderTextColor = (Color)newValue;
                }
            }
        }

        #endregion HeaderTabTextColor

        #region HeaderSelectionUnderlineColor

        public static readonly BindableProperty HeaderSelectionUnderlineColorProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineColor), typeof(Color), typeof(TabbedView), Color.White, BindingMode.Default, null, HeaderSelectionUnderlineColorChanged);

        public Color HeaderSelectionUnderlineColor
        {
            get { return (Color)GetValue(HeaderSelectionUnderlineColorProperty); }
            set { SetValue(HeaderSelectionUnderlineColorProperty, value); }
        }

        private static void HeaderSelectionUnderlineColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabbedView tabViewControl && tabViewControl.ItemsSource != null)
            {
                foreach (var tab in tabViewControl.ItemsSource)
                {
                    tab.HeaderSelectionUnderlineColor = (Color)newValue;
                }
            }
        }

        #endregion HeaderSelectionUnderlineColor

        #region HeaderSelectionUnderlineThickness

        public static readonly BindableProperty HeaderSelectionUnderlineThicknessProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineThickness), typeof(double), typeof(TabbedView), TabDefaults.DefaultThickness, BindingMode.Default, null, HeaderSelectionUnderlineThicknessChanged);

        public double HeaderSelectionUnderlineThickness
        {
            get { return (double)GetValue(HeaderSelectionUnderlineThicknessProperty); }
            set { SetValue(HeaderSelectionUnderlineThicknessProperty, value); }
        }

        private static void HeaderSelectionUnderlineThicknessChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabbedView tabViewControl && tabViewControl.ItemsSource != null)
            {
                foreach (var tab in tabViewControl.ItemsSource)
                {
                    tab.HeaderSelectionUnderlineThickness = (double)newValue;
                }
            }
        }

        #endregion HeaderSelectionUnderlineThickness

        #region HeaderSelectionUnderlineWidth

        public static readonly BindableProperty HeaderSelectionUnderlineWidthProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineWidth), typeof(double), typeof(TabbedView), (double)0, BindingMode.Default, null, HeaderSelectionUnderlineWidthChanged);

        public double HeaderSelectionUnderlineWidth
        {
            get { return (double)GetValue(HeaderSelectionUnderlineWidthProperty); }
            set { SetValue(HeaderSelectionUnderlineWidthProperty, value); }
        }

        private static void HeaderSelectionUnderlineWidthChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabbedView tabViewControl && tabViewControl.ItemsSource != null)
            {
                foreach (var tab in tabViewControl.ItemsSource)
                {
                    tab.HeaderSelectionUnderlineWidth = (double)newValue;
                }
            }
        }

        #endregion HeaderSelectionUnderlineWidth

        #region HeaderTabTextFontSize

        public static readonly BindableProperty HeaderTabTextFontSizeProperty = BindableProperty.Create(nameof(HeaderTabTextFontSize), typeof(double), typeof(TabbedView), (double)14, BindingMode.Default, null, HeaderTabTextFontSizeChanged);

        [TypeConverter(typeof(FontSizeConverter))]
        public double HeaderTabTextFontSize
        {
            get { return (double)GetValue(HeaderTabTextFontSizeProperty); }
            set { SetValue(HeaderTabTextFontSizeProperty, value); }
        }

        private static void HeaderTabTextFontSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabbedView tabViewControl && tabViewControl.ItemsSource != null)
            {
                foreach (var tab in tabViewControl.ItemsSource)
                {
                    tab.HeaderTabTextFontSize = (double)newValue;
                }
            }
        }

        #endregion HeaderTabTextFontSize

        #region HeaderTabTextFontFamily

        public static readonly BindableProperty HeaderTabTextFontFamilyProperty = BindableProperty.Create(nameof(HeaderTabTextFontFamily), typeof(string), typeof(TabbedView), null, BindingMode.Default, null, HeaderTabTextFontFamilyChanged);

        public string HeaderTabTextFontFamily
        {
            get { return (string)GetValue(HeaderTabTextFontFamilyProperty); }
            set { SetValue(HeaderTabTextFontFamilyProperty, value); }
        }

        private static void HeaderTabTextFontFamilyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabbedView tabViewControl && tabViewControl.ItemsSource != null)
            {
                foreach (var tab in tabViewControl.ItemsSource)
                {
                    tab.HeaderTabTextFontFamily = (string)newValue;
                }
            }
        }

        #endregion HeaderTabTextFontFamily

        #region HeaderTabTextFontAttributes

        public static readonly BindableProperty HeaderTabTextFontAttributesProperty = BindableProperty.Create(nameof(HeaderTabTextFontAttributes), typeof(FontAttributes), typeof(TabbedView), FontAttributes.None, BindingMode.Default, null, HeaderTabTextFontAttributesChanged);

        public FontAttributes HeaderTabTextFontAttributes
        {
            get { return (FontAttributes)GetValue(HeaderTabTextFontAttributesProperty); }
            set { SetValue(HeaderTabTextFontAttributesProperty, value); }
        }

        private static void HeaderTabTextFontAttributesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabbedView tabViewControl && tabViewControl.ItemsSource != null)
            {
                foreach (var tab in tabViewControl.ItemsSource)
                {
                    tab.HeaderTabTextFontAttributes = (FontAttributes)newValue;
                }
            }
        }

        #endregion HeaderTabTextFontAttributes

        #region ItemSource

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IList<TabbedItem>), typeof(TabbedView), default(IList), BindingMode.OneWay, propertyChanged: ItemsSourceChanged);

        public IList<TabbedItem> ItemsSource
        {
            get => (IList<TabbedItem>)GetValue(ItemsSourceProperty);
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabbedView tabViewControl && tabViewControl.ItemsSource != null)
            {
                if (tabViewControl.ItemsSource.Count > 0)
                {
                    tabViewControl.Init();

                    foreach (var tab in tabViewControl.ItemsSource)
                    {
                        tabViewControl.SetTabProps(tab);
                        tabViewControl.AddTabToView(tab);
                    }
                }

                if (tabViewControl.ItemsSource is INotifyCollectionChanged oldCollection)
                {
                    oldCollection.CollectionChanged -= tabViewControl.ItemSource_CollectionChanged;
                }

                if (tabViewControl.ItemsSource is INotifyCollectionChanged newCollection)
                {
                    newCollection.CollectionChanged += tabViewControl.ItemSource_CollectionChanged;
                }
            }
        }

        #endregion ItemSource

        #region TabSizeOption

        public static readonly BindableProperty TabSizeOptionProperty = BindableProperty.Create(nameof(TabSizeOption), typeof(GridLength), typeof(TabbedView), default(GridLength), propertyChanged: OnTabSizeOptionChanged);

        public GridLength TabSizeOption
        {
            get => (GridLength)GetValue(TabSizeOptionProperty);
            set { SetValue(TabSizeOptionProperty, value); }
        }

        private static void OnTabSizeOptionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabbedView tabViewControl && tabViewControl._headerContainer != null && tabViewControl.ItemsSource != null)
            {
                foreach (var tabContainer in tabViewControl._headerContainer.ColumnDefinitions)
                {
                    tabContainer.Width = (GridLength)newValue;
                }
            }
        }

        #endregion TabSizeOption

        #region SelectedTabIndex

        public static readonly BindableProperty SelectedTabIndexProperty = BindableProperty.Create(nameof(SelectedTabIndex), typeof(int), typeof(TabbedView), 0, BindingMode.TwoWay, propertyChanged: OnSelectedTabIndexChanged);

        public int SelectedTabIndex
        {
            get => (int)GetValue(SelectedTabIndexProperty);
            set { SetValue(SelectedTabIndexProperty, value); }
        }

        private static void OnSelectedTabIndexChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabbedView tabViewControl && tabViewControl.ItemsSource != null && tabViewControl._carouselView.Position != (int)newValue)
            {
                tabViewControl.SetPosition((int)newValue);
            }
        }

        #endregion SelectedTabIndex

        #region ContentTemplate

        public static readonly BindableProperty ContentTemplateProperty = BindableProperty.Create(nameof(ContentTemplate), typeof(DataTemplate), typeof(ExpandableView), default(DataTemplate));

        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        #endregion ContentTemplate

        private StackLayout _mainContainer;
        private ScrollView _header;
        private Grid _headerContainer;
        private CarouselViewControl _carouselView;

        public event PositionChangingEventHandler PositionChanging;

        public event PositionChangedEventHandler PositionChanged;

        protected virtual void OnPositionChanging(ref PositionChangingEventArgs e)
        {
            PositionChanging?.Invoke(this, e);
        }

        protected virtual void OnPositionChanged(PositionChangedEventArgs e)
        {
            PositionChanged?.Invoke(this, e);
        }

        public TabbedView()
        {
            ItemsSource = new ObservableCollection<TabbedItem>();

            //Parameterless constructor required for xaml instantiation.
            Init();
        }

        public TabbedView(IList<TabbedItem> tabItems, int selectedTabIndex = 0)
        {
            ItemsSource = new ObservableCollection<TabbedItem>();

            Init();
            
            foreach (var tab in tabItems)
            {
                ItemsSource.Add(tab);
            }

            if (selectedTabIndex > 0)
            {
                SelectedTabIndex = selectedTabIndex;
            }
        }

        private void Init()
        {
            _headerContainer = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                BackgroundColor = HeaderBackgroundColor,
                MinimumHeightRequest = 50
            };

            _header = new ScrollView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                Orientation = ScrollOrientation.Horizontal,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                Content = _headerContainer
            };

            _carouselView = new CarouselViewControl
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = ContentHeight,
                ShowArrows = false,
                ShowIndicators = false,
                BindingContext = this
            };

            _carouselView.PropertyChanged += _carouselView_PropertyChanged;

            _mainContainer = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = { _header, _carouselView },
                Spacing = 0
            };

            Content = _mainContainer;

            SetPosition(SelectedTabIndex, true);
        }

        private void ItemSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Init();
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    foreach (var tab in e.NewItems)
                    {
                        if (tab is TabbedItem newTab)
                        {
                            SetTabProps(newTab);
                            AddTabToView(newTab);
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                RemoveTab(e.OldStartingIndex);
            }
        }

        //Set the tab properties from the main control only if not set in xaml at the individual tab level.
        private void SetTabProps(TabbedItem tab)
        {
            if (tab.HeaderTextColor == TabDefaults.DefaultColor && HeaderTabTextColor != TabDefaults.DefaultColor)
                tab.HeaderTextColor = HeaderTabTextColor;

            if (tab.HeaderSelectionUnderlineColor == TabDefaults.DefaultColor && HeaderSelectionUnderlineColor != TabDefaults.DefaultColor)
                tab.HeaderSelectionUnderlineColor = HeaderSelectionUnderlineColor;

            if (tab.HeaderSelectionUnderlineThickness.Equals(TabDefaults.DefaultThickness) && !HeaderSelectionUnderlineThickness.Equals(TabDefaults.DefaultThickness))
                tab.HeaderSelectionUnderlineThickness = HeaderSelectionUnderlineThickness;

            if (tab.HeaderSelectionUnderlineWidth > 0)
                tab.HeaderSelectionUnderlineWidth = HeaderSelectionUnderlineWidth;

            if (tab.HeaderTabTextFontSize.Equals(TabDefaults.DefaultTextSize) && !HeaderTabTextFontSize.Equals(TabDefaults.DefaultTextSize))
                tab.HeaderTabTextFontSize = HeaderTabTextFontSize;

            if (tab.HeaderTabTextFontFamily is null && !string.IsNullOrWhiteSpace(HeaderTabTextFontFamily))
                tab.HeaderTabTextFontFamily = HeaderTabTextFontFamily;

            if (tab.HeaderTabTextFontAttributes == FontAttributes.None && HeaderTabTextFontAttributes != FontAttributes.None)
                tab.HeaderTabTextFontAttributes = HeaderTabTextFontAttributes;

            if (tab.Content == null && ContentTemplate != null)
            {
                View view = ContentTemplate.CreateContent() as View;
                if (view != null && tab.BindingContext != null)
                    view.BindingContext = tab.BindingContext;
                tab.Content = view;
            }
        }

        private bool _supressCarouselViewPositionChangedEvent;

        private void _carouselView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_carouselView.Position) && !_supressCarouselViewPositionChangedEvent)
            {
                var positionChangingArgs = new PositionChangingEventArgs()
                {
                    Canceled = false,
                    NewPosition = _carouselView.Position,
                    OldPosition = SelectedTabIndex
                };

                OnPositionChanging(ref positionChangingArgs);

                if (positionChangingArgs != null && positionChangingArgs.Canceled)
                {
                    _supressCarouselViewPositionChangedEvent = true;
                    _carouselView.PositionSelected -= _carouselView_PositionSelected;
                    _carouselView.PropertyChanged -= _carouselView_PropertyChanged;
                    _carouselView.Position = SelectedTabIndex;
                    _carouselView.PositionSelected += _carouselView_PositionSelected;
                    _carouselView.PropertyChanged += _carouselView_PropertyChanged;
                    _supressCarouselViewPositionChangedEvent = false;
                }
            }
        }

        private void _carouselView_PositionSelected(object sender, PositionSelectedEventArgs e)
        {
            if (ItemsSource != null && (_carouselView.Position != e.NewValue || SelectedTabIndex != e.NewValue))
            {
                SetPosition(e.NewValue);
                
                _header.ScrollToAsync(_headerContainer.Children.ElementAt(e.NewValue), ScrollToPosition.Center, true);
            }
        }

        private void AddTabToView(TabbedItem tab)
        {
            var tabSize = (TabSizeOption.IsAbsolute && TabSizeOption.Value.Equals(0)) ? new GridLength(1, GridUnitType.Star) : TabSizeOption;

            _headerContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = tabSize });

            tab.IsCurrent = _headerContainer.ColumnDefinitions.Count - 1 == SelectedTabIndex;

            var headerIcon = new Image
            {
                Margin = new Thickness(0, 10, 0, 0),
                BindingContext = tab,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = tab.HeaderIconSize,
                HeightRequest = tab.HeaderIconSize
            };
            headerIcon.SetBinding(Image.SourceProperty, nameof(TabbedItem.HeaderIcon));
            headerIcon.SetBinding(IsVisibleProperty, nameof(TabbedItem.HeaderIcon), converter: new NullToBoolConverter());

            var headerLabel = new Label
            {
                Margin = new Thickness(5, headerIcon.IsVisible ? 0 : 10, 5, 0),
                BindingContext = tab,
                VerticalTextAlignment = TextAlignment.Start,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center
            };
            headerLabel.SetBinding(Label.TextProperty, nameof(TabbedItem.HeaderText));
            headerLabel.SetBinding(Label.TextColorProperty, nameof(TabbedItem.HeaderTextColor));
            headerLabel.SetBinding(Label.FontSizeProperty, nameof(TabbedItem.HeaderTabTextFontSize));
            headerLabel.SetBinding(Label.FontFamilyProperty, nameof(TabbedItem.HeaderTabTextFontFamily));
            headerLabel.SetBinding(Label.FontAttributesProperty, nameof(TabbedItem.HeaderTabTextFontAttributes));
            headerLabel.SetBinding(IsVisibleProperty, nameof(TabbedItem.HeaderText), converter: new NullToBoolConverter());

            var selectionBarBoxView = new BoxView
            {
                VerticalOptions = LayoutOptions.EndAndExpand,
                BindingContext = tab,
                HeightRequest = HeaderSelectionUnderlineThickness,
                WidthRequest = HeaderSelectionUnderlineWidth
            };
            selectionBarBoxView.SetBinding(IsVisibleProperty, nameof(TabbedItem.IsCurrent));
            selectionBarBoxView.SetBinding(BoxView.ColorProperty, nameof(TabbedItem.HeaderSelectionUnderlineColor));
            selectionBarBoxView.SetBinding(WidthRequestProperty, nameof(TabbedItem.HeaderSelectionUnderlineWidth));
            selectionBarBoxView.SetBinding(HeightRequestProperty, nameof(TabbedItem.HeaderSelectionUnderlineThickness));
            selectionBarBoxView.SetBinding(HorizontalOptionsProperty, nameof(TabbedItem.HeaderSelectionUnderlineWidthProperty), converter: new DoubleToLayoutOptionsConverter());

            selectionBarBoxView.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == nameof(TabbedItem.IsCurrent))
                {
                    SetPosition(ItemsSource.IndexOf((TabbedItem)((BoxView)sender).BindingContext));
                }
                if (e.PropertyName == nameof(WidthRequest))
                {
                    selectionBarBoxView.HorizontalOptions = tab.HeaderSelectionUnderlineWidth > 0 ? LayoutOptions.CenterAndExpand : LayoutOptions.FillAndExpand;
                }
            };

            var headerItem = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = { headerIcon, headerLabel, selectionBarBoxView }
            };
            var tapRecognizer = new TapGestureRecognizer();
            tapRecognizer.Tapped += (object s, EventArgs e) =>
            {
                _supressCarouselViewPositionChangedEvent = true;
                SetPosition(_headerContainer.Children.IndexOf((View)s));
                _header.ScrollToAsync((View)s, ScrollToPosition.Center, true);
                _supressCarouselViewPositionChangedEvent = false;
            };
            headerItem.GestureRecognizers.Add(tapRecognizer);

            _headerContainer.Children.Add(headerItem, _headerContainer.ColumnDefinitions.Count - 1, 0);

            _carouselView.ItemsSource = ItemsSource.Select(t => t.Content);
        }

        public void SetPosition(int position, bool initialRun = false)
        {
            if (SelectedTabIndex == position && !initialRun)
            {
                return;
            }

            int oldPosition = SelectedTabIndex;

            var positionChangingArgs = new PositionChangingEventArgs()
            {
                Canceled = false,
                NewPosition = position,
                OldPosition = oldPosition
            };
            OnPositionChanging(ref positionChangingArgs);

            if (positionChangingArgs != null && positionChangingArgs.Canceled)
            {
                return;
            }

            if ((position >= 0 && position < ItemsSource.Count) || initialRun)
            {
                if (_carouselView.Position != position || initialRun)
                {
                    _carouselView.PositionSelected -= _carouselView_PositionSelected;
                    _carouselView.Position = position;
                    _carouselView.PositionSelected += _carouselView_PositionSelected;
                }
                if (oldPosition != position)
                {
                    if (oldPosition < ItemsSource.Count)
                    {
                        ItemsSource[oldPosition].IsCurrent = false;
                    }
                    ItemsSource[position].IsCurrent = true;
                    SelectedTabIndex = position;
                }
            }

            var positionChangedArgs = new PositionChangedEventArgs()
            {
                NewPosition = SelectedTabIndex,
                OldPosition = oldPosition
            };
            OnPositionChanged(positionChangedArgs);
        }

        public void SelectNext()
        {
            if (ItemsSource != null)
            {
                SetPosition(SelectedTabIndex + 1);
            }
        }

        public void SelectPrevious()
        {
            if (ItemsSource != null)
            {
                SetPosition(SelectedTabIndex - 1);
            }
        }

        public void SelectFirst()
        {
            if (ItemsSource != null)
            {
                SetPosition(0);
            }
        }

        public void SelectLast()
        {
            if (ItemsSource != null)
            {
                SetPosition(ItemsSource.Count - 1);
            }
        }

        public void AddTab(TabbedItem tab, int position = -1, bool selectNewPosition = false)
        {
            if (ItemsSource == null)
            {
                return;
            }

            if (position > -1)
            {
                ItemsSource.Insert(position, tab);
            }
            else
            {
                ItemsSource.Add(tab);
            }
            if (selectNewPosition)
            {
                SelectedTabIndex = position;
            }
        }

        public void RemoveTab(int position = -1)
        {
            if (ItemsSource == null)
            {
                return;
            }

            if (position > -1)
            {
                ItemsSource.RemoveAt(position);
                _headerContainer.Children.RemoveAt(position);
                _headerContainer.ColumnDefinitions.RemoveAt(position);
            }
            else
            {
                ItemsSource.Remove(ItemsSource.Last());
                _headerContainer.Children.RemoveAt(_headerContainer.Children.Count - 1);
                _headerContainer.ColumnDefinitions.Remove(_headerContainer.ColumnDefinitions.Last());
            }
            _carouselView.ItemsSource = ItemsSource.Select(t => t.Content);
            SelectedTabIndex = position >= 0 && position < ItemsSource.Count ? position : ItemsSource.Count - 1;
        }
    }
}