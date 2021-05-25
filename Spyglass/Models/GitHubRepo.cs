// yes this file has to exist ok. go away.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spyglass.Models
{
    public class GitHubRepo
    {
        public GitHubRepo(int stargazers_count)
        {
            this.stargazers_count = stargazers_count;
        }

        public int stargazers_count { get; set; }
    }
}
