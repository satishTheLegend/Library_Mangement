using Library_Mangement.Resx;
using Library_Mangement.Validation;
using Library_Mangement.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Library_Mangement.ViewModels
{
    public class SpalshViewModel : ValidatableBase
    {
        #region Properties
        public readonly List<string> TitleList = new List<string>() { AppResources.Spash_Title_1, AppResources.Spash_Title_2, AppResources.Spash_Title_3};
        public readonly List<string> SummaryList = new List<string>() { AppResources.Splash_Summary_1, AppResources.Splash_Summary_2, AppResources.Splash_Summary_3};
        public readonly List<string> BGImageList = new List<string>() { "Screen_1.jpg", "Screen_2.jpg", "Screen_3.jpg" };
        
        private ImageSource bgImage = "Screen_1.jpg";
        public ImageSource BgImage
        {
            get { return bgImage; }
            set 
            { 
                bgImage = value;
                OnPropertyChanged(nameof(BgImage));
            }
        }

        private int id = 0;
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string summary;
        public string Summary
        {
            get { return summary; }
            set
            {
                summary = value;
                OnPropertyChanged(nameof(Summary));
            }
        }


        #endregion

        #region Constructor
        public SpalshViewModel()
        {
            NextClicked(true);
        }
        #endregion

        #region Command
        public ICommand NextCommand => new Command(() => NextClicked(true));
        public ICommand PrevCommand => new Command(() => NextClicked(false));
        #endregion

        #region Event Handlers
        public async void NextClicked(bool isNext)
        {
            if (Id == 3 && isNext)
            {
                await App.Current.MainPage.Navigation.PushAsync(new LandingView());
                return;
            }

            if (!isNext && (Id == 0 || Id == 1))
                return;
            if(isNext)
            {
                Id++;
            }
            else
            {
                Id--;
            }

            //Id = isNext ? Id++ : Id--;
            Title = TitleList[Id - 1];
            Summary = SummaryList[Id - 1];
            BgImage = BGImageList[Id - 1];
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

    }
}
