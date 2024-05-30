namespace Data.ViewModel.System
{
    public class JwtModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Username {  get; set; }
        public string FirstName { get; set; }
        public DateTime AccessTokenExpiredDate {  get; set; }
    }
}
