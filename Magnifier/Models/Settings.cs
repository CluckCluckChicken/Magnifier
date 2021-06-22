using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public enum EmbedPlayer
    {
        Scratch,
        TurboWarp
    }

    public record Settings
    {
        public Settings(EmbedPlayer embedPlayer)
        {
            this.embedPlayer = embedPlayer;
        }

        public EmbedPlayer embedPlayer { get; set; }
    }
}
