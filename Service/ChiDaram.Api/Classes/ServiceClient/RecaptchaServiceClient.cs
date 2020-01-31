using System.Net.Http;
using System.Threading.Tasks;
using ChiDaram.Common.Classes;

namespace ChiDaram.Api.Classes.ServiceClient
{
    public class RecaptchaServiceClient : BaseServiceClient
    {
        private readonly RecaptchaConfig _recaptchaConfig;
        public RecaptchaServiceClient(HttpClient httpClient, RecaptchaConfig recaptchaConfig) : base(httpClient)
        {
            _recaptchaConfig = recaptchaConfig;
        }

        public async Task<RecaptchaResponse> VerifyToken(string token, string remoteIp)
        {
            var address = $"{_recaptchaConfig.SiteVerifyAddress}?secret={_recaptchaConfig.SecretKey}&response={token}&remoteip={remoteIp}";
            return await SendPostAsJsonRequest<RecaptchaResponse>(address, new { });
        }
    }
}
