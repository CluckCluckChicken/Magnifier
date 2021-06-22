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
        private HttpClient Http;

        private AuthenticationService AuthenticationService;

        public Settings settings;

        public SettingsService(HttpClient _Http, AuthenticationService _authenticationService)
        {
            Http = _Http;
            AuthenticationService = _authenticationService;
        }

        public async Task Load()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://magnifier-api.potatophant.net/api/Auth/settings");
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

        public async Task Save()
        {
            if (settings == null)
            {
                settings = new Settings(EmbedPlayer.Scratch);
            }

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"https://magnifier-api.potatophant.net/api/Auth/settings?settings={System.Text.Json.JsonSerializer.Serialize(settings)}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthenticationService.token);
            HttpResponseMessage response = await Http.SendAsync(request);
        }
    }
}
