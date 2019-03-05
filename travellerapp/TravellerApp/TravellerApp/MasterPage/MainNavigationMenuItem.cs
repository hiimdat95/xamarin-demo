using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace TravellerApp
{
    public class MainNavigationMenuItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; }

        public string Title { get; set; }

        public string IconPath { get; set; }

        public Color BackgroundColor { get; set; }

        private Color _textColor;

        public Color TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                _textColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TextColor"));
            }
        }

        public Type TargetType { get; set; }

        public object[] Params { get; set; }

        public MainNavigationMenuItem()
        {
            TextColor = Color.White;
            BackgroundColor = (Color)Application.Current.Resources["Color_Background"];
        }
    }
}