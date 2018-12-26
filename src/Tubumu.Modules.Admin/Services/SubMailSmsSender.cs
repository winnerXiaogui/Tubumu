using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tubumu.Modules.Framework.Services;

namespace Tubumu.Modules.Admin.Services
{
    public class SubMailSmsSender : ISmsSender
    {
        private readonly IHttpClientFactory _clientFactory;

        public SubMailSmsSender(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;

        }
        public async Task<bool> SendAsync(string mobile, string message, string expirationInterval = null)
        {
            var client = _clientFactory.CreateClient();

            var uri = new Uri("https://api.submail.cn/message/xsend.json");
            var httpContent = new FormUrlEncodedContent(new[]
            {
                // TODO: 改为从配置文件读取
                new KeyValuePair<string, string>("appid", "15360"),
                new KeyValuePair<string, string>("project", "9B3235"),
                new KeyValuePair<string, string>("signature", "xxxxxxxx"),
                new KeyValuePair<string, string>("to", mobile),
                new KeyValuePair<string, string>("vars", "{\"code\":\""+ message +"\",\"time\":\""+ expirationInterval +"\"}"),
            });
            try
            {
                var response = await client.PostAsync(uri, httpContent);
                var content = await response.Content.ReadAsStringAsync();
                // TODO: 检查短信发送结果
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
