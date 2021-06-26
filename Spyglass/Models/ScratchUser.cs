using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Spyglass.Models
{
    public record ScratchUser
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("sys_id")]
        public int SysId { get; set; }

        [JsonPropertyName("joined")]
        public DateTime Joined { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("bio")]
        public string Bio { get; set; }

        [JsonPropertyName("work")]
        public string Work { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("school")]
        public object School { get; set; }
    }
}
