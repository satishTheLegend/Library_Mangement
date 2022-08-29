using Acr.UserDialogs;
using Library_Mangement.Validation;
using Library_Mangement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Library_Mangement.ViewModels
{
    public class CategoryViewModel : ValidatableBase
    {
        #region Properties
        private ObservableCollection<Catagory> _catagories;
        public ObservableCollection<Catagory> Catagories
        {
            get => _catagories;
            set
            {
                _catagories = value;
                OnPropertyChanged(nameof(Catagories));
            }
        }
        #endregion

        #region Constructor
        public CategoryViewModel()
        {

        }
        #endregion

        #region Commands
        public ICommand CatagorySelectionCommand = new Command((frame) => CatagorySelectionClicked(frame as Frame));
        #endregion

        #region Event Handlers
        private static async void CatagorySelectionClicked(Frame frame)
        {
            var catagorydata = frame.BindingContext as Catagory;
            if(catagorydata != null)
            {
                await App.Current.MainPage.Navigation.PushAsync(new BooksView(catagorydata.CategoryName));
            }
        }
        #endregion

        #region Public Mathods
        public async Task ExploreBooks()
        {
            UserDialogs.Instance.ShowLoading();
            var catList = await App.Database.CodesMaster.GetListByGroupName("Catagory");
            List<Catagory> catagoryList = new List<Catagory>();
            foreach (var catItem in catList)
            {
                Catagory catagory = new Catagory()
                {
                    CategoryName = catItem.CodeName
                };
                catagoryList.Add(catagory);
            }
            Catagories = new ObservableCollection<Catagory>(catagoryList);
            UserDialogs.Instance.HideLoading();
        }
        #endregion

        #region Private Method

        #endregion
    }

    public class Catagory
    {
        public string CategoryImage { get; set; } = "Language.png";
        public string CategoryName { get; set; }
    }
}
