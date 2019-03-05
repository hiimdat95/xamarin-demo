using System;
using TravellerApp.Controls;
using UIKit;
using Xamarin.Forms;
using TravellerApp.iOS.Renderers;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;

[assembly: ExportRenderer(typeof(CustomRatingBar), typeof(RatingBarRenderer))]

namespace TravellerApp.iOS.Renderers
{
    class RatingBarRenderer:ViewRenderer<CustomRatingBar, UIView>
    {
        const string IconStarBlank = "ic_star_blank.png";
        const string IconStarYellow = "ic_star_yellow.png";
        const string IconStarGreen = "ic_star_green.png";
        const string IconStarOrange = "ic_star_orange.png";
        const string IconStarRed = "ic_star_red.png";
 
        UIView rateView;
        UIButton pointFiveStar, oneStar, onePointFiveStar, twoStar, twoPointFiveStar, threeStar, threePointFiveStar, fourStar, fourPointFiveStar, fiveStar;
        float starSize;
        CustomRatingBar element;


        protected override void OnElementChanged(ElementChangedEventArgs<CustomRatingBar> e)
        {
            base.OnElementChanged(e);
            element = Element as CustomRatingBar;
            if (element == null)
                return;
            InitializeButton();
            SetTouchEvents(element.IsReadonly);

            rateView = new UIView()
            {
                
            };
            if (element.IsSmallStyle)
            {
                starSize = 15;
                rateView.Frame = new CGRect(0, 0, 95, 15);
                SetRatingBarSmall();
                AddStarInView();
                if (element.Rating >= 0)
                    ShowRatingBar();
            }
            else
            {
                starSize = 30;
                rateView.Frame = new CGRect(0, 0, 170, 30);
                SetRatingBarDefault();
                AddStarInView();
                ShowRatingBar();
            }
            SetNativeControl(rateView);
            var tapGesture = new UITapGestureRecognizer(OnRateViewTapped);
            rateView.AddGestureRecognizer(tapGesture);
        }

        private void OnRateViewTapped()
        {
            element.OnTapped();
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName.Equals(CustomRatingBar.RatingProperty.PropertyName))
            {
                ShowRatingBar();
            }
            else if (e.PropertyName.Equals(CustomRatingBar.IsReadonlyProperty.PropertyName))
            {
                SetTouchEvents(element.IsReadonly);
            }
        }

        // rating bar bind with value;
        private void ShowRatingBar()
        {
            if (Element.Rating <= 0)
                SetBlankStarRating();
            else if (Element.Rating <= 0.5)
                SetPointFiveStarRating();
            else if (Element.Rating <= 1)
                SetOneStarRating();
            else if (Element.Rating <= 1.5)
                SetOnePointFiveStar();
            else if (Element.Rating <= 2)
                SetTwoStarRating();
            else if (Element.Rating <= 2.5)
                SetTwoPointFiveStar();
            else if (Element.Rating <= 3)
                SetThreeStarRating();
            else if (Element.Rating <= 3.5)
                SetThreePointFiveStar();
            else if (Element.Rating <= 4)
                SetFourStarRating();
            else if (Element.Rating <= 4.5)
                SetFourPointFiveStar();
            else if (Element.Rating <= 5)
                SetFiveStarRating();
        }

        // button initialize
        private void InitializeButton()
        {
            pointFiveStar = UIButton.FromType(UIButtonType.Custom);
            pointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal); 

            oneStar = UIButton.FromType(UIButtonType.Custom);
            oneStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);

            onePointFiveStar = UIButton.FromType(UIButtonType.Custom);
            onePointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);

            twoStar = UIButton.FromType(UIButtonType.Custom);
            twoStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);

            twoPointFiveStar = UIButton.FromType(UIButtonType.Custom);
            twoPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);

            threeStar = UIButton.FromType(UIButtonType.Custom);
            threeStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);

            threePointFiveStar = UIButton.FromType(UIButtonType.Custom);
            threePointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);

            fourStar = UIButton.FromType(UIButtonType.Custom);
            fourStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);

            fourPointFiveStar = UIButton.FromType(UIButtonType.Custom);
            fourPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);

            fiveStar = UIButton.FromType(UIButtonType.Custom);
            fiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
        }

        void pointFiveStar_TouchUpInside(object sender, EventArgs e)
        {
            element.OnRatingChanged(0.5f);
        }
        void OneStar_TouchUpInside(object sender, EventArgs e)
        {
            element.OnRatingChanged(1);
        }
        void onePointFiveStar_TouchUpInside(object sender, EventArgs e)
        {
            element.OnRatingChanged(1.5f);
        }
        void TwoStar_TouchUpInside(object sender, EventArgs e)
        {
            element.OnRatingChanged(2);
        }
        void TwoPointFiveStar_TouchUpInside(object sender, EventArgs e)
        {
            element.OnRatingChanged(2.5f);
        }
        void ThreeStar_TouchUpInside(object sender, EventArgs e)
        {
            element.OnRatingChanged(3);
        }
        void ThreePointFiveStar_TouchUpInside(object sender, EventArgs e)
        {
            element.OnRatingChanged(3.5f);
        }
        void FourStar_TouchUpInside(object sender, EventArgs e)
        {
            element.OnRatingChanged(4);
        }
        void FourPointFiveStar_TouchUpInside(object sender, EventArgs e)
        {
            element.OnRatingChanged(4.5f);
        }
        void FiveStar_TouchUpInside(object sender, EventArgs e)
        {
            element.OnRatingChanged(5);
        }

        private void SetTouchEvents(bool isReadOnly)
        {
            pointFiveStar.TouchUpInside -= pointFiveStar_TouchUpInside;
            oneStar.TouchUpInside -= OneStar_TouchUpInside;
            onePointFiveStar.TouchUpInside -= onePointFiveStar_TouchUpInside;
            twoStar.TouchUpInside -= TwoStar_TouchUpInside;
            twoPointFiveStar.TouchUpInside -= TwoPointFiveStar_TouchUpInside;
            threeStar.TouchUpInside -= ThreeStar_TouchUpInside;
            threePointFiveStar.TouchUpInside -= ThreePointFiveStar_TouchUpInside;
            fourStar.TouchUpInside -= FourStar_TouchUpInside;
            fourPointFiveStar.TouchUpInside -= FourPointFiveStar_TouchUpInside;
            fiveStar.TouchUpInside -= FiveStar_TouchUpInside;
            if (isReadOnly == false)
            {
                pointFiveStar.TouchUpInside += pointFiveStar_TouchUpInside;
                oneStar.TouchUpInside += OneStar_TouchUpInside;
                onePointFiveStar.TouchUpInside += onePointFiveStar_TouchUpInside;
                twoStar.TouchUpInside += TwoStar_TouchUpInside;
                twoPointFiveStar.TouchUpInside += TwoPointFiveStar_TouchUpInside;
                threeStar.TouchUpInside += ThreeStar_TouchUpInside;
                threePointFiveStar.TouchUpInside += ThreePointFiveStar_TouchUpInside;
                fourStar.TouchUpInside += FourStar_TouchUpInside;
                fourPointFiveStar.TouchUpInside += FourPointFiveStar_TouchUpInside;
                fiveStar.TouchUpInside += FiveStar_TouchUpInside;   
            }
        }


        // rating bar size
        private void SetRatingBarSmall()
        {
            //int x = (int)(App.ScreenSize.Width - ratingBarWidth) / 2;
            int x = 0; 
            oneStar.Frame = new CGRect(x, 0, starSize, starSize);
            twoStar.Frame = new CGRect(x = x + 20, 0, starSize, starSize);
            threeStar.Frame = new CGRect(x = x + 20, 0, starSize, starSize);
            fourStar.Frame = new CGRect(x = x + 20, 0, starSize, starSize);
            fiveStar.Frame = new CGRect(x = x + 20, 0, starSize, starSize);
        }

        private void SetRatingBarDefault()
        {
            //int x = (int)(App.ScreenSize.Width - ratingBarWidth) / 2;
            int x = 0;
            oneStar.Frame = new CGRect(x, 0, starSize, starSize);
            twoStar.Frame = new CGRect(x = x + 35, 0, starSize, starSize);
            threeStar.Frame = new CGRect(x = x + 35, 0, starSize, starSize);
            fourStar.Frame = new CGRect(x = x + 35, 0, starSize, starSize);
            fiveStar.Frame = new CGRect(x = x + 35, 0, starSize, starSize);
        }

        private void AddStarInView()
        {
            rateView.AddSubview(pointFiveStar);
            rateView.AddSubview(oneStar);
            rateView.AddSubview(onePointFiveStar);
            rateView.AddSubview(twoStar);
            rateView.AddSubview(twoPointFiveStar);
            rateView.AddSubview(threeStar);
            rateView.AddSubview(threePointFiveStar);
            rateView.AddSubview(fourStar);
            rateView.AddSubview(fourPointFiveStar);
            rateView.AddSubview(fiveStar);
        }

        private void SetBlankStarRating()
        {
            pointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            oneStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            onePointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            twoStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            twoPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            threeStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            threePointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fourStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fourPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
        }

        private void SetPointFiveStarRating()
        {
            pointFiveStar.SetImage(UIImage.FromFile(IconStarRed), UIControlState.Normal);
            oneStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            onePointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            twoStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            twoPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            threeStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            threePointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fourStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fourPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
        }

        private void SetOneStarRating()
        {
            pointFiveStar.SetImage(UIImage.FromFile(IconStarRed), UIControlState.Normal);
            oneStar.SetImage(UIImage.FromFile(IconStarRed), UIControlState.Normal);
            onePointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            twoStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            twoPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            threeStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            threePointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fourStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fourPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
        }

        private void SetOnePointFiveStar()
        {
            pointFiveStar.SetImage(UIImage.FromFile(IconStarRed), UIControlState.Normal);
            oneStar.SetImage(UIImage.FromFile(IconStarRed), UIControlState.Normal);
            onePointFiveStar.SetImage(UIImage.FromFile(IconStarRed), UIControlState.Normal);
            twoStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            twoPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            threeStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            threePointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fourStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fourPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
        }

        private void SetTwoStarRating()
        {
            pointFiveStar.SetImage(UIImage.FromFile(IconStarRed), UIControlState.Normal);
            oneStar.SetImage(UIImage.FromFile(IconStarRed), UIControlState.Normal);
            onePointFiveStar.SetImage(UIImage.FromFile(IconStarRed), UIControlState.Normal);
            twoStar.SetImage(UIImage.FromFile(IconStarRed), UIControlState.Normal);
            twoPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            threeStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            threePointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fourStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fourPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
        }

        private void SetTwoPointFiveStar()
        {
            pointFiveStar.SetImage(UIImage.FromFile(IconStarRed), UIControlState.Normal);
            oneStar.SetImage(UIImage.FromFile(IconStarRed), UIControlState.Normal);
            onePointFiveStar.SetImage(UIImage.FromFile(IconStarRed), UIControlState.Normal);
            twoStar.SetImage(UIImage.FromFile(IconStarRed), UIControlState.Normal);
            twoPointFiveStar.SetImage(UIImage.FromFile(IconStarRed), UIControlState.Normal);
            threeStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            threePointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fourStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fourPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
        }

        private void SetThreeStarRating()
        {
            pointFiveStar.SetImage(UIImage.FromFile(IconStarOrange), UIControlState.Normal);
            oneStar.SetImage(UIImage.FromFile(IconStarOrange), UIControlState.Normal);
            onePointFiveStar.SetImage(UIImage.FromFile(IconStarOrange), UIControlState.Normal);
            twoStar.SetImage(UIImage.FromFile(IconStarOrange), UIControlState.Normal);
            twoPointFiveStar.SetImage(UIImage.FromFile(IconStarOrange), UIControlState.Normal);
            threeStar.SetImage(UIImage.FromFile(IconStarOrange), UIControlState.Normal);
            threePointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fourStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fourPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
        }

        private void SetThreePointFiveStar()
        {
            pointFiveStar.SetImage(UIImage.FromFile(IconStarOrange), UIControlState.Normal);
            oneStar.SetImage(UIImage.FromFile(IconStarOrange), UIControlState.Normal);
            onePointFiveStar.SetImage(UIImage.FromFile(IconStarOrange), UIControlState.Normal);
            twoStar.SetImage(UIImage.FromFile(IconStarOrange), UIControlState.Normal);
            twoPointFiveStar.SetImage(UIImage.FromFile(IconStarOrange), UIControlState.Normal);
            threeStar.SetImage(UIImage.FromFile(IconStarOrange), UIControlState.Normal);
            threePointFiveStar.SetImage(UIImage.FromFile(IconStarOrange), UIControlState.Normal);
            fourStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fourPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
        }

        private void SetFourStarRating()
        {
            pointFiveStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            oneStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            onePointFiveStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            twoStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            twoPointFiveStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            threeStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            threePointFiveStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            fourStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            fourPointFiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
            fiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
        }

        private void SetFourPointFiveStar()
        {
            pointFiveStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            oneStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            onePointFiveStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            twoStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            twoPointFiveStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            threeStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            threePointFiveStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            fourStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            fourPointFiveStar.SetImage(UIImage.FromFile(IconStarYellow), UIControlState.Normal);
            fiveStar.SetImage(UIImage.FromFile(IconStarBlank), UIControlState.Normal);
        }

        private void SetFiveStarRating()
        {
            pointFiveStar.SetImage(UIImage.FromFile(IconStarGreen), UIControlState.Normal);
            oneStar.SetImage(UIImage.FromFile(IconStarGreen), UIControlState.Normal);
            onePointFiveStar.SetImage(UIImage.FromFile(IconStarGreen), UIControlState.Normal);
            twoStar.SetImage(UIImage.FromFile(IconStarGreen), UIControlState.Normal);
            twoPointFiveStar.SetImage(UIImage.FromFile(IconStarGreen), UIControlState.Normal);
            threeStar.SetImage(UIImage.FromFile(IconStarGreen), UIControlState.Normal);
            threePointFiveStar.SetImage(UIImage.FromFile(IconStarGreen), UIControlState.Normal);
            fourStar.SetImage(UIImage.FromFile(IconStarGreen), UIControlState.Normal);
            fourPointFiveStar.SetImage(UIImage.FromFile(IconStarGreen), UIControlState.Normal);
            fiveStar.SetImage(UIImage.FromFile(IconStarGreen), UIControlState.Normal);
        }
    }
}