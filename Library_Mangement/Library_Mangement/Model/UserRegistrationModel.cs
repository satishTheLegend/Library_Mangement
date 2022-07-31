using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Library_Mangement.Model
{
    public class UserRegistrationModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required] 
        public string StudentId { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid e-mail id.")]
        public string Email { get; set; }
        [Required]
        [Phone]
        public long Phone { get; set; }
        [Required]
        public DateTime DOB { get; set; }
        [Required]
        public string CollageName { get; set; }
        [Required]
        public string Education { get; set; }
        [Required]
        public string UserName { get; set; }

        #region Validation Properties
        public bool FirstName_IconVisible_Property { get; set; } = false;
        public bool LastName_IconVisible_Property { get; set; } = false;
        public bool StudentId_IconVisible_Property { get; set; } = false;
        public bool Email_IconVisible_Property { get; set; } = false;
        public bool Phone_IconVisible_Property { get; set; } = false;
        public bool DOB_IconVisible_Property { get; set; } = false;
        public bool CollageName_IconVisible_Property { get; set; } = false;
        public bool Education_IconVisible_Property { get; set; } = false;
        public bool UserName_IconVisible_Property { get; set; } = false;
        #endregion

        public UserRegistrationModel()
        {

        }
    }
}
