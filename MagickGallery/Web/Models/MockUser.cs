namespace Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class MockUser
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
    }
}
