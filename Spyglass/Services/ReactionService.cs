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
        private HttpClient Http;

        public List<Reaction> reactions;

        public ReactionService(HttpClient _Http)
        {
            Http = _Http;
        }

        public async Task Initialize()
        {
            reactions = await Http.GetFromJsonAsync<List<Reaction>>($"https://magnifier-api.potatophant.net/api/Reactions");
        }
    }
}
