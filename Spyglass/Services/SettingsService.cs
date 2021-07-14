using Magnifier.Models;
using Spyglass.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Spyglass.Services
{
    public class SettingsService
    {
        private AppSettings AppSettings;

        private HttpClient Http;

        private AuthenticationService AuthenticationService;

        public Settings settings;

        public SettingsService(AppSettings _AppSettings, HttpClient _Http, AuthenticationService _authenticationService)
        {
            AppSettings = _AppSettings;
            Http = _Http;
            AuthenticationService = _authenticationService;
        }

        public async Task Load()
        {
            if (AuthenticationService.user != null)
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{AppSettings.ApiRoot}/Auth/settings");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthenticationService.token);
                HttpResponseMessage response = await Http.SendAsync(request);
                string json = await response.Content.ReadAsStringAsync();

                settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(json);

                if (settings == null)
                {
                    await Save();

                    await Load();
                }
            }
        }

        public async Task Save()
        {
            if (settings == null)
            {
                settings = new Settings();
            }

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{AppSettings.ApiRoot}/Auth/settings?settings={System.Text.Json.JsonSerializer.Serialize(settings)}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthenticationService.token);
            HttpResponseMessage response = await Http.SendAsync(request);
        }
    }
}
