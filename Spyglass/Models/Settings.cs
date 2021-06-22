using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spyglass.Models
{
    public enum EmbedPlayer
    {
        Scratch,
        TurboWarp
    }

    public record Settings
    {
        public Settings()
        {
            embedPlayer = EmbedPlayer.Scratch;
        }

        public Settings(EmbedPlayer embedPlayer)
        {
            this.embedPlayer = embedPlayer;
        }

        public EmbedPlayer embedPlayer { get; set; }
    }
}