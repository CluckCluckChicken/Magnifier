using Blazored.LocalStorage;
using Magnifier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Spyglass.Services
{
    public class AuthenticationService
    {
        private HttpClient Http;
        private ISyncLocalStorageService LocalStorage;

        public User user;

        public string token;

        public AuthenticationService(HttpClient _Http, ISyncLocalStorageService _LocalStorage)
        {
            Http = _Http;
            LocalStorage = _LocalStorage;
        }

        public void Initialize()
        {
            user = LocalStorage.GetItem<User>("user");
        }

        public async Task<string> Login(string authCode)
        {
            token = await Http.GetStringAsync($"https://localhost:5001/api/Auth/token?code={authCode}");

            LocalStorage.SetItem("token", token);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:5001/api/Auth/user");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", LocalStorage.GetItem<string>("token"));
            HttpResponseMessage response = await Http.SendAsync(request);
            string json = await response.Content.ReadAsStringAsync();

            user = System.Text.Json.JsonSerializer.Deserialize<User>(json);

            LocalStorage.SetItem("user", user);

            return token;
        }
    }
}
