using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Model.ApiResponse
{
    public class userDataResp
    {
        public int id { get; set; }
        public int rollNo { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string password { get; set; }
        public string collageName { get; set; }
        public string currentEducation { get; set; }
        public string profileAvatar { get; set; }
        public object catagories { get; set; }
        public string gender { get; set; }
        public DateTime birthDate { get; set; }
        public DateTime userTokenCreatedDate { get; set; }
        public object userToken { get; set; }
    }

    public class UserRegistrationApiResp
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public userDataResp data { get; set; }
    }
}
