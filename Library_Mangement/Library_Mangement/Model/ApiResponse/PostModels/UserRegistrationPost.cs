using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Model.ApiResponse.PostModels
{
    public class UserRegistrationPost
    {
        public int RollNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string CollageName { get; set; }
        public string CurrentEducation { get; set; }
        public string ProfileAvatar { get; set; }
        public string Catagories { get; set; }
        public string DeviceId { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
