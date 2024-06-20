namespace Data.ViewModel.User
{
    public class UserVM
    {
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public bool? EmailComfirm { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? Address { get; set; }

        public DateOnly? Birthday { get; set; }

        public string? Avatar { get; set; }
    }
}
