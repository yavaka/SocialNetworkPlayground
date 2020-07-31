namespace SocialMedia.Services.Models
{
    using SocialMedia.Data.Models;
    using System;

    public class UserServiceModel
    {
        public UserServiceModel()
        {
        }

        public UserServiceModel(User user)
        {
            this.Id = user.Id;
            this.UserName = user.UserName;
            this.FullName = user.FullName;
            this.Country = user.Country;
            this.DateOfBirth = user.DOB;
        }

        public string Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Country { get; set; }

        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Wether the check box in a view is checked
        /// </summary>
        public bool Checked { get; set; } = false;
    }
}
