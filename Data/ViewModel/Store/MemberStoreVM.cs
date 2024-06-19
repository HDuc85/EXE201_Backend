namespace Data.ViewModel.Store
{
    public class MemberStoreVM
    {
        public int? StoreId { get; set; }
     
        public List<Member>? members { get; set; }
    }
    public class Member
    {
        public string? Name {  get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }   
        public DateOnly? Birthday { get; set; }
        public string? Avatar { get; set; }
    }
}
