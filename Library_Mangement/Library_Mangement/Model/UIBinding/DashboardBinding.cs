using Library_Mangement.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Library_Mangement.Model.UIBinding
{
    public class DashboardBinding : ValidatableBase
    {
        private ImageSource _profileImage = "user_default_img.png";
        public ImageSource ProfileImage
        {
            get => _profileImage;
            set
            {
                _profileImage = value;
                OnPropertyChanged(nameof(ProfileImage));
            }
        }
        private string _userName = "User";
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                if(!string.IsNullOrEmpty(_userName))
                {
                    _userName = $"Hello {_userName}!";
                }
                OnPropertyChanged(nameof(UserName));
            }
        }
        private bool _dashboardVisible = true;
        public bool DashboardVisible
        {
            get => _dashboardVisible;
            set
            {
                _dashboardVisible = value;
                OnPropertyChanged(nameof(DashboardVisible));
            }
        }

    }
}
