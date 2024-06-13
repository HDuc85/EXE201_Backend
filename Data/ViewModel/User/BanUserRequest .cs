namespace Data.ViewModel.User
{
    public class BanUserRequest
    {
        public Guid? UserId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Reason { get; set; }
        public DateTime? BanUntil { get; set; }
    }
}
