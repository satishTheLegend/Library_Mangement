using Library_Mangement.Database.Models;
using Library_Mangement.Model;
using Library_Mangement.Validation;
using Library_Mangement.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Library_Mangement.ViewModels
{
    public class CatagoryChipsViewModel : ValidatableBase
    {
        #region Properties

        
        private List<string> _catagories;

        public List<string> Catagories
        {
            get
            {
                return _catagories;
            }
            set
            {
                _catagories = value;
                OnPropertyChanged(nameof(Catagories));

            }
        }
        private List<string> _selectedItems = new List<string>();

        public List<string> SelectedItems
        {
            get
            {
                return _selectedItems;
            }
            set
            {
                _selectedItems = value;
                OnPropertyChanged(nameof(SelectedItems));

            }
        }

        #endregion

        #region Constructor
        public CatagoryChipsViewModel()
        {

        }
        #endregion

        #region Commands
        public ICommand SaveCommand => new Command(async () => await Save());

        #endregion

        #region Event Handlers
        private async Task Save()
        {
            if(SelectedItems?.Count > 0)
            {
                await App.Database.User.AddCatagories(SelectedItems, "");
                await App.Current.MainPage.Navigation.PushAsync(new HomeView());
            }
        }
        #endregion

        #region Public Methods
        public async Task LoadCatagories()
        {
            var catagoryList = await App.Database.CodesMaster.GetListByContainingId("Cat");
            if(catagoryList?.Count > 0)
            {
                Catagories = catagoryList.Select(y=> y.CodeText).ToList();
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
