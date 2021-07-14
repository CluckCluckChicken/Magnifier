using Blazored.LocalStorage;
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
    public class ReactionService
    {
        private AppSettings AppSettings;

        private HttpClient Http;

        public List<Reaction> reactions;

        public ReactionService(AppSettings _AppSettings, HttpClient _Http)
        {
            AppSettings = _AppSettings;
            Http = _Http;
        }

        public async Task Initialize()
        {
            reactions = await Http.GetFromJsonAsync<List<Reaction>>($"{AppSettings.ApiRoot}/Reactions");
        }
    }
}
