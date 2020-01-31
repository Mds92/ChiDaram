using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChiDaram.Api.Classes.ServiceClient
{
    public class HtmlValidatorServiceClient : BaseServiceClient
    {
        public HtmlValidatorServiceClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<JObject> ValidateHtml(string html)
        {
            return await SendPostRequest<JObject>("", new StringContent(html.Normalize(), Encoding.UTF8, "text/html"));
        }
    }
}
