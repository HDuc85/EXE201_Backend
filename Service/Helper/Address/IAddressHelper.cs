
namespace Service.Helper.Address
{
    public interface IAddressHelper
    {
        Task<string> AddressFormater(string address);
    }
}