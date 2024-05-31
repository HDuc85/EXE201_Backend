using System.ComponentModel.DataAnnotations;

namespace Data.ViewModel.User
{
    public class UpdateUserRequest
    {
        public Guid UserId {  get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly BirthDay { get; set; }
        public string Address { get; set; }
        public string Phonenumber { get; set; }
    }
}
