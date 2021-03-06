﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Tubumu.Modules.Framework.Services;

namespace Tubumu.Modules.Admin.Services
{
    public class SubMailSmsSender : ISmsSender
    {
        private readonly SubMailSmsSettings _subMailSmsSettings;
        private readonly IHttpClientFactory _clientFactory;

        public SubMailSmsSender(IOptions<SubMailSmsSettings> subMailSmsSettingsOptons, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _subMailSmsSettings = subMailSmsSettingsOptons.Value;

        }
        public async Task<bool> SendAsync(string mobile, string message, string expirationInterval = null)
        {
            var client = _clientFactory.CreateClient();

            var uri = new Uri("https://api.submail.cn/message/xsend.json");
            var httpContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("appid", _subMailSmsSettings.AppId),
                new KeyValuePair<string, string>("project", _subMailSmsSettings.Project),
                new KeyValuePair<string, string>("signature", _subMailSmsSettings.Signature),
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