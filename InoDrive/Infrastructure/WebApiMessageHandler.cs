﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebUI.Infrastructure
{
    public class LanguageMessageHandler : DelegatingHandler
    {
        private const string LangruRU = "ru-RU";
        private const string LangenGB = "en-GB";

        private readonly List<string> _supportedLanguages = new List<string> { LangruRU,  LangenGB };

        private bool SetHeaderIfAcceptLanguageMatchesSupportedLanguage(HttpRequestMessage request)
        {
            foreach (var lang in request.Headers.AcceptLanguage)
            {
                if (_supportedLanguages.Contains(lang.Value))
                {
                    SetCulture(request, lang.Value);
                    return true;
                }
            }

            return false;
        }

        private bool SetHeaderIfGlobalAcceptLanguageMatchesSupportedLanguage(HttpRequestMessage request)
        {
            foreach (var lang in request.Headers.AcceptLanguage)
            {
                var globalLang = lang.Value.Substring(0, 2);
                if (_supportedLanguages.Any(t => t.StartsWith(globalLang)))
                {
                    SetCulture(request, _supportedLanguages.FirstOrDefault(i => i.StartsWith(globalLang)));
                    return true;
                }
            }

            return false;
        }

        private void SetCulture(HttpRequestMessage request, string lang)
        {
            request.Headers.AcceptLanguage.Clear();
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(lang));
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!SetHeaderIfAcceptLanguageMatchesSupportedLanguage(request))
            {
                // Whoops no localization found. Lets try Globalisation
                if (!SetHeaderIfGlobalAcceptLanguageMatchesSupportedLanguage(request))
                {
                    // no global or localization found
                    SetCulture(request, LangenGB);
                }
            }

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}