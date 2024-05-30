using System.ComponentModel.DataAnnotations;

namespace Data.ViewModel.User
{
    public class RegisterRequest
    {
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]

        public string ConfirmPassword { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        [DataType(DataType.Date)]
        public DateOnly Birthday { get; set; }
    }
}
