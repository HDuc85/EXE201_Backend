using Newtonsoft.Json.Linq;
using System.Text;

namespace Service.Helper.Address
{
    public class AddressHelper : IAddressHelper
    {
        private readonly IConfiguration _configuration;

        public AddressHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> AddressFormater(string address)
        {
            using (var client = new HttpClient())
            {
                var url = _configuration["VTP:AddressFormatUrl"];
                // Tạo nội dung của request với param "address"
                var content = new StringContent($"{{\"address\":\"{address}\"}}", Encoding.UTF8, "application/json");

                // Gửi request POST
                var response = await client.PostAsync(url, content);

                // Đọc phản hồi
                var responseString = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(responseString);
                string result = jsonResponse["formattedAddress"].ToString();


               return result;
            }
        }
    }
}
