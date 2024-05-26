namespace Data.Models
{
    public class UserToken
    {
        public int Id { get; set; }
        public Guid UserId {  get; set; }
        public string AccessToken {  get; set; }
        public DateTime ExpireadDateAccessToken { get; set; }
        public DateTime ExpireadDateRefreshToken { get; set; }
        public string RefreshToken { get; set; }
        public string CodeRefreshToken { get; set; }

        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
    }
}
