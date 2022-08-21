using Library_Mangement.Validation;
using Library_Mangement.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace Library_Mangement.Model.UIBinding
{
    public class ProfileBinding : ValidatableBase
    {
        private ObservableCollection<DynamicPropertyDataViewModel> _fieldItems;
        public ObservableCollection<DynamicPropertyDataViewModel> FieldItems
        {
            get => _fieldItems;
            set
            {
                _fieldItems = value;
                OnPropertyChanged(nameof(FieldItems));
            }
        }

        private ImageSource _defaultProfile = "user.png";
        public ImageSource DefaultProfile
        {
            get => _defaultProfile;
            set
            {
                _defaultProfile = value;
                OnPropertyChanged(nameof(DefaultProfile));
            }
        }

        private bool _profileVisible = false;
        public bool ProfileVisible
        {
            get => _profileVisible;
            set
            {
                _profileVisible = value;
                OnPropertyChanged(nameof(ProfileVisible));
            }
        }
    }

}
